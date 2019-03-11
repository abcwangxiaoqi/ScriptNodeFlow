/*
 * start the flow when the object is created
 * drop the flow when the object is destroyed
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptNodeFlow
{
    public enum RuntimeState
    {
        Idle,//idle,can be called
        Wait,// was called but not finish completely 
        Finished//finish completely,and move the next one
    }

    [DisallowMultipleComponent]
    public class NodeController : MonoBehaviour
    {

#if UNITY_EDITOR
        public List<int> nodePathMessage { get; private set; }
        public List<RouterPathMessage> routerPathMessage { get; private set; }
#endif

        public event Action<bool> onFinish;

        public NodeCanvasData nodeFlowData;

        public string currentName;

        [HideInInspector]
        public int currentID;

        [HideInInspector]
        public string error;

        //id = key
        Dictionary<int, Node> entityMap = new Dictionary<int, Node>();

        //id + className = key
        Dictionary<string, RouterCondition> conditionMap = new Dictionary<string, RouterCondition>();

        NodeWindowData current;
        SharedData shareData = null;
        // Use this for initialization
        void Awake()
        {
#if UNITY_EDITOR
            nodePathMessage = new List<int>();
            routerPathMessage = new List<RouterPathMessage>();
#endif

            if (!string.IsNullOrEmpty(nodeFlowData.shareData))
            {
                shareData = Activator.CreateInstance(Type.GetType(nodeFlowData.shareData)) as SharedData;
            }

            current = nodeFlowData.GetEntrance();
            moveNext = true;
        }

        Type type;
        Node currentEntity;

        bool moveNext = false;
        public bool finished { get; private set; }

        RouterWindowData tempRouter;
        // Update is called once per frame
        void Update()
        {
            /*
             * execute the next oen
             */

            if (finished)
                return;

            if (!moveNext)
                return;

            if (current == null)
            {
                //end when current is null
                finished = true;

                if(onFinish!=null)
                {
                    onFinish.Invoke(true);
                }                
                
                return;
            }

            currentName = current.name;
            currentID = current.ID;

            try
            {
                if (!entityMap.ContainsKey(current.ID))
                {
                    type = Type.GetType(current.className);
                    currentEntity = Activator.CreateInstance(type, shareData) as Node;
                    entityMap.Add(current.ID, currentEntity);
                }
                currentEntity = entityMap[current.ID];

                currentEntity.run();
                moveNext = false;
            }
            catch (Exception e)
            {
                finished = true;
                if (onFinish != null)
                {
                    onFinish.Invoke(false);
                }
                error = e.Message;
                throw;
            }
        }

        WindowDataBase tempDataBase;
        RouterCondition tempCondition;
        private void LateUpdate()
        {
            if (finished)
                return;

            if (moveNext)
                return;

            if (currentEntity == null)
                return;

            if (currentEntity.State == RuntimeState.Finished)
            {

#if UNITY_EDITOR

                if (!nodePathMessage.Contains(current.ID))
                {
                    nodePathMessage.Add(current.ID);
                }

#endif

                currentEntity.reset();
                moveNext = true;

                tempDataBase = nodeFlowData.Get(current.nextWindowId);
                if (null == tempDataBase)
                {
                    current = null;
                }
                else if (tempDataBase.type == NodeType.Router)
                {
                    tempRouter = tempDataBase as RouterWindowData;
                    currentID = tempRouter.ID;
                    currentName = tempRouter.name;

                    try
                    {
                        for (int i = 0; i < tempRouter.conditions.Count; i++)
                        {
                            RouterWindowConditionData item = tempRouter.conditions[i];

                            string key = string.Format("{0}+{1}", tempRouter.ID, item.className);
                            if (!conditionMap.ContainsKey(key))
                            {
                                type = Type.GetType(item.className);

                                tempCondition = Activator.CreateInstance(type, shareData) as RouterCondition;

                                conditionMap.Add(key, tempCondition);
                            }

                            tempCondition = conditionMap[key];

                            if (!tempCondition.justify())
                            {
                                continue;
                            }

#if UNITY_EDITOR
                            addRouterPathMessage(tempRouter, i);
#endif

                            tempDataBase = nodeFlowData.Get(item.nextWindowId);
                            if (null == tempDataBase)
                            {
                                current = null;
                            }
                            else
                            {
                                current = tempDataBase as NodeWindowData;
                            }
                            return;
                        }

#if UNITY_EDITOR
                        addRouterPathMessage(tempRouter, -1);
#endif

                        tempDataBase = nodeFlowData.Get(tempRouter.nextWindowId);
                        if (null == tempDataBase)
                        {
                            current = null;
                        }
                        else
                        {
                            current = tempDataBase as NodeWindowData;
                        }
                    }
                    catch (Exception e)
                    {
                        error = e.Message;
                        finished = true;
                        if (onFinish != null)
                        {
                            onFinish.Invoke(false);
                        }                        
                        throw;
                    }
                }
                else if (tempDataBase.type == NodeType.Canvas)
                {
                    CanvasWindowData data = tempDataBase as CanvasWindowData;
                    if (null == data.canvasData)
                    {
                        GameObject canvas= new GameObject(data.canvasData.name);
                        canvas.transform.SetParent(transform);
                        NodeController controller = canvas.AddComponent<NodeController>();
                        controller.nodeFlowData = data.canvasData;
                        controller.onFinish += (bool b) =>
                        {
                            Destroy(canvas);

                            if (b == false)
                            {
                                current = null;
                                if (onFinish != null)
                                {
                                    onFinish(false);
                                }
                            }
                            else
                            {
                                //nodeFlowData.Get(data.nextWindowId)
                            }
                        };
                    }
                }
                else //NodeType.Node
                {
                    current = tempDataBase as NodeWindowData;
                }
            }
        }

#if UNITY_EDITOR
        void addRouterPathMessage(RouterWindowData router, int coditionIndex)
        {
            RouterPathMessage rpm = routerPathMessage.Find((RouterPathMessage m) =>
            {
                return m.id == router.ID;
            });

            if (rpm == null)
            {
                routerPathMessage.Add(new RouterPathMessage(router.ID, coditionIndex));
            }
            else
            {
                rpm.conditionIndex = coditionIndex;
            }
        }
#endif

        private void OnDestroy()
        {
            shareData = null;

#if UNITY_EDITOR
            nodePathMessage = null;
            routerPathMessage = null;
#endif

            if (finished)
                return;

            if (currentEntity != null)
            {
                currentEntity.stop();
            }
        }

        public T LoadShareData<T>()
            where T : SharedData, new()
        {
            if (null == shareData)
                return null;

            return shareData as T;
        }
    }




#if UNITY_EDITOR
    public class RouterPathMessage
    {
        public RouterPathMessage(int _id, int _conditionIndex)
        {
            this.id = _id;
            this.conditionIndex = _conditionIndex;
        }

        public int id { get; private set; }

        public int conditionIndex;
    }
#endif
}
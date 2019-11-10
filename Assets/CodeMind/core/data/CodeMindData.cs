using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace CodeMind
{
    public enum PlayMode
    {
        Once,
        Loop
    }

    [Serializable]
    public class CodeMindData : ScriptableObject
    {
        public PlayMode mode = PlayMode.Once;

        public SharedData shareData;

        public StartWindowData start = new StartWindowData();
        //public List<NodeWindowData> nodelist = new List<NodeWindowData>();
        public List<Node> nodelist = new List<Node>();
        public List<RouterWindowData> routerlist = new List<RouterWindowData>();
        public List<CodeMindWindowData> subCodeMindlist = new List<CodeMindWindowData>();

        public string desc;

        internal WindowDataBase Get(string id)
        {
            WindowDataBase data = null;

            /*data = nodelist.Find(windowData => { return windowData.ID == id; });
            if (data != null)
                return data;*/

            data = routerlist.Find(windowData => { return windowData.ID == id; });
            if (data != null)
                return data;

            data = subCodeMindlist.Find(windowData => { return windowData.ID == id; });

            return data;
        }

        public NodeWindowData AddNode(Vector2 position)
        {
            NodeWindowData node = new NodeWindowData();
            node.position = position;
            //nodelist.Add(node);
            return node;
        }

        public Node AddCustomNode(Type type,Vector2 position)
        {
            var data = ScriptableObject.CreateInstance(type) as Node;
            data.name = data.ID;
            data.winPos = position;

            UnityEditor.AssetDatabase.AddObjectToAsset(data, this);

            nodelist.Add(data);

            return data;
        }

        public RouterWindowData AddRouter(Vector2 position)
        {
            RouterWindowData router = new RouterWindowData();
            router.position = position;
            routerlist.Add(router);
            return router;
        }

        public CodeMindWindowData AddSubCanvas(Vector2 position)
        {
            CodeMindWindowData subCanvas = new CodeMindWindowData();
            subCanvas.position = position;
            subCodeMindlist.Add(subCanvas);
            return subCanvas;
        }


        /// <summary>
        /// Instantiate this instance.
        /// </summary>
        /// <returns>The instantiate.</returns>
        public GameObject Instantiate(out CodeMindController controller, Transform root = null)
        {
            GameObject gameObject = new GameObject(this.name, typeof(CodeMindController));
            gameObject.transform.SetParent(root);
            DontDestroyOnLoad(gameObject);
            controller = gameObject.GetComponent<CodeMindController>();
            controller.mindData = this;
            return gameObject;
        }

        internal void OnReset()
        {
            start.OnReset();

            foreach (var item in nodelist)
            {
                item.Reset(shareData);
            }

            foreach (var item in routerlist)
            {
                item.OnReset();
            }

            foreach (var item in subCodeMindlist)
            {
                item.OnReset();
            }
        }

        internal void OnCreate()
        {
            if (shareData != null)
            {
                shareData.OnCreate();
            }

            foreach (var item in nodelist)
            {
                item.Init(shareData);
            }

            foreach (var item in routerlist)
            {
                item.OnCreate(shareData);
            }

            foreach (var item in subCodeMindlist)
            {
                item.OnCreate(shareData);
            }
        }

        internal void OnAssetDestroy()
        {
            if (shareData != null)
            {
                shareData.OnObjectDestroy();
            }

            foreach (var item in nodelist)
            {
                item.Destroy(shareData);
            }

            foreach (var item in routerlist)
            {
                item.OnObjectDestroy(shareData);
            }

            foreach (var item in subCodeMindlist)
            {
                item.OnObjectDestroy(shareData);
            }
        }


#if UNITY_EDITOR

        public void Compile()
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(this);

            foreach (var item in nodelist)
            {
                if (item != null)
                    continue;

                Debug.LogErrorFormat("{0} : Node '{1}' script is invalid", path, item.name);
            }


            List<RouterWindowData> routers = routerlist;
            foreach (var item in routers)
            {
                foreach (var condItem in item.conditions)
                {
                    /*if (condItem.routerCondition != null)
                        continue;*/
                    
                    if (condItem != null)
                        continue;

                    Debug.LogErrorFormat("{0} : Condition '{1}' of Router '{2}' script is invalid", path, condItem.name, item.name);
                }
            }
        }
#endif
    }
}
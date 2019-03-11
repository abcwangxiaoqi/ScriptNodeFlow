using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptNodeFlow
{
    [Serializable]
    public class NodeCanvasData : ScriptableObject
    {
        public string shareData;

        public StartWindowData start = new StartWindowData();
        public List<NodeWindowData> nodelist = new List<NodeWindowData>();
        public List<RouterWindowData> routerlist = new List<RouterWindowData>();
        public List<CanvasWindowData> canvaslist = new List<CanvasWindowData>();

        public WindowDataBase Get(int id)
        {
            WindowDataBase data = null;

            data = nodelist.Find(windowData => { return windowData.ID == id;});
            if (data != null)
                return data;

            data = routerlist.Find(windowData => { return windowData.ID == id; });
            if (data != null)
                return data;

            data = canvaslist.Find(windowData => { return windowData.ID == id; });

            return data;
        }

        public NodeWindowData GetEntrance()
        {
            return nodelist.Find(data => { return data.ID == start.nextWindowId; });
        }
    }

    public enum NodeType
    {
        Node,
        Router,
        Start,
        Canvas
    }

    public class WindowDataBase
    {
        public int ID;
        public string name;
        public Vector2 position;
        public int nextWindowId = -1;

        public virtual NodeType type
        {
            get
            {
                return NodeType.Node;
            }
        }

    }

    [Serializable]
    public class NodeWindowData : WindowDataBase
    {
        public string className;

        public override NodeType type
        {
            get
            {
                return NodeType.Node;
            }
        }

        #region runtime

        private Node node;

        public void excute(SharedData sdata)
        {
            Type type = Type.GetType(className);
            node = Activator.CreateInstance(type, sdata) as Node;
            node.run();
        }

        public RuntimeState runtimeState
        {
            get { return node.State; }
        }

        #endregion
    }

    [Serializable]
    public class RouterWindowData : WindowDataBase
    {
        public List<RouterWindowConditionData> conditions = new List<RouterWindowConditionData>();

        public override NodeType type
        {
            get
            {
                return NodeType.Router;
            }
        }

        #region runtime

        [NonSerialized] public int runtimeNextId = -1;

        public void excute(SharedData sdata)
        {
            bool condFlag = false;
            for (int i = 0; i < conditions.Count; i++)
            {
                if (conditions[i].excute(sdata))
                {
                    condFlag = true;
                    runtimeNextId = conditions[i].nextWindowId;
                    break;
                }
            }

            if (condFlag == false)
            {
                runtimeNextId = nextWindowId;
            }
        }

        #endregion
    }

    [Serializable]
    public class CanvasWindowData : WindowDataBase
    {
        public NodeCanvasData canvasData = null;

        public override NodeType type
        {
            get
            {
                return NodeType.Canvas;
            }
        }

        #region runtime

        private GameObject go = null;
        public void excute(Transform root)
        {
            go = new GameObject(canvasData.name);
            go.transform.SetParent(root);
            NodeController nc = go.AddComponent<NodeController>();
            nc.nodeFlowData = canvasData;
            nc.onFinish += Nc_onFinish;
        }

        private void Nc_onFinish(bool obj)
        {
            GameObject.Destroy(go);


        }

        #endregion

    }

    [Serializable]
    public class StartWindowData : WindowDataBase
    {
        public StartWindowData()
        {
            ID = 0;
            name = "Start";
            position = new Vector2(300,300);
        }

        public override NodeType type
        {
            get
            {
                return NodeType.Start;
            }
        }

        #region runtime

        public void excute()
        {

        }

        #endregion
        }

    [Serializable]
    public class RouterWindowConditionData
    {
        public string className;
        public int nextWindowId = -1;

        private RouterCondition condition;
        public bool excute(SharedData sdata)
        {
            Type type = Type.GetType(className);

            condition = Activator.CreateInstance(type, sdata) as RouterCondition;

            return condition.justify();
        }
    }
}
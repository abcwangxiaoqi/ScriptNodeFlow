using System;
using System.Collections.Generic;
using UnityEngine;

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
        public List<NodeWindowData> nodelist = new List<NodeWindowData>();
        public List<RouterWindowData> routerlist = new List<RouterWindowData>();
        public List<CodeMindWindowData> subCodeMindlist = new List<CodeMindWindowData>();

        public string desc;

        public WindowDataBase Get(string id)
        {
            WindowDataBase data = null;

            data = nodelist.Find(windowData => { return windowData.ID == id; });
            if (data != null)
                return data;

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
            nodelist.Add(node);
            return node;
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
        public CodeMindController Instantiate(Transform root = null)
        {
            GameObject gameObject = new GameObject(this.name, typeof(CodeMindController));
            gameObject.transform.SetParent(root);
            DontDestroyOnLoad(gameObject);
            CodeMindController controller = gameObject.GetComponent<CodeMindController>();
            controller.mindData = this;
            return controller;
        }

        public void Reset()
        {
            start.reset();

            foreach (var item in nodelist)
            {
                item.reset();
            }

            foreach (var item in routerlist)
            {
                item.reset();
            }

            foreach (var item in subCodeMindlist)
            {
                item.reset();
            }
        }
    }
}
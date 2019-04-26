using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMind
{
    [Serializable]
    public class CodeMindData : ScriptableObject
    {
        public string ID = DateTime.UtcNow.ToString("yyMMddHHmmssff");

        public string shareData;

        public StartWindowData start = new StartWindowData();
        public List<NodeWindowData> nodelist = new List<NodeWindowData>();
        public List<RouterWindowData> routerlist = new List<RouterWindowData>();
        public List<CanvasWindowData> subCanvaslist = new List<CanvasWindowData>();

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

            data = subCanvaslist.Find(windowData => { return windowData.ID == id; });

            return data;
        }


        /// <summary>
        /// Instantiate this instance.
        /// </summary>
        /// <returns>The instantiate.</returns>
        public CodeMindController Instantiate()
        {
            GameObject gameObject = new GameObject(this.name, typeof(CodeMindController));
            CodeMindController controller = gameObject.GetComponent<CodeMindController>();
            controller.nodeFlowData = this;
            return controller;
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMind
{
    [Serializable]
    public class SubCanvasData : ScriptableObject
    {
        public StartWindowData start = new StartWindowData();
        public List<NodeWindowData> nodelist = new List<NodeWindowData>();
        public List<RouterWindowData> routerlist = new List<RouterWindowData>();

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

            return data;
        }
    }
}

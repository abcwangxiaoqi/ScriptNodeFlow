using System;
using System.Collections.Generic;
using UnityEngine;

namespace EditorCodeMind
{
    [Serializable]
    public class SubCanvasData : ScriptableObject
    {
        public StartWindowData start = new StartWindowData();
        public List<NodeWindowProxy> nodelist = new List<NodeWindowProxy>();
        public List<RouterWindowProxy> routerlist = new List<RouterWindowProxy>();

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

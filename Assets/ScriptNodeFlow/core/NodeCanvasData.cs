﻿using System;
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
        public List<CanvasWindowData> subCanvaslist = new List<CanvasWindowData>();

        public WindowDataBase Get(int id)
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
    }
}
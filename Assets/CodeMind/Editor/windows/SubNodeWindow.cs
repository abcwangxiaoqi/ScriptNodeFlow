using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeMind
{
    public class SubNodeWindow : NodeWindow
    {
        public SubNodeWindow(NodeWindowData itemData, List<BaseWindow> _windowList, CodeMindData _mindData)
            : base(itemData, _windowList, _mindData)
        {
        }
    }
}

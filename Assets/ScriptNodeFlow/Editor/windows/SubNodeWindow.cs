using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class SubNodeWindow : NodeWindow
    {
        public SubNodeWindow(Vector2 pos, List<BaseWindow> _windowList, string _canvasID)
            : base( pos, _windowList, _canvasID)
        {
        }

        public SubNodeWindow(NodeWindowData itemData, List<BaseWindow> _windowList, string _canvasID)
            : base(itemData, _windowList, _canvasID)
        {
        }
    }
}

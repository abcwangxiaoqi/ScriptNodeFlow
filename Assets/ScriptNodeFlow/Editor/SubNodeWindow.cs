using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class SubNodeWindow : NodeWindow
    {
        public SubNodeWindow(Vector2 pos, List<BaseWindow> _windowList, int _flowID)
            : base( pos, _windowList, _flowID)
        {
        }

        public SubNodeWindow(NodeWindowData itemData, List<BaseWindow> _windowList, int _flowID)
            : base(itemData, _windowList, _flowID)
        {
        }
    }
}

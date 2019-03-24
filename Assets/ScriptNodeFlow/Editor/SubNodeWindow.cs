using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class SubNodeWindow : NodeWindow
    {
        public SubNodeWindow(string orgin, Vector2 pos, List<BaseWindow> _windowList)
            : base(orgin, pos, _windowList)
        {
        }

        public SubNodeWindow(string orgin, NodeWindowData itemData, List<BaseWindow> _windowList)
            : base(orgin, itemData, _windowList)
        {
        }
    }
}

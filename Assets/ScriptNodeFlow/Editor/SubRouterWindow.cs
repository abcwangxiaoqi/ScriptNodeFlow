using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class SubRouterWindow : RouterWindow
    {
        public SubRouterWindow(string orgin, Vector2 pos, List<BaseWindow> _windowList)
            : base(orgin, pos, _windowList)
        {
        }

        public SubRouterWindow(string orgin, RouterWindowData itemData, List<BaseWindow> _windowList)
            : base(orgin, itemData, _windowList)
        {
        }
    }
}

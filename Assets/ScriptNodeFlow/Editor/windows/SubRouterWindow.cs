using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace CodeMind
{
    public class SubRouterWindow : RouterWindow
    {
        public SubRouterWindow(Vector2 pos, List<BaseWindow> _windowList,string _canvasID)
            : base( pos, _windowList, _canvasID)
        {
        }

        public SubRouterWindow(RouterWindowData itemData, List<BaseWindow> _windowList, string _canvasID)
            : base(itemData, _windowList, _canvasID)
        {
        }
    }
}

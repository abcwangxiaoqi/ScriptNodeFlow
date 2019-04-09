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
        public SubRouterWindow(Vector2 pos, List<BaseWindow> _windowList,int _flowID)
            : base( pos, _windowList, _flowID)
        {
        }

        public SubRouterWindow(RouterWindowData itemData, List<BaseWindow> _windowList, int _flowID)
            : base(itemData, _windowList, _flowID)
        {
        }
    }
}

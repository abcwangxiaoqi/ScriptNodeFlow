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
        public SubRouterWindow(RouterWindowData itemData, List<BaseWindow> _windowList, CodeMindData _mindData)
            : base(itemData, _windowList, _mindData)
        {
        }
    }
}

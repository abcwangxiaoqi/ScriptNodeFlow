using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeMind
{
    public class SubStartWindow : StartWindow
    {
        public SubStartWindow(StartWindowData itemData, List<BaseWindow> _windowList)
            : base(itemData, _windowList)
        {
        }
    }
}

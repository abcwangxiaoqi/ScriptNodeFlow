using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class SubStartWindow : StartWindow
    {
        public SubStartWindow(StartWindowData itemData, List<BaseWindow> _windowList, int _flowID)
            : base(itemData, _windowList, _flowID)
        {
        }
    }
}

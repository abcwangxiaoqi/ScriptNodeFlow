using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace EditorCodeMind
{

/// <summary>
    /// be used in Node
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class NodeUsageAttribute : Attribute
    {
        public NodeUsageAttribute(Type _targetType, string _viewText, string _winName, string _winDes)
        {
            winName = _winName;
            winDes = _winDes;
            targetType = _targetType;
            viewText = _viewText;
        }

        public string viewText { get; private set; }
        public string winName { get; private set; }
        public string winDes { get; private set; }
        public Type targetType { get; private set; }
    }

    /// <summary>
    /// Router usage attribute.
    /// </summary>
    public class RouterUsageAttribute : Attribute
    {
        public RouterUsageAttribute(string _viewText, string _winName, string _winDes, bool _showAddBtn = true)
        {
            winName = _winName;
            winDes = _winDes;
            showAddBtn = _showAddBtn;
            viewText = _viewText;
        }

        public string viewText { get; private set; }
        public string winName { get; private set; }
        public string winDes { get; private set; }
        public bool showAddBtn { get; private set; }
    }


    /// <summary>
    /// be used in Condition
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ConditionUsageAttribute : Attribute
    {
        public ConditionUsageAttribute(Type _conditionType, string _viewText, string _Name)
        {
            conditionName = _Name;
            conditionType = _conditionType;
            viewText = _viewText;
        }

        public string viewText { get; private set; }
        public string conditionName { get; private set; }
        public Type conditionType { get; private set; }
    }

    /// <summary>
    /// be used in SharedData
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class SharedDataUsageAttribute : Attribute
    {
        public SharedDataUsageAttribute(string _name, string _viewText)
        {
            name = _name;
            viewText = _viewText;
        }

        public string viewText { get; private set; }
        public string name { get; private set; }
    }
}

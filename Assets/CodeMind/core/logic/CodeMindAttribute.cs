using System;

namespace CodeMind
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CodeMindNodeAttribute : System.Attribute
    {
        public CodeMindNodeAttribute(string _showName, string _windowName, string _windowDes)
        {
            showName = _showName;
            windowName = _windowName;
            windowDes = _windowDes;
        }

        public string showName { get; private set; }
        public string windowName { get; private set; }
        public string windowDes { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CodeMindRouterAttribute : System.Attribute
    {
        public CodeMindRouterAttribute(Type _routerType, string _showName, string _windowName, string _windowDes, bool _allowModifyName = false, bool _allowModifyDes = false)
        {
            routerType = _routerType;
            showName = _showName;
            windowName = _windowName;
            windowDes = _windowDes;
            allowModifyName = _allowModifyName;
            allowModifyDes = _allowModifyDes;
        }

        public Type routerType { get; private set; }
        public string showName { get; private set; }
        public string windowName { get; private set; }
        public string windowDes { get; private set; }
        public bool allowModifyName { get; private set; }
        public bool allowModifyDes { get; private set; }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class CodeMindConditionAttribute : System.Attribute
    {
        public CodeMindConditionAttribute(Type _conditionType, string _showName,string _name)
        {
            name = _name;
            conditionType = _conditionType;
            showName = _showName;
        }

        public string name { get; private set; }
        public Type conditionType { get; private set; }
        public string showName { get; private set; }
    }
}
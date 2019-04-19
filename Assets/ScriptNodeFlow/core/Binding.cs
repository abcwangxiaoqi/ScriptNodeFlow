using System;

namespace ScriptNodeFlow
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ShareDataBinding : Attribute
    {
        public ShareDataBinding(string canvasid)
        {
            CanvasID = canvasid;
        }
        public string CanvasID;

        public const string Format = "[ShareDataBinding(\"{0}\")]";
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class NodeBinding : Attribute
    {
        public NodeBinding(string canvasid,string windowid)
        {
            CanvasID = canvasid;
            WindowID = windowid;
        }
        public string CanvasID;
        public string WindowID;

        public const string Format = "[NodeBinding(\"{0}\",\"{1}\")]";
    }
    
    [AttributeUsage(AttributeTargets.Class)]
    public class RouterBinding : Attribute
    {
        public RouterBinding(string canvasid,string routerId, string conditionId)
        {
            RouterID = routerId;
            ConditionID = conditionId;
            CanvasID = canvasid;
        }
        public string CanvasID;
        public string RouterID;
        public string ConditionID;

        public const string Format = "[RouterBinding(\"{0}\",\"{1}\",\"{2}\")]";
    }
}

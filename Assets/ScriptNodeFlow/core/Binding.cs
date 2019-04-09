using System;

namespace ScriptNodeFlow
{
    [AttributeUsage(AttributeTargets.Class)]
    public class BindingFlow : Attribute
    {
        public BindingFlow(int id)
        {
            ID = id;
        }
        public int ID;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class BindingNode : Attribute
    {
        public BindingNode(string id)
        {
            ID = id;
        }
        public string ID;
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class BindingRouter : Attribute
    {
        public BindingRouter(string routerId, string coditionId)
        {
            RouterID = routerId;
            CoditionId = coditionId;
        }
        public string RouterID;
        public string CoditionId;
    }
}

using UnityEngine;

namespace ScriptNodeFlow
{
    public enum RuntimeState
    {
        Idle,//idle,can be called
        Running,// was called but not finish completely 
        Error,//something wrong
        Finished//finish completely,and move the next one
    }

    public enum NodeType
    {
        Node,
        Router,
        Start,
        SubCanvas
    }

    public abstract class WindowDataBase
    {
        public int ID;
        public string name;
        public Vector2 position;
        public int nextWindowId = -1;

        public virtual NodeType type
        {
            get
            {
                return NodeType.Node;
            }
        }

        #region runtime

        public RuntimeState runtimeState { get; protected set; }
        public string runtimeError { get; protected set; }
        public virtual void exit()
        { }

        public virtual void reset()
        {
            runtimeState = RuntimeState.Idle;
        }

        #endregion

    }
}

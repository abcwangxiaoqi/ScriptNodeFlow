using System;
using UnityEngine;

namespace CodeMind
{
    public enum RuntimeState
    {
        Idle=0,//idle,can be called
        Running=1,// was called but not finish completely 
        Error=2,//something wrong
        Finished=3//finish completely,and move the next one
    }

    public enum NodeType
    {
        Node,
        Router,
        Start,
        SubCodeMind,
    }

    public abstract class WindowDataBase
    {
        public string ID = Guid.NewGuid().ToString();
        public string name;
        public Vector2 position;
        public string nextWindowId = null;

        public string desc;

        public virtual NodeType type
        {
            get
            {
                return NodeType.Node;
            }
        }

        #region runtime

        [NonSerialized]
        public RuntimeState runtimeState = RuntimeState.Idle;

        public string runtimeError { get; protected set; }

        public virtual void OnCreate(SharedData sharedData)
        {
            
        }

        public virtual void OnObjectDestroy(SharedData sharedData)
        {
        }

        public abstract void OnPlay(CodeMindController mindController);

        public virtual void OnUpdate(CodeMindController mindController) { }

        public virtual void OnReset()
        {
            runtimeState = RuntimeState.Idle;
        }

        #endregion

    }
}

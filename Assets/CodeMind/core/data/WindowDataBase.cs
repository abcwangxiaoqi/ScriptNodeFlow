using System;
using UnityEngine;

namespace EditorCodeMind
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

        protected SharedData m_SharedData { get; private set; }
        public void InitData(SharedData sharedData)
        {
            m_SharedData = sharedData;
        }

        [NonSerialized]
        public RuntimeState runtimeState = RuntimeState.Idle;

        public string runtimeError { get; protected set; }

        public void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake(){}

        public void Start()
        {
            OnStart();
        }

        protected virtual void OnStart() { }

        public void ProcessUpdate()
        {
            OnProcessUpdate();
        }

        protected virtual void OnProcessUpdate() { }

        public void ProcessLateUpdate()
        {
            OnProcessLateUpdate();
        }

        protected virtual void OnProcessLateUpdate() { }

        public void Destroy()
        {
            OnDestroy();
        }
        protected virtual void OnDestroy() { }

        public void Enter(EditorModeCodeMindController mindController)
        {
            OnEnter(mindController);            
        }


        protected virtual void OnEnter(EditorModeCodeMindController mindController)
        {}

        public void Exist()
        {
            OnExist();
        }

        protected virtual void OnExist()
        {}


        //public virtual void OnUpdate(CodeMindController mindController) { }


        public void Reset()
        {
            runtimeState = RuntimeState.Idle;
            OnReset();
        }

        protected virtual void OnReset()
        {
            
        }

        #endregion

    }
}

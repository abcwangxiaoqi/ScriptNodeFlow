
using System;
using UnityEngine;

namespace CodeMind
{
    public abstract class Node : ScriptableObject
    {
        internal bool finished { get; private set; }

        internal string errorMessage { get; private set; }

        protected SharedData m_SharedData { get; private set; }

        internal void Init(SharedData data)
        {
            m_SharedData = data;
        }

        internal void OnCreate()
        {
            OnNodeCreate();
        }

        protected virtual void OnNodeCreate()
        {

        }

        internal void Enter()
        {
            finished = false;
            errorMessage = string.Empty;

            OnEnter();
        }

        protected virtual void OnEnter()
        {
            
        }

        internal void Exist()
        {
            OnExist();
        }

        protected virtual void OnExist()
        {
            
        }

        internal void OnProcessUpdate()
        {
            OnNodeUpdate();
        }

        protected virtual void OnNodeUpdate()
        {

        }

        internal void OnProcessLateUpdate()
        {
            OnNodeLateUpdate();
        }

        protected virtual void OnNodeLateUpdate()
        {

        }


        //you must call this when you're sure the execute method is finished completely,
        //then the current node move to the next one
        //
        //why be designed like this? 
        //cause maybe your execute method includes some asyn operations
        protected void moveNext(string error = null)
        {
            finished = true;
            errorMessage = error;
        }

        internal virtual void OnObjectDestroy()
        {
            OnNodeDestroy();
        }

        protected virtual void OnNodeDestroy()
        {

        }
    }
}
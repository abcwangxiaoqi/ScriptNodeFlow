
using System;
using UnityEngine;

namespace CodeMind
{
    public abstract class Node : ScriptableObject
    {
        internal bool finished { get; private set; }

        internal string errorMessage { get; private set; }

        internal void OnCreate(SharedData sharedData)
        {
            OnNodeCreate(sharedData);
        }

        protected virtual void OnNodeCreate(SharedData sharedData)
        {

        }

        internal void OnPlay(SharedData sharedData)
        {
            finished = false;
            errorMessage = string.Empty;

            OnNodePlay(sharedData);
        }

        protected virtual void OnNodePlay(SharedData sharedData)
        {

        }

        internal void ProcessUpdate(SharedData sharedData)
        {
            OnNodeUpdate(sharedData);
        }

        protected virtual void OnNodeUpdate(SharedData sharedData)
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

        internal virtual void OnObjectDestroy(SharedData sharedData)
        {
            OnNodeDestroy(sharedData);
        }

        protected virtual void OnNodeDestroy(SharedData sharedData)
        {

        }
    }
}
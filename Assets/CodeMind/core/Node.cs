
using System;
using UnityEngine;

namespace CodeMind
{
    public abstract class Node : ScriptableObject
    {
        public bool finished { get; private set; }
        public string errorMessage { get; private set; }

        public abstract void Play(SharedData sharedData);

        public virtual void ProcessUpdate(SharedData sharedData) { }


        //you must call this when you're sure the execute method is finished completely,
        //then the current node move to the next one
        //
        //why be designed like this? 
        //cause maybe your execute method includes some asyn operations
        protected void finish(string error = null)
        {
            finished = true;
            errorMessage = error;
        }

        public virtual void OnCreate(SharedData sharedData)
        {}

        public virtual void OnDelete(SharedData sharedData)
        {}
    }
}

using System;

namespace CodeMind
{
    public abstract class Node
    {
        public bool finished { get; private set; }
        public string errorMessage { get; private set; }
        
        protected SharedData shareData;
        public Node(SharedData data)
        {
            shareData = data;
        }

        public abstract void Play();

        public virtual void Update() { }

        //be called when destroy canvas
        public virtual void OnDestroy() { }
        

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
    }
}

using System;

namespace CodeMind
{
    public abstract class Node
    {
        public event Action<bool, string> onFinishEvent;
        protected SharedData shareData;
        public Node(SharedData data)
        {
            shareData = data;
        }

        public abstract void execute();

        //be called when flow is broken
        public virtual void stop()
        {

        }

        //you must call this when you're sure the execute method is finished completely,
        //then the current node move to the next one
        //
        //why be designed like this? 
        //cause maybe your execute method includes some asyn operations
        protected void finish(bool success, string error = null)
        {
            if (onFinishEvent != null)
            {
                onFinishEvent.Invoke(success, error);
            }
        }
    }
}
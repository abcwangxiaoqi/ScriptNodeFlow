using System;

namespace CodeMind
{
    [Serializable]
    public class NodeWindowData : WindowDataBase
    {
        public string className;

        public override NodeType type
        {
            get
            {
                return NodeType.Node;
            }
        }

        #region runtime

        private Node node;
        public void excute(SharedData sdata)
        {
            try
            {
                runtimeState = RuntimeState.Running;

                if (null == node)
                {
                    Type type = Type.GetType(className);
                    node = Activator.CreateInstance(type, sdata) as Node;
                    node.onFinishEvent += Node_onFinishEvent;
                }

                node.execute();
            }
            catch (Exception e)
            {
                runtimeError = e.Message;
                runtimeState = RuntimeState.Error;
                throw;
            }
        }

        private void Node_onFinishEvent(bool success, string error)
        {
            if (success)
            {
                runtimeState = RuntimeState.Finished;
            }
            else
            {
                runtimeState = RuntimeState.Error;
                runtimeError = error;
            }
        }


        public override void exit()
        {
            if (node == null)
                return;
            node.stop();
        }
        #endregion
    }
}

using System;

namespace CodeMind
{
    [Serializable]
    public class NodeWindowData : WindowDataBase
    {
        public NodeWindowData()
        {
            name = "Node Name";
        }

        public Node node;

        public override NodeType type
        {
            get
            {
                return NodeType.Node;
            }
        }

        #region runtime


        public override void play(CodeMindController mindController)
        {
            try
            {
                runtimeState = RuntimeState.Running;

                node.Play(mindController.mindData.shareData);
            }
            catch (Exception e)
            {
                runtimeError = e.Message;
                runtimeState = RuntimeState.Error;
                throw;
            }
        }

        public override void update(CodeMindController mindController)
        {            
            if(node.finished)
            {
                if(string.IsNullOrEmpty(node.errorMessage))
                {
                    runtimeState = RuntimeState.Finished;
                }
                else
                {
                    runtimeState = RuntimeState.Error;
                    runtimeError = node.errorMessage;
                }
                return;
            }

            node.ProcessUpdate(mindController.mindData.shareData);
        }

        #endregion
    }
}

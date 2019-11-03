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

        public sealed override NodeType type
        {
            get
            {
                return NodeType.Node;
            }
        }

        public string typeName;

        new public Type GetType()
        {
            return Type.GetType(typeName);
        }

        #region runtime

        public override void OnCreate(SharedData sharedData)
        {
            base.OnCreate(sharedData);

            node.OnCreate(sharedData);
        }

        public override void OnObjectDestroy(SharedData sharedData)
        {
            base.OnObjectDestroy(sharedData);

            node.OnObjectDestroy(sharedData);
        }


        public override void OnPlay(CodeMindController mindController)
        {
            try
            {
                runtimeState = RuntimeState.Running;

                node.OnPlay(mindController.mindData.shareData);
            }
            catch (Exception e)
            {
                runtimeError = e.Message;
                runtimeState = RuntimeState.Error;
                throw;
            }
        }

        public override void OnUpdate(CodeMindController mindController)
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

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

       // private Node node;

        public override void play(params object[] objs)
        {
            try
            {
                //SharedData sdata = objs[0] as SharedData;

                //runtimeState = RuntimeState.Running;

                //if (null == node)
                //{
                //    Type type = Type.GetType(node);
                //    node = Activator.CreateInstance(type, sdata) as Node;
                //}

                //node.Play();
            }
            catch (Exception e)
            {
                runtimeError = e.Message;
                runtimeState = RuntimeState.Error;
                throw;
            }
        }

        public override void update()
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

            node.Update();
        }

        public override void stop()
        {
            if (node == null)
                return;

            node.OnDestroy();
        }
        #endregion
    }
}

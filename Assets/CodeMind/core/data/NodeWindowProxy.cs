using System;
using UnityEngine;

namespace CodeMind
{
    [Serializable]//这个类不能 是 抽象类 否则 在编译后序列化会有问题
    public class NodeWindowProxy : WindowDataBase,IWindowsAsset
    {
        public NodeWindowProxy()
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

        public void AssetCreate()
        {
            OnAssetCreate();
        }

        protected virtual void OnAssetCreate()
        {
            
        }

        public void AssetDelete()
        {
            if(node!=null)
            {
                UnityEngine.Object.DestroyImmediate(node);
            }

            OnAssetDelete();
        }

        protected virtual void OnAssetDelete()
        {}


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

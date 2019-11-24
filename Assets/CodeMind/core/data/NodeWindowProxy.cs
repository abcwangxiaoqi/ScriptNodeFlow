using System;
using UnityEngine;

namespace EditorCodeMind
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

        protected override void OnAwake()
        {
            node.Init(m_SharedData);

            node.OnCreate();
        }

        protected override void OnDestroy()
        {
            node.OnObjectDestroy();
        }


        protected override void OnEnter(EditorModeCodeMindController mindController)
        {
            try
            {
                runtimeState = RuntimeState.Running;

                node.Enter();
            }
            catch (Exception e)
            {
                runtimeError = e.Message;
                runtimeState = RuntimeState.Error;
                throw;
            }
        }

        protected override void OnExist()
        {
            if (node == null)
                return;
            
            node.Exist();
        }

        protected override void OnProcessUpdate()
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

            node.OnProcessUpdate();
        }

        protected override void OnProcessLateUpdate()
        {
            if (node.finished)
                return;

            node.OnProcessLateUpdate();
        }

        #endregion
    }
}

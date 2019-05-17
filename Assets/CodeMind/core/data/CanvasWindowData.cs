using System;
using TreeEditor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMind
{
    [Serializable]
    public class CanvasWindowData : WindowDataBase
    {
        public CanvasWindowData()
        {
            name = "SubCanvas Name";
        }

        public SubCanvasData canvasData = null;

        public override NodeType type
        {
            get
            {
                return NodeType.SubCanvas;
            }
        }

        #region runtime

        private SubCodeMindController nc = null;
        private GameObject ncGameObject = null;
        public override void play(params object[] objs)
        {
            try
            {
                Transform root = objs[0] as Transform;
                SharedData shareData = objs[1] as SharedData;

                 runtimeState = RuntimeState.Running;
                if (ncGameObject == null)
                {
                    ncGameObject = new GameObject(canvasData.name);
                    //ncGameObject.hideFlags = HideFlags.HideInHierarchy;
                    ncGameObject.transform.SetParent(root);
                    nc = ncGameObject.AddComponent<SubCodeMindController>();
                    nc.subNodeFlowData = canvasData;
                    nc.shareData = shareData;
                    nc.onFinish += Nc_onFinish;
                }
            }
            catch (Exception e)
            {
                runtimeState = RuntimeState.Error;
                runtimeError = e.Message;

                if (ncGameObject != null)
                {
                    Object.Destroy(ncGameObject);
                }

                throw;
            }
        }

        private void Nc_onFinish(bool obj)
        {
            if (obj)
            {
                runtimeState = RuntimeState.Finished;
            }
            else
            {
                runtimeState = RuntimeState.Error;
            }
        }

        public override void reset()
        {
            base.reset();

            if (nc != null)
            {
                nc.stateReset();
            }
        }

        #endregion
    }
}

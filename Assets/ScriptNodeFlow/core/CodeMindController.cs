/*
 * start the flow when the object is created
 * drop the flow when the object is destroyed
 */

using System;
using UnityEngine;

namespace CodeMind
{
    [DisallowMultipleComponent]
    public class CodeMindController : MonoBehaviour
    {
        public event Action<bool> onFinish;

        public event Action<SharedData> onSharedDataInitialize;

        public CodeMindData nodeFlowData;

        SharedData shareData = null;
        // Use this for initialization
        void Start()
        {
            if (!string.IsNullOrEmpty(nodeFlowData.shareData))
            {
                shareData = Activator.CreateInstance(Type.GetType(nodeFlowData.shareData)) as SharedData;

                if (onSharedDataInitialize != null)
                {
                    onSharedDataInitialize(shareData);
                }
            }

            current = nodeFlowData.start;
        }

        public bool finished { get; private set; }

        private WindowDataBase current = null;

        // Update is called once per frame
        void Update()
        {
            if (finished)
                return;

            if (current == null)
                return;

            if (current.runtimeState != RuntimeState.Idle)
                return;

            if (current.type == NodeType.Start)
            {
                (current as StartWindowData).excute();
            }
            else if (current.type == NodeType.Node)
            {
                (current as NodeWindowData).excute(shareData);
            }
            else if (current.type == NodeType.Router)
            {
                (current as RouterWindowData).excute(shareData);
            }
            else if (current.type == NodeType.SubCanvas)
            {
                 (current as CanvasWindowData).excute(transform,shareData);
            }
        }

        private void LateUpdate()
        {
            if (finished)
                return;

            if (current == null)
                return;

            if (current.runtimeState == RuntimeState.Finished)
            {
                //get next
                if (current.type == NodeType.Router)
                {
                    current = nodeFlowData.Get((current as RouterWindowData).runtimeNextId);
                }
                else
                {
                    current = nodeFlowData.Get(current.nextWindowId);
                }

                if (current == null)
                {
                    finished = true;
                    if (onFinish != null)
                    {
                        onFinish.Invoke(true);
                    }
                }
                else
                {
                    current.reset();
                }
            }
            else if (current.runtimeState == RuntimeState.Error)
            {
                finished = true;
                if (onFinish != null)
                {
                    onFinish.Invoke(false);
                }
            }
        }


        private void OnDestroy()
        {
            shareData.Dispose();
            shareData = null;

            if (finished)
                return;

            if (current != null)
            {
                current.exit();
            }
        }
    }
}
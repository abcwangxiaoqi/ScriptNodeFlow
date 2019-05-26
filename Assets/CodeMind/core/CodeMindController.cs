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

        public CodeMindData mindData { get; internal set; }

        SharedData shareData = null;
        // Use this for initialization
        void Start()
        {
            if (mindData.shareData!=null && onSharedDataInitialize != null)
            {
                onSharedDataInitialize(mindData.shareData);
            }

            current = mindData.start;
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

            #region Node Update

            if (current.runtimeState == RuntimeState.Running)
            {
                current.update(this);
            }

            #endregion


            if (current.runtimeState != RuntimeState.Idle)
                return;

            current.play(this);

            /*if (current.type == NodeType.Start)
            {
                current.play(mindData);
            }
            else if (current.type == NodeType.Node ||
                current.type == NodeType.Router)
            {
                current.play(mindData);
            }
            else if (current.type == NodeType.SubCodeMind)
            {
                current.play(mindData);
            }*/
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
                    current = mindData.Get((current as RouterWindowData).runtimeNextId);
                }
                else
                {
                    current = mindData.Get(current.nextWindowId);
                }

                if (current == null)
                {
                    if(mindData.mode == PlayMode.Loop)
                    {
                        reset();
                    }
                    else
                    {
                        finished = true;
                        if (onFinish != null)
                        {
                            onFinish.Invoke(true);
                        }
                    }
                }
                /*else if (current.runtimeState == RuntimeState.Finished)
                {
                    current.reset();
                }*/
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

        public void reset()
        {
            mindData.Reset();

            finished = false;

            current = mindData.start;
        }

        public void Destroy()
        {
            Destroy(this);
        }

        void OnDestroy()
        {
        }
    }
}
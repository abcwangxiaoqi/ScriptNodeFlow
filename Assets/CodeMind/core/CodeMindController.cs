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
        public event Action onFinish;

        public CodeMindData mindData { get; internal set; }

        SharedData shareData = null;

        // Use this for initialization
        void Start()
        {
            mindData.OnCreate();

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

            if (current.runtimeState == RuntimeState.Running)
            {
                current.OnUpdate(this);
            }
            else if(current.runtimeState == RuntimeState.Idle)
            {
                current.OnPlay(this);
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
                    current = mindData.Get((current as RouterWindowProxy).runtimeNextId);
                }
                else
                {
                    current = mindData.Get(current.nextWindowId);
                }

                if (current == null)
                {
                    lastAdjust();
                }
                else if(current != null && current.runtimeState == RuntimeState.Finished)
                {
                    Debug.LogWarningFormat("Node \"{0}\" current state is Finished , but you still try to run ", current.name);
                    lastAdjust();
                }
            }
            else if (current.runtimeState == RuntimeState.Error)
            {
                finished = true;

                hasError = true;
            }
        }

        public bool hasError { get; private set; }

        void lastAdjust()
        {
            if (mindData.mode == PlayMode.Loop)
            {
                Reset();
            }
            else
            {
                finished = true;

                if(onFinish!=null)
                {
                    onFinish.Invoke();
                }
            }
        }

        public void Reset()
        {
            mindData.OnReset();

            finished = false;

            current = mindData.start;
        }

        private void OnDestroy()
        {
            if(!finished)
            {
                if (onFinish != null)
                {
                    onFinish.Invoke();
                }
            }

            mindData.OnAssetDestroy();
        }
    }
}
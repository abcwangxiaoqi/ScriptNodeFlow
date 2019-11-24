/*
 * start the flow when the object is created
 * drop the flow when the object is destroyed
 */

using System;
using UnityEngine;

namespace CodeMind
{
    [DisallowMultipleComponent]
    public class EditorModeCodeMindController
    {
        public event Action onFinish;

        public CodeMindData mindData { get; private set; }

        SharedData shareData = null;

        private void Awake()
        {
        }

        internal void Init(CodeMindData data)
        {
            mindData = data;

            mindData.OnAwake();
        }

        // Use this for initialization
        void Start()
        {
            mindData.OnStart();

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
                current.ProcessUpdate();
            }
            else if(current.runtimeState == RuntimeState.Idle)
            {
                current.Enter(this);
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
                current.Exist();

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
            else
            {
                //late update
                current.ProcessLateUpdate();
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

            mindData.OnDestroy();
        }
    }
}
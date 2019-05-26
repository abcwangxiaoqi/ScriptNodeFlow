using System;
using TreeEditor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMind
{
    [Serializable]
    public class CodeMindWindowData : WindowDataBase
    {
        public CodeMindWindowData()
        {
            name = "Sub Name";
        }

        public CodeMindData canvasData = null;

        public override NodeType type
        {
            get
            {
                return NodeType.SubCodeMind;
            }
        }

        #region runtime

        CodeMindController controller;

        public override void play(CodeMindController mindController)
        {
            runtimeState = RuntimeState.Running;

            if(controller == null)
            {
                controller = canvasData.Instantiate(mindController.transform);
                controller.onFinish += subFinish;
            }
        }

        void subFinish(bool success)
        {
            if(success)
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

            controller.reset();
        }

        #endregion
    }
}

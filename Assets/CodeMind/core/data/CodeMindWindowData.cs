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
        GameObject gameObject;

        public override void OnPlay(CodeMindController mindController)
        {
            runtimeState = RuntimeState.Running;

            if(controller == null)
            {
                gameObject = canvasData.Instantiate(out controller,mindController.transform);
                controller.onFinish += subFinish;
            }
        }

        public override void OnUpdate(CodeMindController mindController)
        {
            base.OnUpdate(mindController);

            if(controller.hasError)
            {
                runtimeState = RuntimeState.Error;
            }
        }

        void subFinish()
        {
             runtimeState = RuntimeState.Finished;
        }

        public override void OnReset()
        {
            base.OnReset();

            controller.Reset();
        }

        #endregion
    }
}

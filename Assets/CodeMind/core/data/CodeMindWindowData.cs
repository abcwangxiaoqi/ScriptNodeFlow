using System;
using TreeEditor;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorCodeMind
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

        EditorModeCodeMindController controller;
        GameObject gameObject;

        protected override void OnEnter(EditorModeCodeMindController mindController)
        {
            runtimeState = RuntimeState.Running;

            if(controller == null)
            {
                //gameObject = canvasData.Instantiate(out controller,mindController.transform);
                controller.onFinish += subFinish;
            }
        }

        protected override void OnProcessUpdate()
        {
            if(controller.hasError)
            {
                runtimeState = RuntimeState.Error;
            }
        }

        void subFinish()
        {
             runtimeState = RuntimeState.Finished;
        }

        protected override void OnReset()
        {
            controller.Reset();
        }

        #endregion
    }
}

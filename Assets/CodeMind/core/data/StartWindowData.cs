using System;
using UnityEngine;

namespace EditorCodeMind
{
    [Serializable]
    public class StartWindowData : WindowDataBase
    {
        public StartWindowData()
        {
            name = "Start";
            position = new Vector2(300, 300);
        }

        public override NodeType type
        {
            get
            {
                return NodeType.Start;
            }
        }

        #region runtime

        protected override void OnEnter(EditorModeCodeMindController mindController)
        {
            runtimeState = RuntimeState.Finished;
        }

        #endregion
    }
}

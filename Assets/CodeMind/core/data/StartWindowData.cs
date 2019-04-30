using System;
using UnityEngine;

namespace CodeMind
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

        public override void play(params object[] objs)
        {
            runtimeState = RuntimeState.Finished;
        }

        #endregion
    }
}

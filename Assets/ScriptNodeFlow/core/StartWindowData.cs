using System;
using UnityEngine;

namespace ScriptNodeFlow
{
    [Serializable]
    public class StartWindowData : WindowDataBase
    {
        public StartWindowData()
        {
            ID = 0;
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

        public void excute()
        {
            runtimeState = RuntimeState.Finished;
        }

        #endregion
    }
}

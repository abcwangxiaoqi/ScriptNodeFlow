using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMind
{
    [Serializable]
    public class RouterWindowData : WindowDataBase
    {
        public RouterWindowData()
        {
            name = "Router Name";
        }

        public List<RouterWindowConditionData> conditions = new List<RouterWindowConditionData>();

        public override NodeType type
        {
            get
            {
                return NodeType.Router;
            }
        }

        #region runtime

        [NonSerialized] public string runtimeNextId = null;

        public override void play(CodeMindController mindController)
        {
            try
            {
                runtimeState = RuntimeState.Running;
                bool condFlag = false;
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (conditions[i].excute(mindController.mindData.shareData))
                    {
                        condFlag = true;
                        runtimeNextId = conditions[i].nextWindowId;
                        break;
                    }
                }

                if (condFlag == false)
                {
                    runtimeNextId = nextWindowId;
                }

                runtimeState = RuntimeState.Finished;
            }
            catch (Exception e)
            {
                runtimeState = RuntimeState.Error;
                runtimeError = e.Message;
                Debug.LogError(e.Message);
                throw;
            }
        }

        #endregion
    }

    [Serializable]
    public class RouterWindowConditionData
    {
        public string ID = Guid.NewGuid().ToString();
        
        public string name = "Condition Name";

        public RouterCondition routerCondition;

        public string nextWindowId = null;

        public bool excute(SharedData sdata)
        {
            return routerCondition.justify(sdata);
        }
    }
}

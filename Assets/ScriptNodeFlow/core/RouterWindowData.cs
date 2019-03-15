using System;
using System.Collections.Generic;
using UnityEngine;

namespace ScriptNodeFlow
{
    [Serializable]
    public class RouterWindowData : WindowDataBase
    {
        public List<RouterWindowConditionData> conditions = new List<RouterWindowConditionData>();

        public override NodeType type
        {
            get
            {
                return NodeType.Router;
            }
        }

        #region runtime

        [NonSerialized] public int runtimeNextId = -1;
        public void excute(SharedData sdata)
        {
            try
            {
                runtimeState = RuntimeState.Running;
                bool condFlag = false;
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (conditions[i].excute(sdata))
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
        public string className;
        public int nextWindowId = -1;

        private RouterCondition condition;
        public bool excute(SharedData sdata)
        {
            if (condition == null)
            {
                Type type = Type.GetType(className);

                condition = Activator.CreateInstance(type, sdata) as RouterCondition;
            }

            return condition.justify();
        }
    }
}

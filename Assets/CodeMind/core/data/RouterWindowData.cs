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

        public override void play(params object[] objs)
        {
            try
            {
                SharedData sdata = objs[0] as SharedData;

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
        public string name = "Condition Name";

        public RouterCondition className;
        public string nextWindowId = null;

        private RouterCondition condition;
        public bool excute(SharedData sdata)
        {
            //if (condition == null)
            //{
            //    Type type = Type.GetType(className);

            //    condition = Activator.CreateInstance(type, sdata) as RouterCondition;
            //}

            //return condition.justify();
            return true;
        }
    }
}

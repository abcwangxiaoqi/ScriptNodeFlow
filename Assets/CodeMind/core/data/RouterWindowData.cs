using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMind
{

    public interface IWindow
    {
        string ID { get; set; }
        NodeType nodeType { get;}
        Vector2 position { get; set; }
        string winName{ get; set; }
        string winDes{ get; set; }

        void OnCreateAsset();
        void OnDeleteAsset();
    }



    [Serializable]
    public class RouterWindowData : WindowDataBase
    {
        public RouterWindowData()
        {
            name = "Router Name";
        }

        //public List<RouterWindowConditionData> conditions = new List<RouterWindowConditionData>();
        public List<RouterCondition> conditions = new List<RouterCondition>();

        public sealed override NodeType type
        {
            get
            {
                return NodeType.Router;
            }
        }

        #region runtime

        [NonSerialized] public string runtimeNextId = null;

        public override void OnCreate(SharedData sharedData)
        {
            base.OnCreate(sharedData);

            foreach (var item in conditions)
            {
                //item.awake(sharedData);
                item.OnCreate(sharedData);
            }
        }

        public override void OnObjectDestroy(SharedData sharedData)
        {
            base.OnObjectDestroy(sharedData);

            foreach (var item in conditions)
            {
                //item.destroy(sharedData);
                item.OnObjectDestroy(sharedData);
            }
        }

        public override void OnPlay(CodeMindController mindController)
        {
            try
            {
                runtimeState = RuntimeState.Running;
                bool condFlag = false;
                for (int i = 0; i < conditions.Count; i++)
                {
                   /* if (conditions[i].excute(mindController.mindData.shareData))
                    {
                        condFlag = true;
                        runtimeNextId = conditions[i].nextWindowId;
                        break;
                    }*/

                    if (conditions[i].Justify(mindController.mindData.shareData))
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

        public void awake(SharedData sdata)
        {
            routerCondition.OnCreate(sdata);
        }

        public void destroy(SharedData sdata)
        {
            routerCondition.OnObjectDestroy(sdata);
        }

        public bool excute(SharedData sdata)
        {
            return routerCondition.Justify(sdata);
        }
    }
}

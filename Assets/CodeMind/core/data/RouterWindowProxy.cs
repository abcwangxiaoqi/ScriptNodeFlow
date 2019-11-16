using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMind
{
    public interface IWindowsAsset
    {
        void AssetCreate();
        void AssetDelete();
    }
    
    [Serializable]
    public class RouterWindowProxy : WindowDataBase,IWindowsAsset
    {
        public RouterWindowProxy()
        {
            name = "Router Name";
        }

        public List<RouterWindowConditionProxy> conditions = new List<RouterWindowConditionProxy>();

        public List<string> preConditionTypes { get; private set; }
        public sealed override NodeType type
        {
            get
            {
                return NodeType.Router;
            }
        }

        public void AssetCreate()
        {
            preConditionTypes = new List<string>();

            OnAssetCreate();
        }

        protected virtual void OnAssetCreate()
        {
        }

        public void AssetDelete()
        {
            OnAssetDelete();

            //destroy all conditions
            foreach (var item in conditions)
            {
                item.AssetDelete();
            }

            conditions.Clear();
        }

        protected virtual void OnAssetDelete()
        { }

        /// <summary>
        /// Adds the condition.
        /// </summary>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        protected void AddPreCondition<T>()
            where T:RouterWindowConditionProxy
        {
            var tyName = typeof(T).FullName;
            preConditionTypes.Add(tyName);
        }

        #region runtime

        [NonSerialized] public string runtimeNextId = null;

        public override void OnCreate(SharedData sharedData)
        {
            base.OnCreate(sharedData);

            foreach (var item in conditions)
            {
                item.awake(sharedData);
            }
        }

        public override void OnObjectDestroy(SharedData sharedData)
        {
            base.OnObjectDestroy(sharedData);

            foreach (var item in conditions)
            {
                item.destroy(sharedData);
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



    [Serializable] //这个类不能 是 抽象类 否则 在编译后序列化会有问题
    public class RouterWindowConditionProxy:IWindowsAsset
    {
        public string ID = Guid.NewGuid().ToString();
        
        public string name = "Condition Name";

        public RouterCondition routerCondition;

        public string nextWindowId = null;

        public void AssetCreate()
        {
            OnAssetCreate();
        }

        protected virtual void OnAssetCreate()
        {
        }

        public void AssetDelete()
        {
            OnAssetDelete();

            if (routerCondition == null)
                return;

            UnityEngine.Object.DestroyImmediate(routerCondition,true);
        }

        protected virtual void OnAssetDelete()
        { }

        /*-----runtime---------*/

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

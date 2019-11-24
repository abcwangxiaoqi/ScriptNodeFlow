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

        protected override void OnAwake()
        {

            foreach (var item in conditions)
            {
                item.Init(m_SharedData);
                item.OnAwake();
            }
        }

        protected override void OnStart()
        {
            foreach (var item in conditions)
            {
                item.OnStart();
            }
        }

        protected override void OnDestroy()
        {
            foreach (var item in conditions)
            {
                item.OnDestroy();
            }
        }

        protected override void OnEnter(CodeMindController mindController)
        {
            try
            {
                runtimeState = RuntimeState.Running;
                bool condFlag = false;
                for (int i = 0; i < conditions.Count; i++)
                {
                    if (conditions[i].Justify())
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

        protected override void OnExist()
        {
            
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

        protected SharedData m_SharedData { get; private set; }

        internal void Init(SharedData data)
        {
            m_SharedData = data;

            routerCondition.Init(m_SharedData);
        }

        public void OnAwake()
        {
            routerCondition.Awake();
        }

        public void OnStart()
        {
            routerCondition.Start();
        }

        public void OnDestroy()
        {
            routerCondition.Destroy();
        }

        public bool Justify()
        {
            return routerCondition.Justify();
        }
    }
}

using UnityEngine;

namespace EditorCodeMind
{
    public abstract class RouterCondition : ScriptableObject
    {
        protected T GetSharedDate<T>()
            where T:SharedData
        {
            return m_SharedData as T;
        }
        
        protected SharedData m_SharedData { get; private set; }
        internal void Init(SharedData data)
        {
            m_SharedData = data;
        }

        internal void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        {
            
        }

        internal void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        {
            
        }

        public abstract bool Justify();

        internal void Destroy()
        {
            OnDestroy();
        }

        protected virtual void OnDestroy()
        {

        }
    }
}
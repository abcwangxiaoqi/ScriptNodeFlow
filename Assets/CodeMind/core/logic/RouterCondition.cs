using UnityEngine;

namespace CodeMind
{
    public abstract class RouterCondition : ScriptableObject
    {
        public string des = "Condition Name";
        public string nextWindowId = null;
        
        internal void OnCreate(SharedData shareData)
        {
            OnConditionCreate(shareData);
        }

        protected virtual void OnConditionCreate(SharedData shareData)
        {

        }
        
        public abstract bool Justify(SharedData shareData);

        internal void OnObjectDestroy(SharedData shareData)
        {
            OnConditionDestroy(shareData);
        }

        protected virtual void OnConditionDestroy(SharedData shareData)
        {

        }
    }
}
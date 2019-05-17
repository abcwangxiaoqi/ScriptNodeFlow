using UnityEngine;

namespace CodeMind
{
    public abstract class RouterCondition : ScriptableObject
    {
        protected SharedData shareData;
        public RouterCondition(SharedData data)
        {
            shareData = data;
        }

        public abstract bool justify();
    }
}
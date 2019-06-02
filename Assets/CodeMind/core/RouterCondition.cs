using UnityEngine;

namespace CodeMind
{
    public abstract class RouterCondition : ScriptableObject
    {
        public virtual void OnCreate(SharedData shareData)
        {
            
        }

        public virtual void OnDelete(SharedData shareData)
        {
            
        }
        
        public abstract bool Justify(SharedData shareData);
    }
}
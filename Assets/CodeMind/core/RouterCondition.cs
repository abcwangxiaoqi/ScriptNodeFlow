using UnityEngine;

namespace CodeMind
{
    public abstract class RouterCondition : ScriptableObject
    {
        public virtual void OnCreate(SharedData shareData)
        {
            
        }
        
        public abstract bool Justify(SharedData shareData);

        public virtual void OnObjectDestroy(SharedData shareData)
        {

        }
    }
}
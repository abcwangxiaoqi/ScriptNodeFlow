using UnityEngine;

namespace CodeMind
{
    public abstract class RouterCondition : ScriptableObject
    {
        public abstract bool justify(SharedData shareData);
    }
}
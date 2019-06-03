using UnityEngine;

namespace CodeMind
{
    public abstract class SharedData : ScriptableObject
    {
        public virtual void OnCreate()
        { }

        public virtual void OnObjectDestroy()
        { }
    }
}

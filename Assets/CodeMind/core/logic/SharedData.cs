using UnityEngine;

namespace CodeMind
{
    public abstract class SharedData : ScriptableObject
    {
        internal void OnCreate()
        {
            OnDataCreate();
        }

        protected virtual void OnDataCreate()
        { }

        internal void OnObjectDestroy()
        {
            OnDataDestroy();
        }

        protected virtual void OnDataDestroy()
        {

        }

        [SerializeField]
        string dd;
    }
}

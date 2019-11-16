using UnityEngine;

namespace CodeMind
{
    public abstract class SharedData : ScriptableObject,IWindowsAsset
    {
        public void AssetCreate()
        {
            OnAssetCreate();
        }

        protected virtual void OnAssetCreate(){}

        public void AssetDelete()
        {
            OnAssetDelete();
        }

        protected virtual void OnAssetDelete() { }

        /*runtime method*/

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
    }
}

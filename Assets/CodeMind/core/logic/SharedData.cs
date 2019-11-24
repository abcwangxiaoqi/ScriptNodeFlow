using UnityEngine;

namespace EditorCodeMind
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

        public void Awake()
        {
            OnAwake();
        }

        protected virtual void OnAwake()
        { }

        public void Destroy()
        {
            OnDestroy();
        }

        protected virtual void OnDestroy()
        {

        }

        public void Start()
        {
            OnStart();
        }

        protected virtual void OnStart()
        { }

        public void Update()
        {
            OnUpdate();
        }

        protected virtual void OnUpdate()
        { }

        public void LateUpdate()
        {
            OnLateUpdate();
        }

        protected virtual void OnLateUpdate()
        { }
    }
}


using System;
using UnityEngine;

namespace CodeMind
{
    
    public abstract class Node : ScriptableObject,IWindow
    {
        [HideInInspector]
        [SerializeField]
        string _ID = Guid.NewGuid().ToString();
        public string ID
        {
            get
            {
                return _ID;
            }
        }


        [HideInInspector]
        [SerializeField]
        Vector2 _winPos;

        public Vector2 winPos
        {
            get{
                return _winPos;
            }
            internal set
            {
                _winPos = value;
            }
        }

        public NodeType nodeType 
        {
            get
            {
                return NodeType.Node;
            }
        }


        [HideInInspector]
        [SerializeField]
        string _winName;

        public string winName
        {
            get
            {
                return _winName;
            }
            internal set
            {
                _winName = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        string _winDes;

        public string winDes
        {
            get
            {
                return _winDes;
            }
            internal set
            {
                _winDes = value;
            }
        }

        [HideInInspector]
        [SerializeField]
        string _nextWindowId = null;
        public string nextWindowId
        {
            get
            {
                return _nextWindowId;
            }
            internal set
            {
                _nextWindowId = value;
            }
        }

        //资源创建的时候
        public virtual void OnCreateAsset()
        {
        }

        //资源删除的时候
        public virtual void OnDeleteAsset()
        {
        }

        /*------runtime--------*/

        
        public bool isFinished { get; private set; }

        public string errorMessage { get; private set; }

        public void Init(SharedData sharedData)
        {
            OnInit(sharedData);
        }

        protected virtual void OnInit(SharedData sharedData)
        {

        }

        public void Play(SharedData sharedData)
        {
            isFinished = false;
            errorMessage = string.Empty;


            OnPlay(sharedData);
        }

        protected virtual void OnPlay(SharedData sharedData)
        {

        }

        public void ProcessUpdate(SharedData sharedData)
        {
            OnProcessUpdate(sharedData);
        }

        protected virtual void OnProcessUpdate(SharedData sharedData)
        {

        }


        //you must call this when you're sure the execute method is finished completely,
        //then the current node move to the next one
        //
        //why be designed like this? 
        //cause maybe your execute method includes some asyn operations
        protected void moveNext(string error = null)
        {
            isFinished = true;
            errorMessage = error;
        }

        public void Destroy(SharedData sharedData)
        {
            OnDestroy(sharedData);
        }

        protected virtual void OnDestroy(SharedData sharedData)
        {
            
        }

        public void Reset(SharedData sharedData)
        {
            OnReset(sharedData);
        }


        protected virtual void OnReset(SharedData sharedData)
        {}


    }
}
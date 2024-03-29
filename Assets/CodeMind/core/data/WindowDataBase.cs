﻿using System;
using UnityEngine;

namespace CodeMind
{
    public enum RuntimeState
    {
        Idle=0,//idle,can be called
        Running=1,// was called but not finish completely 
        Error=2,//something wrong
        Finished=3//finish completely,and move the next one
    }

    public enum NodeType
    {
        Node,
        Router,
        Start,
        SubCanvas
    }

    public abstract class WindowDataBase
    {
        public string ID = DateTime.Now.ToString("yyMMddHHmmssff");
        public string name;
        public Vector2 position;
        public string nextWindowId = null;

        public string desc;

        public virtual NodeType type
        {
            get
            {
                return NodeType.Node;
            }
        }

        #region runtime

        [NonSerialized]
        public RuntimeState runtimeState = RuntimeState.Idle;

        [NonSerialized]
        public string runtimeError;

        public virtual void play(params object[] objs) { }

        public virtual void update() { }

        public virtual void stop()
        { }

        public virtual void reset()
        {
            runtimeState = RuntimeState.Idle;
        }

        #endregion

    }
}

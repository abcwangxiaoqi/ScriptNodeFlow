using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class StartWindow : BaseWindow
    {
        protected static GUIContent nextNewNodeContent = new GUIContent("Next/New Node");
        protected static GUIContent nextNewSubCanvasContent = new GUIContent("Next/New SubCanvas");
        protected static string separator = "Next/";

        //下一节点
        public BaseWindow next { get; protected set; }

        Vector2 _size = new Vector2(150, 60);
        protected override Vector2 size
        {
            get
            {
                return _size;
            }
        }


        public override NodeType windowType
        {
            get
            {
                return NodeType.Start;
            }
        }

        public StartWindow(StartWindowData itemData, List<BaseWindow> _windowList)
            : base(itemData, _windowList)
        {
        }

        public void SetNext(BaseWindow entity)
        {
            if (entity == null && next != null)
            {
                next.SetParent(null);
            }

            next = entity;

            if (entity != null)
            {
                entity.SetParent(this);
            }
        }

        public override WindowDataBase GetData()
        {
            StartWindowData dataEntity = new StartWindowData();
            dataEntity.position = position;

            if (next != null)
            {
                dataEntity.nextWindowId = next.Id;
            }

            return dataEntity;
        }

        private bool connectFlag = false;
        Event curEvent;
        protected override void drawBefore()
        {
            base.drawBefore();

            curEvent = Event.current;

            if (connectFlag && curEvent.button == 1)
            {
                // mouse right key
                DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectAnotherPort);
                connectFlag = false;
            }
        }

        protected override void drawAfter()
        {
            base.drawAfter();

            if (next != null)
            {
                if (!windowList.Contains(next))
                {
                    next = null;
                    return;
                }

                Color color = Color.white;

                if (Application.isPlaying && windowData.runtimeState == RuntimeState.Finished)
                {
                    color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
                }

                DrawArrow(GetOutPositionByPort(OutPortRect), next.In, color);
            }

            if (GUI.Button(OutPortRect, "", (connectFlag || next != null) ? Styles.connectedBtn : Styles.connectBtn))
            {

                SetNext(null);
                DelegateManager.Instance.AddListener(DelegateCommand.HANDLECONNECTPORT, connectAnotherPort);
                connectFlag = true;
            }

            if (connectFlag)
            {
                DrawArrow(GetOutPositionByPort(OutPortRect), curEvent.mousePosition, Color.white);
            }
        }

        void connectAnotherPort(object[] objs)
        {
            DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectAnotherPort);

            BaseWindow window = objs[0] as BaseWindow;

            if (window != null
                && window.Id != Id 
                && window.windowType != NodeType.Router
                && window.windowType != NodeType.Start)
            {
                SetNext(window);
            }

            connectFlag = false;
        }

        protected override void drawWindowContent()
        {
            GUILayout.Label("Start", Styles.titleLabel);
        }
    }
}

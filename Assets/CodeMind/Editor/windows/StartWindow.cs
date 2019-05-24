using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeMind
{
    public class StartWindow : BaseWindow
    {
        //下一节点
        public BaseWindow next { get; protected set; }

        protected override float windowWidth
        {
            get
            {
                return 150f;
            }
        }


        public override NodeType windowType
        {
            get
            {
                return NodeType.Start;
            }
        }

        StartWindowData startData;
        public StartWindow(StartWindowData itemData, BaseCanvas canvas)
            : base(itemData, canvas)
        {
            startData = itemData;
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

                startData.nextWindowId = entity.Id;
            }
            else
            {
                startData.nextWindowId = null;
            }
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

                Color color = CanvasLayout.Layout.canvas.lineColor;

                if (Application.isPlaying && windowData.runtimeState == RuntimeState.Finished)
                {
                    color = CanvasLayout.Layout.canvas.runtimelineColor;
                }

                DrawArrow(GetOutPositionByPort(OutPortRect), next.In, color);
            }
            
            if (GUI.Button(OutPortRect, "", (connectFlag || next != null) ? CanvasLayout.Layout.canvas.ConnectedBtStyle : CanvasLayout.Layout.canvas.ConnectBtStyle))
            {
                if(!Application.isPlaying)
                {
                    SetNext(null);
                    DelegateManager.Instance.AddListener(DelegateCommand.HANDLECONNECTPORT, connectAnotherPort);
                    connectFlag = true;
                }
            }

            if (connectFlag)
            {
                DrawArrow(GetOutPositionByPort(OutPortRect), curEvent.mousePosition, CanvasLayout.Layout.canvas.lineColor);
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
            GUILayout.Label(CanvasLayout.Layout.canvas.StartContent, CanvasLayout.Layout.canvas.StartLabelStyle);
        }
    }
}

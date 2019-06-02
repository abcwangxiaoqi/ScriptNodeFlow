using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace CodeMind
{
    public class SubCanvasWindow : BaseWindow
    {
        static SubCanvasWindow()
        {
        }

        //下一节点
        public BaseWindow next { get; protected set; }

        protected override float windowWidth
        {
            get
            {
                return 200f;
            }
        }


        public override NodeType windowType
        {
            get
            {
                return NodeType.SubCodeMind;
            }
        }
        
        CodeMindWindowData codeMindWindowData;
        public SubCanvasWindow(CodeMindWindowData itemData, BaseCanvas canvas)
            : base(itemData, canvas)
        {
            codeMindWindowData = itemData;
        }

        public void SetNext(BaseWindow entity)
        {
            if (next != null)
            {
                next.SetParent(null);
            }

            next = entity;

            if (entity != null)
            {
                entity.SetParent(entity);
                codeMindWindowData.nextWindowId = entity.Id;
            }            
            else
            {
                codeMindWindowData.nextWindowId = null;
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

            GUI.Button(InPortRect, "", parentRef == 0 ? CanvasLayout.Layout.canvas.ConnectBtStyle : CanvasLayout.Layout.canvas.ConnectedBtStyle);

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

            //draw line
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
        }

        void connectAnotherPort(object[] objs)
        {
            DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectAnotherPort);

            BaseWindow window = objs[0] as BaseWindow;

            if (window != null && window.Id != Id)
            {
                SetNext(window);
            }

            connectFlag = false;
        }
        
        private SubCanvasData tempCanvas;
        protected override void drawWindowContent()
        {
            base.drawWindowContent();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            codeMindWindowData.canvasData = (CodeMindData)EditorGUILayout.ObjectField(codeMindWindowData.canvasData, typeof(CodeMindData), false);

            EditorGUI.EndDisabledGroup();

            if(codeMindWindowData.canvasData == null)
            {
                EditorGUILayout.LabelField(CanvasLayout.Layout.canvas.SubCanvasErrorContent, CanvasLayout.Layout.canvas.SubCanvasErrorStyle);
            }
        }

        public override void deleteWindow()
        {
            if (next != null)
            {
                next.SetParent(null);
            }
        }

        public override void leftMouseDoubleClick()
        {
            if (codeMindWindowData.canvasData == null)
                return;


            if(Application.isPlaying)
            {
                RuntimeCanvas.Open(codeMindWindowData.canvasData);
            }
            else
            {
                EditorCanvas.Open(codeMindWindowData.canvasData);
            }

            //DelegateManager.Instance.Dispatch(DelegateCommand.OPENSUBCANVAS,
            //    canvas);
            GUIUtility.ExitGUI();
        }
    }
}

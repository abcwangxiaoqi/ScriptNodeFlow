using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace ScriptNodeFlow
{
    public class SubCanvasWindow : BaseWindow
    {
        static GUIContent doubleClickContent = new GUIContent("double click to open canvas");

        static GUIContent deleteContent = new GUIContent("Delte");

        static SubCanvasWindow()
        {
        }

        protected SubNodeCanvasData canvas { get; private set; }

        //下一节点
        public BaseWindow next { get; protected set; }

        Vector2 _size = new Vector2(150, 80);
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
                return NodeType.SubCanvas;
            }
        }
        
        public SubCanvasWindow(string orgin, Vector2 pos, List<BaseWindow> _windowList, int _flowID)
            : base(pos, _windowList, _flowID)
        {
            Orgin = orgin;
            Name = "Canvas";
        }

        protected string Orgin;

        public SubCanvasWindow(string orgin,CanvasWindowData itemData, List<BaseWindow> _windowList, int _flowID)
            : base(itemData, _windowList, _flowID)
        {
            Orgin = orgin;
            canvas = itemData.canvasData;
        }

        public void SetNext(BaseWindow entity)
        {
            if (next != null)
            {
                next.SetParent(null);
            }

            if (entity != null)
            {
                entity.SetParent(entity);
            }

            next = entity;
        }

        public override WindowDataBase GetData()
        {
            CanvasWindowData dataEntity = new CanvasWindowData();
            dataEntity.position = position;
            dataEntity.name = Name;
            dataEntity.ID = Id;
            dataEntity.desc = describe;

            dataEntity.canvasData = canvas;

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

            GUI.Button(InPortRect, "", parentRef == 0 ? Styles.connectBtn : Styles.connectedBtn);

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

            //draw line
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

        private Object obj;
        private SubNodeCanvasData tempCanvas;
        protected override void drawWindowContent()
        {
            base.drawWindowContent();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            tempCanvas = (SubNodeCanvasData)EditorGUILayout.ObjectField(canvas, typeof(SubNodeCanvasData), false);

            if(tempCanvas!=null)
            {
                //Debug.Log(">>>" + tempCanvas.GetInstanceID());
            }


            if (tempCanvas!=null && AssetDatabase.GetAssetPath(tempCanvas) == Orgin)
            {
                canvas = tempCanvas;
            }
            else if(tempCanvas == null)
            {
                canvas = null;
            }
            else
            {
                tempCanvas = canvas;
            }

            EditorGUI.EndDisabledGroup();

            /*GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();
            if (canvas == null)
            {
                GUILayout.Label("", Styles.wrongLabel);
                GUILayout.Label("Canvas asset is null", Styles.tipErrorLabel);
            }
            else
            {
                GUILayout.Label("", Styles.rightLabel);
                GUILayout.Label("Everything is right", Styles.tipLabel);
            }

            GUILayout.EndHorizontal();*/
        }


        public override void rightMouseClick(Vector2 mouseposition)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(deleteContent, false, () =>
            {
                if (next != null)
                {
                    next.SetParent(null);
                }
                windowList.Remove(this);
            });

            menu.ShowAsContext();
        }

        public override void leftMouseDoubleClick()
        {
            if(canvas==null)
                return;

            
            DelegateManager.Instance.Dispatch(DelegateCommand.OPENSUBCANVAS,
                canvas);
            GUIUtility.ExitGUI();
        }
    }
}

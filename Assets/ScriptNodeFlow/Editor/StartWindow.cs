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

        public StartWindow(string orgin, StartWindowData itemData, List<BaseWindow> _windowList)
            : base(orgin,itemData, _windowList)
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

        public override void draw()
        {
            base.draw();

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

            #region draw connect port

            if (GUI.Button(OutPortRect, "", (connectFlag || next != null) ? Styles.connectedBtn : Styles.connectBtn))
            {

                SetNext(null);
               
                connectFlag = true;
            }

            if (connectFlag)
            {
                Event curEvent = Event.current;
                DrawArrow(GetOutPositionByPort(OutPortRect), curEvent.mousePosition, Color.white);


                if (curEvent.button == 1) // mouse right key
                {
                    connectFlag = false;
                }
                else if (curEvent.button == 0 && curEvent.isMouse)
                {
                    if (curEvent.type == EventType.MouseUp)
                    {
                        BaseWindow win = windowList.Find(window => { return window.isClick(curEvent.mousePosition); });

                        if (win != null
                            && win.Id != Id
                            && win.windowType != NodeType.Start)
                        {
                            SetNext(win);
                        }

                        connectFlag = false;
                    }
                }
            }

            #endregion
        }

        protected override void gui(int id)
        {
            GUILayout.Label("Start", Styles.titleLabel);
        }
    }
}

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
        static List<string> allEntityClass = new List<string>();

        static GUIContent doubleClickContent = new GUIContent("double click to open canvas");

        static GUIContent nextNewNodeContent = new GUIContent("Next/New Node");
        static GUIContent nextNewRouterContent = new GUIContent("Next/New Router");
        static GUIContent nextNewSubCanvasContent = new GUIContent("Next/New SubCanvas");
        static GUIContent deleteContent = new GUIContent("Delte");
        static string separator = "Next/";

        static SubCanvasWindow()
        {
            Assembly _assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp.dll");
            Type[] tys = _assembly.GetTypes();

            foreach (var item in tys)
            {
                if (item.IsSubclassOf(typeof(Node)) && !item.IsInterface && !item.IsAbstract)
                {
                    allEntityClass.Add(item.FullName);
                }
            }
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
        
        public SubCanvasWindow(string orgin, Vector2 pos, List<BaseWindow> _windowList)
            : base(orgin,pos, _windowList)
        {
            Name = "Canvas";
        }

        public SubCanvasWindow(string orgin,CanvasWindowData itemData, List<BaseWindow> _windowList)
            : base(orgin,itemData, _windowList)
        {
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

            dataEntity.canvasData = canvas;

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

            GUI.Button(InPortRect, "", parent == null ? Styles.connectBtn : Styles.connectedBtn);

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

        private Object obj;
        private SubNodeCanvasData tempCanvas;
        protected override void gui(int id)
        {
            base.gui(id);

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            tempCanvas = (SubNodeCanvasData)EditorGUILayout.ObjectField(canvas, typeof(SubNodeCanvasData), false);

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

            GUILayout.FlexibleSpace();

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

            GUILayout.EndHorizontal();
        }

        GenericMenu menu;

        public override void rightMouseClick(Vector2 mouseposition)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(deleteContent, false, () =>
            {
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

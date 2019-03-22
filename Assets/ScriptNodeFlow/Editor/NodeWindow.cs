using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class NodeWindow : BaseWindow
    {
        protected static List<string> allEntityClass = new List<string>();

        protected GUIContent nextNewNodeContent = new GUIContent("Next/New Node");
        protected GUIContent nextNewRouterContent = new GUIContent("Next/New Router");
        protected static GUIContent nextNewSubCanvasContent = new GUIContent("Next/New SubCanvas");
        protected GUIContent deleteContent = new GUIContent("Delte");
        protected string separator = "Next/";

        static NodeWindow()
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

        protected string ClassName { get; private set; }

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
                return NodeType.Node;
            }
        }

        public NodeWindow(string orgin,Vector2 pos, List<BaseWindow> _windowList)
            : base(orgin,pos, _windowList)
        {
            Name = "Node";
        }

        public NodeWindow(string orgin, NodeWindowData itemData, List<BaseWindow> _windowList)
            : base(orgin,itemData, _windowList)
        {
            ClassName = itemData.className;

            classIndex = allEntityClass.IndexOf(ClassName);
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
            NodeWindowData dataEntity = new NodeWindowData();
            dataEntity.position = position;
            dataEntity.name = Name;
            dataEntity.ID = Id;

            dataEntity.className = ClassName;

            if (next != null)
            {
                dataEntity.nextWindowId = next.Id;
            }

            return dataEntity;
        }

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

        private bool connectFlag = false;

        int classIndex = -1;

        protected override void gui(int id)
        {
            base.gui(id);


            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            classIndex = EditorGUILayout.Popup(classIndex, allEntityClass.ToArray(), popupStyle);
            EditorGUI.EndDisabledGroup();

            if (classIndex >= 0)
            {
                ClassName = allEntityClass[classIndex];
            }
            
            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            if (classIndex < 0)
            {
                GUILayout.Label("", Styles.wrongLabel);
                GUILayout.Label("Selection is null", Styles.tipErrorLabel);
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
    }
}

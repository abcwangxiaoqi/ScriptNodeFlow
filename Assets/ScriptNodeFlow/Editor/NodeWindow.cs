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

                DrawArrow(Out, next.In, color);
            }
        }

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

            GUI.DragWindow();
        }

        GenericMenu menu;

        public override void rightMouseClick(Vector2 mouseposition)
        {
            GenericMenu menu = new GenericMenu();


            menu.AddItem(nextNewNodeContent, false, () =>
            {
                var tempWindow = new NodeWindow(Orgin, position, windowList);
                windowList.Add(tempWindow);
                next = tempWindow;
            });

            menu.AddItem(nextNewRouterContent, false, () =>
            {
                var tempWindow = new RouterWindow(Orgin, position, windowList);
                windowList.Add(tempWindow);
                next = tempWindow;
            });

            menu.AddItem(nextNewSubCanvasContent, false, () =>
            {
                var tempWindow = new SubCanvasWindow(Orgin, position, windowList);
                windowList.Add(tempWindow);
                next = tempWindow;
            });

            menu.AddSeparator(separator);

            #region select the next one
            List<BaseWindow> selectionList = new List<BaseWindow>();

            foreach (var item in windowList)
            {
                if (item.Id == Id)
                    continue;

                if (item.windowType == NodeType.Start)
                    continue;

                selectionList.Add(item);
            }

            foreach (var item in selectionList)
            {
                bool select = (next != null) && next.Id == item.Id;

                menu.AddItem(new GUIContent(string.Format("Next/[{0}][{1}] {2}", item.Id, item.windowType, item.Name))
                             , select, () =>
                             {
                                 if (select)
                                 {
                                     next = null;
                                 }
                                 else
                                 {
                                     next = item;
                                 }
                             });
            }
            #endregion


            menu.AddItem(deleteContent, false, () =>
            {
                windowList.Remove(this);
            });


            menu.ShowAsContext();
        }
    }
}

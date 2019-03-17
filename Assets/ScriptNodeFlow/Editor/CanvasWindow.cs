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

            GUI.DragWindow();
        }

        GenericMenu menu;

        public override void rightMouseClick(Vector2 mouseposition)
        {
            GenericMenu menu = new GenericMenu();


            menu.AddItem(nextNewNodeContent, false, () =>
            {
                var tempWindow = new NodeWindow(Orgin, mouseposition + new Vector2(50, 50), windowList);
                windowList.Add(tempWindow);
                next = tempWindow;
            });

            menu.AddItem(nextNewRouterContent, false, () =>
            {
                var tempWindow = new RouterWindow(Orgin, mouseposition + new Vector2(50, 50), windowList);
                windowList.Add(tempWindow);
                next = tempWindow;
            });

            menu.AddItem(nextNewSubCanvasContent, false, () =>
            {
                var tempWindow = new SubCanvasWindow(Orgin, mouseposition + new Vector2(50, 50), windowList);
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

        public override void leftMouseDoubleClick()
        {
            if(canvas==null)
                return;

            DelegateManager.Instance.Dispatch(DelegateCommand.OPENSUBCANVAS,
                canvas);
        }
    }
}

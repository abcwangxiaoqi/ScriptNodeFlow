using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace ScriptNodeFlow
{
    public class CanvasWindow : BaseWindow
    {
        static List<string> allEntityClass = new List<string>();

        static GUIContent nextNewNodeContent = new GUIContent("Next/New Node");
        static GUIContent nextNewRouterContent = new GUIContent("Next/New Router");
        static GUIContent deleteContent = new GUIContent("Delte");
        static string separator = "Next/";

        static CanvasWindow()
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

        protected NodeCanvasData canvas { get; private set; }

        //下一节点
        public BaseWindow next { get; protected set; }

        Vector2 _size = new Vector2(150, 100);
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
                return NodeType.Canvas;
            }
        }

        public CanvasWindow(Vector2 pos, List<BaseWindow> _windowList)
            : base(pos, _windowList)
        {
            Name = "Canvas";
        }

        public CanvasWindow(CanvasWindowData itemData, List<BaseWindow> _windowList)
            : base(itemData, _windowList)
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

                if (Application.isPlaying && passed)
                {
                    color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
                }

                DrawArrow(Out, next.In, color);
            }
        }

        private Object obj;
        
        protected override void gui(int id)
        {
            base.gui(id);
            
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
           canvas = (NodeCanvasData)EditorGUILayout.ObjectField(canvas, typeof(NodeCanvasData),false);
            EditorGUI.EndDisabledGroup();

            GUI.DragWindow();
        }

        GenericMenu menu;

        public override void rightMouseDraw(Vector2 mouseposition)
        {
            GenericMenu menu = new GenericMenu();


            menu.AddItem(nextNewNodeContent, false, () =>
            {
                var tempWindow = new NodeWindow(mouseposition, windowList);
                windowList.Add(tempWindow);
                next = tempWindow;
            });

            menu.AddItem(nextNewRouterContent, false, () =>
            {
                var tempWindow = new RouterWindow(mouseposition, windowList);
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

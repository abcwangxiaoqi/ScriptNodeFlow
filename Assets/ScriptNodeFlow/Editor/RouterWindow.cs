using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace ScriptNodeFlow
{
    public class RouterWindowCondition
    {
        public string className;
        public BaseWindow nextWindow;
        public Vector2 drawPos;
    }

    public class RouterWindow : BaseWindow
    {
        protected static List<string> allConditionClass = new List<string>();
        protected static GUIStyle linkStyle;

        protected static GUIContent newNode = new GUIContent("New Node");
        protected static GUIContent newSubCanvas = new GUIContent("New SubCanvas");
        protected static GUIContent deleteNode = new GUIContent("Delte");

        protected static GUIStyle defaultLButton = new GUIStyle(EditorStyles.miniButton);
        protected static GUIStyle defaultLabel = new GUIStyle(EditorStyles.label);
        static RouterWindow()
        {
            defaultLButton.fixedWidth = 17;

            defaultLabel.alignment = TextAnchor.MiddleRight;
            defaultLabel.normal.textColor = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
            defaultLabel.fontStyle = FontStyle.Bold;

            linkStyle = new GUIStyle(UnityEditor.EditorStyles.boldLabel);
            linkStyle.fixedHeight = 10;
            linkStyle.fontSize = 8;
            linkStyle.alignment = TextAnchor.MiddleCenter;
            linkStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
            linkStyle.fixedWidth = 10;

            Assembly _assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp.dll");
            Type[] tys = _assembly.GetTypes();

            foreach (var item in tys)
            {
                if (item.IsSubclassOf(typeof(RouterCondition)) && !item.IsInterface && !item.IsAbstract)
                {
                    allConditionClass.Add(item.FullName);
                }
            }
        }

        protected List<RouterWindowCondition> conditions = new List<RouterWindowCondition>();

        public override NodeType windowType
        {
            get
            {
                return NodeType.Router;
            }
        }

        protected Vector2 _size = new Vector2(150, 100);
        protected override Vector2 size
        {
            get
            {
                return _size;
            }
        }


        protected BaseWindow defaultNextWindow = null;
        protected Vector2 defaultPos;

        public RouterWindow(string orgin, Vector2 pos, List<BaseWindow> _windowList)
            : base(orgin,pos, _windowList)
        {
            Name = "Router";

            addHeight = buttonStyle.lineHeight + 8;

        }

        public RouterWindow(string orgin, RouterWindowData itemData, List<BaseWindow> _windowList)
            : base(orgin,itemData, _windowList)
        {
            addHeight = buttonStyle.lineHeight + 8;
        }

        public void SetDefault(BaseWindow defEntity)
        {
            defaultNextWindow = defEntity;
        }

        public void SetConditions(List<RouterWindowCondition> conditionEntities)
        {
            conditions = conditionEntities;
            _size.y += addHeight * conditionEntities.Count;
        }

        public override WindowDataBase GetData()
        {
            RouterWindowData data = new RouterWindowData();
            data.ID = Id;
            data.name = Name;
            data.position = position;

            foreach (var item in conditions)
            {
                RouterWindowConditionData cond = new RouterWindowConditionData();

                cond.className = item.className;

                if (item.nextWindow != null)
                {
                    cond.nextWindowId = item.nextWindow.Id;
                }

                data.conditions.Add(cond);
            }

            if (defaultNextWindow != null)
            {
                data.nextWindowId = defaultNextWindow.Id;
            }

            return data;
        }

        protected Color color;

        public override void draw()
        {
            base.draw();

            //画线

            #region condition list
            for (int i = 0; i < conditions.Count; i++)
            {
                RouterWindowCondition item = conditions[i];

                if (item.nextWindow == null)
                    continue;

                if (!windowList.Contains(item.nextWindow))
                {
                    item.nextWindow = null;
                    continue;
                }

                if (item.drawPos == Vector2.zero)
                    continue;

                color = Color.white;

                if (Application.isPlaying 
                    && windowData.runtimeState == RuntimeState.Finished 
                    && (windowData as RouterWindowData).runtimeNextId == item.nextWindow.Id)
                {
                    color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
                }

                DrawArrow(item.drawPos + position, item.nextWindow.In, color);
            }
            #endregion

            #region default
            if (defaultNextWindow == null)
                return;

            if (!windowList.Contains(defaultNextWindow))
            {
                defaultNextWindow = null;
                return;
            }
            if (defaultPos == Vector2.zero)
                return;

            color = Color.white;

            if (Application.isPlaying 
                && windowData.runtimeState == RuntimeState.Finished 
                && (windowData as RouterWindowData).runtimeNextId == defaultNextWindow.Id)
            {
                color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
            }

            DrawArrow(defaultPos + position, defaultNextWindow.In, color);
            #endregion
        }


        protected float addHeight;
        protected Rect rect;
        protected override void gui(int id)
        {
            base.gui(id);

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            GUI.color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
            if (GUILayout.Button("Add", buttonStyle))
            {
                conditions.Add(new RouterWindowCondition());

                _size.y += addHeight;
            }
            GUI.color = Color.white;

            GUILayout.Space(10);

            drawConditions();

            GUILayout.Space(10);

            drawDefualt();

            EditorGUI.EndDisabledGroup();

            GUI.DragWindow();
        }

        protected virtual void drawConditions()
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                RouterWindowCondition rc = conditions[i];
                GUILayout.BeginHorizontal();

                string c = rc.className;
                int selectindex = allConditionClass.IndexOf(c);
                selectindex = EditorGUILayout.Popup(selectindex, allConditionClass.ToArray(), popupStyle);
                if (selectindex >= 0)
                {
                    conditions[i].className = allConditionClass[selectindex];
                }

                //删除
                if (GUILayout.Button("", Styles.miniDelButton))
                {
                    conditions.RemoveAt(i);
                    i--;
                    _size.y -= addHeight;
                }

                //连接选择
                GUI.color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
                if (MyEditorLayout.Button("L", buttonStyle, out rect))
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(newNode, false, () =>
                    {
                        var tempWindow = new NodeWindow(Orgin,new Vector2(50, 50) + position, windowList);
                        windowList.Add(tempWindow);
                        rc.nextWindow = tempWindow;
                    });

                    menu.AddItem(newSubCanvas, false, () =>
                    {
                        var tempWindow = new SubCanvasWindow(Orgin,new Vector2(50, 50) + position, windowList);
                        windowList.Add(tempWindow);
                        rc.nextWindow = tempWindow;
                    });

                    menu.AddSeparator("");

                    List<BaseWindow> selectionList = new List<BaseWindow>();
                    foreach (var item in windowList)
                    {
                        if (item.windowType == NodeType.Node || item.windowType == NodeType.SubCanvas)
                        {
                            selectionList.Add(item);
                        }
                    }

                    foreach (var item in selectionList)
                    {
                        bool select = (rc.nextWindow != null) && rc.nextWindow.Id == item.Id;
                        menu.AddItem(new GUIContent(string.Format("[{0}][{1}] {2}", item.Id, item.windowType, item.Name)), select, () =>
                        {
                            if (select)
                            {
                                rc.nextWindow = null;
                            }
                            else
                            {
                                rc.nextWindow = item;
                            }
                        });
                    }

                    menu.ShowAsContext();
                }


                GUI.color = Color.white;

                if (rc.nextWindow == null)
                {
                    linkStyle.normal.textColor = Color.gray;
                }
                else
                {
                    linkStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
                }

                MyEditorLayout.Label("o", linkStyle, out rect);

                //有的时候 rect会为0，0，1，1
                if (rect.position != Vector2.zero)
                {
                    rc.drawPos.x = rect.position.x + rect.width;
                    rc.drawPos.y = rect.position.y + rect.height / 2;
                }

                GUILayout.EndHorizontal();
            }
        }

        protected virtual void drawDefualt()
        {
            GUILayout.BeginHorizontal();
            
            GUILayout.Label("default", defaultLabel);

            //连接选择
            GUI.color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
            if (MyEditorLayout.Button("L", defaultLButton, out rect))
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(newNode, false, () =>
                {
                    var tempWindow = new NodeWindow(Orgin, position, windowList);
                    windowList.Add(tempWindow);
                    defaultNextWindow = tempWindow;
                });

                menu.AddItem(newSubCanvas, false, () =>
                {
                    var tempWindow = new RouterWindow(Orgin, position, windowList);
                    windowList.Add(tempWindow);
                    defaultNextWindow = tempWindow;
                });

                List<BaseWindow> selectionList = new List<BaseWindow>();
                foreach (var item in windowList)
                {
                    if (item.windowType == NodeType.Node || item.windowType == NodeType.SubCanvas)
                    {
                        selectionList.Add(item);
                    }
                }

                foreach (var item in selectionList)
                {
                    bool select = (defaultNextWindow != null) && defaultNextWindow.Id == item.Id;
                    menu.AddItem(new GUIContent(item.Id + " " + item.Name), select, () =>
                    {
                        if (select)
                        {
                            defaultNextWindow = null;
                        }
                        else
                        {
                            defaultNextWindow = item;
                        }
                    });
                }

                menu.ShowAsContext();
            }

            if (defaultNextWindow == null)
            {
                linkStyle.normal.textColor = Color.gray;
            }
            else
            {
                linkStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
            }

            MyEditorLayout.Label("o", linkStyle, out rect);

            //有的时候 rect会为0，0，1，1
            if (rect.position != Vector2.zero)
            {
                defaultPos.x = rect.position.x + rect.width;
                defaultPos.y = rect.position.y + rect.height / 2;
            }

            GUI.color = Color.white;

            GUILayout.EndHorizontal();
        }

        public override void rightMouseClick(Vector2 mouseposition)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(deleteNode, false, () =>
            {
                windowList.Remove(this);
            });

            menu.ShowAsContext();
        }
    }
}
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
        public Rect connectRect;
        public bool connectFlag = false;
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

        protected static GUIContent newCondition = new GUIContent("New", "add a new condition");
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

        protected Vector2 _size = new Vector2(180, 115);
        protected override Vector2 size
        {
            get
            {
                return _size;
            }
        }
        
        protected BaseWindow defaultNextWindow = null;
        protected Rect defaultConnectRect;
        protected bool defaultConnectFlag = false;

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
            if (defaultNextWindow != null)
            {
                defaultNextWindow.SetParent(null);
            }

            if (defEntity != null)
            {
                defEntity.SetParent(this);
            }

            defaultNextWindow = defEntity;
        }

        public void SetConditions(List<RouterWindowCondition> conditionEntities)
        {
            conditions = conditionEntities;
            _size.y += addHeight * conditionEntities.Count;

            foreach (var condition in conditions)
            {
                if (condition.nextWindow != null)
                {
                    if (condition.nextWindow is NodeWindow)
                    {
                        (condition.nextWindow as NodeWindow).SetParent(this);
                    }
                    else if (condition.nextWindow is SubCanvasWindow)
                    {
                        (condition.nextWindow as SubCanvasWindow).SetParent(this);
                    }
                }
            }
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

            #region draw line

            #region condition list
            foreach (var condition in conditions)
            {
                if (condition.nextWindow == null)
                    continue;

                if (!windowList.Contains(condition.nextWindow))
                {
                    condition.nextWindow = null;
                    continue;
                }

                color = Color.white;

                if (Application.isPlaying
                    && windowData.runtimeState == RuntimeState.Finished
                    && (windowData as RouterWindowData).runtimeNextId == condition.nextWindow.Id)
                {
                    color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
                }

                DrawArrow(GetOutPositionByPort(condition.connectRect), condition.nextWindow.In, color);
            }
            #endregion

            #region default

            if (defaultNextWindow != null)
            {
                if (!windowList.Contains(defaultNextWindow))
                {
                    defaultNextWindow = null;
                    return;
                }

                color = Color.white;

                if (Application.isPlaying
                    && windowData.runtimeState == RuntimeState.Finished
                    && (windowData as RouterWindowData).runtimeNextId == defaultNextWindow.Id)
                {
                    color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
                }

                DrawArrow(GetOutPositionByPort(defaultConnectRect), defaultNextWindow.In, color);
            }
           
            #endregion

            #endregion

            #region draw connect port

            GUI.Button(InPortRect, "", parentRef == 0 ? Styles.connectBtn : Styles.connectedBtn);

            drawConditionsConnect();

            drawDefaultConnect();

            #endregion
        }

        protected virtual void drawConditionsConnect()
        {
            foreach (var condition in conditions)
            {
                if (GUI.Button(condition.connectRect, "",
                    (condition.nextWindow != null || condition.connectFlag) ? Styles.connectedBtn : Styles.connectBtn))
                {
                    setConditionNext(condition, null);
                    DelegateManager.Instance.AddListener(DelegateCommand.HANDLECONNECTPORT, connectConditionAnotherPort);
                    condition.connectFlag = true;
                }

                if (condition.connectFlag)
                {
                    Event curEvent = Event.current;

                    DrawArrow(GetOutPositionByPort(condition.connectRect), curEvent.mousePosition, Color.white);


                    if (curEvent.button == 1) // mouse right key
                    {
                        DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectConditionAnotherPort);
                        condition.connectFlag = false;
                    }
                    else if (curEvent.button == 0 && curEvent.isMouse)
                    {
                        if (curEvent.type == EventType.MouseDown)
                        {
                            BaseWindow win = windowList.Find(window =>
                            {
                                return window.isClick(curEvent.mousePosition) || window.isClickInPort(curEvent.mousePosition);
                            });

                            if (win != null
                                && win.Id != Id
                                && win.windowType != NodeType.Start
                                && win.windowType != NodeType.Router)
                            {
                                setConditionNext(condition, win);
                            }

                            condition.connectFlag = false;
                        }
                    }
                }
            }
        }

        void connectConditionAnotherPort(object[] objs)
        {
            DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectConditionAnotherPort);

            BaseWindow window = objs[0] as BaseWindow;

            RouterWindowCondition condition = conditions.Find((cond)=> 
            {
                return cond.connectFlag;
            });

            if (window != null && window.Id != Id
                && window.windowType != NodeType.Router)
            {
                setConditionNext(condition, window);
            }

            condition.connectFlag = false;
        }


        void setConditionNext(RouterWindowCondition condition,BaseWindow next)
        {
            if (condition.nextWindow != null)
            {
                condition.nextWindow.SetParent(null);
            }

            if (next != null)
            {
                next.SetParent(this);
            }

            condition.nextWindow = next;
        }


        protected virtual void drawDefaultConnect()
        {
            if (GUI.Button(defaultConnectRect, "", 
                (defaultNextWindow!=null || defaultConnectFlag) ? Styles.connectedBtn : Styles.connectBtn))
            {
                SetDefault(null);
                DelegateManager.Instance.AddListener(DelegateCommand.HANDLECONNECTPORT, connectDefaultAnotherPort);
                defaultConnectFlag = true;
            }

            if (defaultConnectFlag)
            {
                Event curEvent = Event.current;
                DrawArrow(GetOutPositionByPort(defaultConnectRect), curEvent.mousePosition, Color.white);


                if (curEvent.button == 1) // mouse right key
                {
                    DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectDefaultAnotherPort);
                    defaultConnectFlag = false;
                }
            }
        }

        void connectDefaultAnotherPort(object[] objs)
        {
            DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectDefaultAnotherPort);

            BaseWindow window = objs[0] as BaseWindow;

            if (window != null && window.Id != Id
                && window.windowType != NodeType.Router)
            {
                SetDefault(window);
            }

            defaultConnectFlag = false;
        }

        protected float addHeight;
        protected override void gui()
        {
            base.gui();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            GUI.color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;

            if (GUILayout.Button(newCondition, buttonStyle))
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

            GUILayout.FlexibleSpace();

            GUILayout.BeginHorizontal();

            bool right = true;
            foreach (var item in conditions)
            {
                if(string.IsNullOrEmpty(item.className))
                {
                    right = false;
                }
            }


            if (!right)
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

        protected void drawConditions()
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                RouterWindowCondition rc = conditions[i];
                GUILayout.BeginHorizontal();

                //删除
                if (GUILayout.Button("", Styles.miniDelButton))
                {
                    conditions.RemoveAt(i);
                    i--;
                    _size.y -= addHeight;
                }

                string c = rc.className;
                int selectindex = allConditionClass.IndexOf(c);
                selectindex = EditorGUILayout.Popup(selectindex, allConditionClass.ToArray(), popupStyle);
                if (selectindex >= 0)
                {
                    conditions[i].className = allConditionClass[selectindex];
                }

                Rect rect = GUILayoutUtility.GetRect(0, 0);
                if (rect.position != Vector2.zero)
                {
                    rect.position += position + new Vector2(0, connectPortOffset);
                    rect.size = new Vector2(connectPortSize, connectPortSize);
                    rc.connectRect = rect;
                }

                GUILayout.EndHorizontal();
            }
        }

        protected void drawDefualt()
        {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.Label("default", defaultLabel);

            Rect rect = GUILayoutUtility.GetRect(0, 0);
            if (rect.position != Vector2.zero)
            {
                rect.position += position + new Vector2(0, connectPortOffset);
                rect.size = new Vector2(connectPortSize, connectPortSize);

                defaultConnectRect = rect;
            }

            GUILayout.EndHorizontal();
        }

        public override void rightMouseClick(Vector2 mouseposition)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(deleteNode, false, () =>
            {

                if(defaultNextWindow!=null)
                {
                    defaultNextWindow.SetParent(null);
                }

                foreach (var item in conditions)
                {
                    if(item.nextWindow!=null)
                    {
                        item.nextWindow.SetParent(null);
                    }
                }

                windowList.Remove(this);
            });

            menu.ShowAsContext();
        }
    }
}
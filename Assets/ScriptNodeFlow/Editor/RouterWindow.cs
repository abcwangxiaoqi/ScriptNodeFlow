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
        static Type[] tys;
        static RouterWindowCondition()
        {
            Assembly _assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp.dll");
            tys = _assembly.GetTypes();
        }
        
        public string ID = DateTime.Now.ToString("yyMMddHHmmssff");
        public string name = "Condition Name";
        public string className;
        public BaseWindow nextWindow;
        public Rect connectRect;
        public bool connectFlag = false;
        public bool expand = false;

        public void updateClassName(int flowID,string routerID,string cName)
        {
            className = cName;

            if (Application.isPlaying)
                return;
            
            className = null;
            foreach (var item in tys)
            {
                if (item.IsSubclassOf(typeof(RouterCondition)) && !item.IsInterface && !item.IsAbstract)
                {
                    object[] bindings = item.GetCustomAttributes(typeof(BindingFlow), false);
                    object[] routerBindings = item.GetCustomAttributes(typeof(BindingRouter), false);
                    if (bindings != null
                        && bindings.Length > 0
                        && (bindings[0] as BindingFlow).ID == flowID.ToString()
                        //--node
                        && routerBindings != null
                        && routerBindings.Length > 0
                        && (routerBindings[0] as BindingRouter).RouterID == routerID
                        && (routerBindings[0] as BindingRouter).CoditionId == ID)
                    {
                        className = item.FullName;
                        break;
                    }
                }
            }
        }
    }

    public class RouterWindow : BaseWindow
    {
        protected static GUIContent deleteNode = new GUIContent("Delte");

        protected static GUIContent newCondition = new GUIContent("New", "add a new condition");

        protected static GUIStyle conditionNameStyle ;

        static int MaxCondition = 10;
        static RouterWindow()
        {
            conditionNameStyle = new GUIStyle(UnityEditor.EditorStyles.textField);
            conditionNameStyle.fontSize = 12;
            conditionNameStyle.fontStyle = FontStyle.Bold;
            conditionNameStyle.alignment = TextAnchor.MiddleCenter;
            conditionNameStyle.normal.textColor = Color.white;
            conditionNameStyle.focused.textColor = Color.white;
            conditionNameStyle.stretchWidth = true;
        }

        protected List<RouterWindowCondition> conditions = new List<RouterWindowCondition>();

        public override NodeType windowType
        {
            get
            {
                return NodeType.Router;
            }
        }

        const float ConditionH = 22;
        const float expandConditionH = 66;

        const float lineSpace = 5;
        const float defaultWindowHeigth = 110;
        protected Vector2 _size = new Vector2(200, defaultWindowHeigth);
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

        public RouterWindow(Vector2 pos, List<BaseWindow> _windowList,int _flowID)
            : base(pos, _windowList, _flowID)
        {
            Name = "Router";
        }

        public RouterWindow( RouterWindowData itemData, List<BaseWindow> _windowList, int _flowID)
            : base(itemData, _windowList, _flowID)
        {
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
            data.desc = describe;

            foreach (var item in conditions)
            {
                RouterWindowConditionData cond = new RouterWindowConditionData();

                cond.className = item.className;
                cond.ID = item.ID;
                cond.name = item.name;

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
        Event curEvent;
        protected override void drawBefore()
        {
            base.drawBefore();

            curEvent = Event.current;

            foreach (var condition in conditions)
            {

                if (condition.connectFlag && curEvent.button == 1)
                {
                    // mouse right key
                    DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectConditionAnotherPort);
                    condition.connectFlag = false;
                }
            }

            if (defaultConnectFlag && curEvent.button == 1)
            {
                // mouse right key
                DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectDefaultAnotherPort);
                defaultConnectFlag = false;
            }

            float h = 0;

            foreach (var c in conditions)
            {
                if(c.expand)
                {
                    h += expandConditionH+lineSpace;
                }
                else
                {
                    h += ConditionH+lineSpace;
                }
            }

            _size.y =defaultWindowHeigth+ h;
        }

        protected override void drawAfter()
        {
            base.drawAfter();

            GUI.Button(InPortRect, "", parentRef == 0 ? Styles.connectBtn : Styles.connectedBtn);

             foreach (var condition in conditions)
            {
                if (GUI.Button(condition.connectRect, "",
                    (condition.nextWindow != null || condition.connectFlag) ? Styles.connectedBtn : Styles.connectBtn))
                {
                    setConditionNext(condition, null);
                    DelegateManager.Instance.AddListener(DelegateCommand.HANDLECONNECTPORT, connectConditionAnotherPort);
                    condition.connectFlag = true;
                }
            }

            if (GUI.Button(defaultConnectRect, "",
                (defaultNextWindow != null || defaultConnectFlag) ? Styles.connectedBtn : Styles.connectBtn))
            {
                SetDefault(null);
                DelegateManager.Instance.AddListener(DelegateCommand.HANDLECONNECTPORT, connectDefaultAnotherPort);
                defaultConnectFlag = true;
            }


            //-------------------------------

            foreach (var condition in conditions)
            {
                if (condition.connectFlag)
                {
                    DrawArrow(GetOutPositionByPort(condition.connectRect), curEvent.mousePosition, Color.white);
                }
            }

            if (defaultConnectFlag)
            {
                DrawArrow(GetOutPositionByPort(defaultConnectRect), curEvent.mousePosition, Color.white);
            }

            #region condition list line
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

            #region default line

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

        protected override void drawWindowContent()
        {
            base.drawWindowContent();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            GUI.color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;

            EditorGUI.BeginDisabledGroup(conditions.Count == MaxCondition);
            if (GUILayout.Button(newCondition, buttonStyle))
            {
                conditions.Add(new RouterWindowCondition());
            }
            EditorGUI.EndDisabledGroup();

            GUI.color = Color.white;

            GUILayout.Space(10);

            drawConditions();

            GUILayout.Space(10);

            drawDefualt();

            EditorGUI.EndDisabledGroup();
        }

        protected void drawConditions()
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                RouterWindowCondition rc = conditions[i];


                if(rc.expand)
                {
                    GUILayout.Box("", GUILayout.Height(expandConditionH), GUILayout.ExpandWidth(true));
                    Rect r = GUILayoutUtility.GetLastRect();
                    if (r.position != Vector2.zero)
                    {
                        Rect rectBtDelete = new Rect();
                        rectBtDelete.position = r.position + new Vector2(2, (r.size.y / 2) - 8);
                        rectBtDelete.size = new Vector2(16, 16);
                        if (GUI.Button(rectBtDelete, "", Styles.miniDelButton))
                        {
                            conditions.RemoveAt(i);
                            return;
                        }


                        Rect rectNameText = new Rect();
                        rectNameText.position = r.position + new Vector2(20, 5);
                        rectNameText.size = new Vector2(r.size.x - 40, (r.size.y - 20) / 3);  
                        rc.name = EditorGUI.TextField(rectNameText, rc.name,conditionNameStyle);

                        Rect rectBt = new Rect();
                        rectBt.position = r.position + new Vector2(20 + rectNameText.size.x + 2, 5);
                        rectBt.size = new Vector2(16, 16);
                        if (GUI.Button(rectBt, "", Styles.unexpandButton))
                        {
                            rc.expand = false;
                        }

                        Rect rectID = new Rect();
                        rectID.position = r.position + new Vector2(20, 5 + rectNameText.size.y + 5);
                        rectID.size = new Vector2(r.size.x - 40, (r.size.y - 20) / 3);

                        EditorGUI.LabelField(rectID, string.Format("ID: {0}", rc.ID),Styles.subTitleLabel);

                        Rect rectCyBt = new Rect();
                        rectCyBt.position = r.position + new Vector2(20+rectID.size.x + 2, 5 + rectNameText.size.y + 5);
                        rectCyBt.size = new Vector2(16, 16);

                        if (GUI.Button(rectCyBt, GUIContents.copyID,Styles.copyButton))
                        {
                            EditorGUIUtility.systemCopyBuffer = rc.ID;
                        }

                        Rect rectScript = new Rect();
                        rectScript.position = r.position + new Vector2(20, 5 + rectID.size.y + 5 + rectID.size.y + 5);
                        rectScript.size = new Vector2(r.size.x-40, (r.size.y - 20) / 3);

                        if (string.IsNullOrEmpty(rc.className))
                        {
                            EditorGUI.LabelField(rectScript, GUIContents.scriptRefNone, Styles.routerconditionErrorLabel);
                        }
                        else
                        {
                            EditorGUI.LabelField(rectScript, string.Format("Ref: {0}",rc.className), Styles.routerconditionLabel);
                        }

                        rc.connectRect.position = position+ r.position + new Vector2(r.size.x+2, -connectPortOffset+r.size.y / 2);
                        rc.connectRect.size = new Vector2(connectPortSize, connectPortSize);
                    }
                }
                else
                {
                    GUILayout.Box("", GUILayout.Height(ConditionH), GUILayout.ExpandWidth(true));
                    Rect r = GUILayoutUtility.GetLastRect();
                    if (r.position != Vector2.zero)
                    {
                        Rect rectBtDelete = new Rect();
                        rectBtDelete.position = r.position + new Vector2(2, (r.size.y/2)-8);
                        rectBtDelete.size = new Vector2(16,16);
                        if(GUI.Button(rectBtDelete,"",Styles.miniDelButton))
                        {
                            conditions.RemoveAt(i);
                            return;
                        }
                        
                        Rect rectScript = new Rect();
                        rectScript.position = r.position + new Vector2(20, 5);
                        rectScript.size = new Vector2(r.size.x - 40, r.size.y - 10);

                        GUIContent nameContent = new GUIContent(rc.name);
                        if(string.IsNullOrEmpty(rc.className))
                        {
                            nameContent.tooltip = "script ref is none";
                            EditorGUI.LabelField(rectScript, nameContent,Styles.routerconditionNameErrorLabel);
                        }
                        else
                        {
                            EditorGUI.LabelField(rectScript, nameContent, Styles.routerconditionNameLabel);
                        }
                      

                        Rect rectBt = new Rect();
                        rectBt.position = r.position + new Vector2(20+ rectScript.size.x+2, 5);
                        rectBt.size = new Vector2(16, 16);
                        if (GUI.Button(rectBt, "",Styles.expandButton))
                        {
                            rc.expand = true;
                        }

                        rc.connectRect.position = position + r.position + new Vector2(r.size.x+2, -connectPortOffset+r.size.y / 2);
                        rc.connectRect.size = new Vector2(connectPortSize, connectPortSize);
                    }
                }
            }
        }

        static GUIContent defaultContent = new GUIContent("Defualt", "router default condition");
        static GUIContent defaultErrotContent = new GUIContent("Defualt", "router default link is none");

        protected void drawDefualt()
        {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if(defaultNextWindow!=null)
            {
                
                GUILayout.Label(defaultContent, Styles.routerconditionLabel);
            }
            else
            {
                GUILayout.Label(defaultErrotContent, Styles.routerconditionErrorLabel);
            }

            GUILayout.Space(10);

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
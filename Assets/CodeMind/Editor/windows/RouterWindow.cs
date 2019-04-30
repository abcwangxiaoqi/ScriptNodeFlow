using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace CodeMind
{
    public class RouterWindowCondition
    {
        public string ID = DateTime.Now.ToString("yyMMddHHmmssff");
        public string name = "Condition Name";
        public string className;
        public BaseWindow nextWindow;
        public Rect connectRect;
        public bool connectFlag = false;
        public bool expand = false;
    }

    public class RouterWindow : BaseWindow
    {
        static int MaxCondition = 10;

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
        protected string canvasID;
        public RouterWindow(Vector2 pos, List<BaseWindow> _windowList, string _canvasID)
            : base(pos, _windowList)
        {
            canvasID = _canvasID;
            Name = "Router";
        }

        public RouterWindow(RouterWindowData itemData, List<BaseWindow> _windowList, string _canvasID)
            : base(itemData, _windowList)
        {
            canvasID = _canvasID;
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
                if (c.expand)
                {
                    h += expandConditionH + lineSpace;
                }
                else
                {
                    h += ConditionH + lineSpace;
                }
            }

            _size.y = defaultWindowHeigth + h;
        }

        protected override void drawAfter()
        {
            base.drawAfter();

            GUI.Button(InPortRect, "", parentRef == 0 ? CanvasLayout.Layout.canvas.ConnectBtStyle : CanvasLayout.Layout.canvas.ConnectedBtStyle);

            foreach (var condition in conditions)
            {
                if (GUI.Button(condition.connectRect, "",
                    (condition.nextWindow != null || condition.connectFlag) ? CanvasLayout.Layout.canvas.ConnectedBtStyle : CanvasLayout.Layout.canvas.ConnectBtStyle))
                {
                    if(!Application.isPlaying)
                    {
                        setConditionNext(condition, null);
                        DelegateManager.Instance.AddListener(DelegateCommand.HANDLECONNECTPORT, connectConditionAnotherPort);
                        condition.connectFlag = true;
                    }
                }
            }

            if (GUI.Button(defaultConnectRect, "",
                (defaultNextWindow != null || defaultConnectFlag) ? CanvasLayout.Layout.canvas.ConnectedBtStyle : CanvasLayout.Layout.canvas.ConnectBtStyle))
            {
                if(!Application.isPlaying)
                {
                    SetDefault(null);
                    DelegateManager.Instance.AddListener(DelegateCommand.HANDLECONNECTPORT, connectDefaultAnotherPort);
                    defaultConnectFlag = true;
                }
            }


            //-------------------------------

            foreach (var condition in conditions)
            {
                if (condition.connectFlag)
                {
                    DrawArrow(GetOutPositionByPort(condition.connectRect), curEvent.mousePosition, CanvasLayout.Layout.canvas.lineColor);
                }
            }

            if (defaultConnectFlag)
            {
                DrawArrow(GetOutPositionByPort(defaultConnectRect), curEvent.mousePosition, CanvasLayout.Layout.canvas.lineColor);
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

                color = CanvasLayout.Layout.canvas.lineColor;

                if (Application.isPlaying
                    && windowData.runtimeState == RuntimeState.Finished
                    && (windowData as RouterWindowData).runtimeNextId == condition.nextWindow.Id)
                {
                    color = CanvasLayout.Layout.canvas.runtimelineColor;
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

                color = CanvasLayout.Layout.canvas.lineColor;

                if (Application.isPlaying
                    && windowData.runtimeState == RuntimeState.Finished
                    && (windowData as RouterWindowData).runtimeNextId == defaultNextWindow.Id)
                {
                    color = CanvasLayout.Layout.canvas.runtimelineColor;
                }

                DrawArrow(GetOutPositionByPort(defaultConnectRect), defaultNextWindow.In, color);
            }

            #endregion
        }

        void connectConditionAnotherPort(object[] objs)
        {
            DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectConditionAnotherPort);

            BaseWindow window = objs[0] as BaseWindow;

            RouterWindowCondition condition = conditions.Find((cond) =>
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


        void setConditionNext(RouterWindowCondition condition, BaseWindow next)
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

            EditorGUI.BeginDisabledGroup(conditions.Count == MaxCondition);

            GUILayout.Space(5);
            GUI.color = Color.green;
            if (GUILayout.Button(CanvasLayout.Layout.canvas.AddConditionContent, CanvasLayout.Layout.canvas.AddConditionBtStyle))
            {
                conditions.Add(new RouterWindowCondition());
            }
            GUI.color = Color.white;
            EditorGUI.EndDisabledGroup();            

            GUILayout.Space(5);

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


                if (rc.expand)
                {
                    GUIStyle box = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene).box;
                    // box style 不对 要修改
                    GUILayout.Box("",CanvasLayout.Layout.canvas.ConditionBoxStyle ,GUILayout.Height(expandConditionH), GUILayout.ExpandWidth(true));
                    //GUILayout.Box("", box,GUILayout.Height(expandConditionH), GUILayout.ExpandWidth(true));
                    Rect r = GUILayoutUtility.GetLastRect();
                    if (r.position != Vector2.zero)
                    {
                        Rect rectBtDelete = new Rect();
                        rectBtDelete.position = r.position + new Vector2(2, (r.size.y / 2) - 8);
                        rectBtDelete.size = new Vector2(16, 16);

                        if (GUI.Button(rectBtDelete, CanvasLayout.Layout.canvas.DelConditionContent, CanvasLayout.Layout.canvas.DelConditionStyle))
                        {
                            conditions.RemoveAt(i);
                            return;
                        }

                        Rect rectNameText = new Rect();
                        rectNameText.position = r.position + new Vector2(20, 5);
                        rectNameText.size = new Vector2(r.size.x - 40, (r.size.y - 20) / 3);
                        rc.name = EditorGUI.TextField(rectNameText, rc.name, CanvasLayout.Layout.canvas.ConditionNameText);

                        Rect rectBt = new Rect();
                        rectBt.position = r.position + new Vector2(20 + rectNameText.size.x + 2, 5);
                        rectBt.size = new Vector2(16, 16);
                        if (GUI.Button(rectBt, CanvasLayout.Layout.canvas.ConditionUnexpandBtContent, CanvasLayout.Layout.canvas.ConditionUnexpandBtStyle))
                        {
                            rc.expand = false;
                        }

                        Rect rectID = new Rect();
                        rectID.position = r.position + new Vector2(20, 5 + rectNameText.size.y + 5);
                        rectID.size = new Vector2(r.size.x - 40, (r.size.y - 20) / 3);

                        EditorGUI.LabelField(rectID, string.Format("ID: {0}", rc.ID), CanvasLayout.Layout.canvas.IDLabelStyle);

                        Rect rectCyBt = new Rect();
                        rectCyBt.position = r.position + new Vector2(20 + rectID.size.x + 2, 5 + rectNameText.size.y + 5 + 2);
                        rectCyBt.size = new Vector2(16, 16);

                        if (GUI.Button(rectCyBt, CanvasLayout.Layout.canvas.CopyBtContent, CanvasLayout.Layout.common.CopyBtStyle))
                        {
                            EditorGUIUtility.systemCopyBuffer = string.Format(RouterBinding.Format, canvasID, Id, rc.ID);
                        }

                        Rect rectScript = new Rect();
                        rectScript.position = r.position + new Vector2(20, 5 + rectID.size.y + 5 + rectID.size.y + 5);
                        rectScript.size = new Vector2(r.size.x - 40, (r.size.y - 20) / 3);

                        if (string.IsNullOrEmpty(rc.className))
                        {
                            EditorGUI.LabelField(rectScript, CanvasLayout.Layout.common.scriptRefNone, CanvasLayout.Layout.canvas.ConditionErrorLabelStyle);
                        }
                        else
                        {
                            EditorGUI.LabelField(rectScript, string.Format("Ref: {0}", rc.className), CanvasLayout.Layout.canvas.ConditionLabelStyle);
                        }

                        rc.connectRect.position = position + r.position + new Vector2(r.size.x + 2, -connectPortOffset + r.size.y / 2);
                        rc.connectRect.size = new Vector2(connectPortSize, connectPortSize);
                    }
                }
                else
                {
                    GUILayout.Box("", CanvasLayout.Layout.canvas.ConditionBoxStyle,GUILayout.Height(ConditionH), GUILayout.ExpandWidth(true));
                    Rect r = GUILayoutUtility.GetLastRect();
                    if (r.position != Vector2.zero)
                    {
                        Rect rectBtDelete = new Rect();
                        rectBtDelete.position = r.position + new Vector2(2, (r.size.y / 2) - 8);
                        rectBtDelete.size = new Vector2(16, 16);
                        if (GUI.Button(rectBtDelete, CanvasLayout.Layout.canvas.DelConditionContent, CanvasLayout.Layout.canvas.DelConditionStyle))
                        {
                            conditions.RemoveAt(i);
                            return;
                        }

                        Rect rectScript = new Rect();
                        rectScript.position = r.position + new Vector2(20, 5);
                        rectScript.size = new Vector2(r.size.x - 40, r.size.y - 10);

                        GUIContent nameContent = new GUIContent(rc.name);
                        if (string.IsNullOrEmpty(rc.className))
                        {
                            nameContent.tooltip = "script ref is none";
                            EditorGUI.LabelField(rectScript, nameContent, CanvasLayout.Layout.canvas.ConditionErrorLabelStyle);
                        }
                        else
                        {
                            EditorGUI.LabelField(rectScript, nameContent, CanvasLayout.Layout.canvas.ConditionLabelStyle);
                        }


                        Rect rectBt = new Rect();
                        rectBt.position = r.position + new Vector2(20 + rectScript.size.x + 2, 5);
                        rectBt.size = new Vector2(16, 16);
                        if (GUI.Button(rectBt, CanvasLayout.Layout.canvas.ConditionExpandBtContent, CanvasLayout.Layout.canvas.ConditionExpandBtStyle))
                        {
                            rc.expand = true;
                        }

                        rc.connectRect.position = position + r.position + new Vector2(r.size.x + 2, -connectPortOffset + r.size.y / 2);
                        rc.connectRect.size = new Vector2(connectPortSize, connectPortSize);
                    }
                }
            }
        }

        protected void drawDefualt()
        {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (defaultNextWindow != null)
            {
                GUILayout.Label(CanvasLayout.Layout.canvas.DefaultContent, CanvasLayout.Layout.canvas.DefaultLabelStyle);
            }
            else
            {
                GUILayout.Label(CanvasLayout.Layout.canvas.DefaultErrorContent, CanvasLayout.Layout.canvas.DefaultErrorLabelStyle);
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

            menu.AddItem(CanvasLayout.Layout.canvas.DelWindowsContent, false, () =>
            {
                if (defaultNextWindow != null)
                {
                    defaultNextWindow.SetParent(null);
                }

                foreach (var item in conditions)
                {
                    if (item.nextWindow != null)
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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Object = UnityEngine.Object;
using UnityEditor.AnimatedValues;

namespace CodeMind
{
    public class RouterWindowCondition
    {
        CodeMindData mindData;
        
        public RouterWindowCondition(RouterWindowConditionData data,CodeMindData _mindData)
        {
            mindData = _mindData;

            scriptFadeGroup = new AnimBool(true);

            conditionData = data;

            if(data.routerCondition!=null)
            {
                monoScript = MonoScript.FromScriptableObject(data.routerCondition);
                scriptEditor = Editor.CreateEditor(data.routerCondition);
            }
        }

        public RouterWindowConditionData conditionData { get; private set; }
        public BaseWindow nextWindow;
        public Rect connectRect;
        public bool connectFlag = false;
        public bool expand = false;

        const float ConditionH = 22;
        const float expandConditionH = 66;

        AnimBool scriptFadeGroup;
        MonoScript monoScript;
        Editor scriptEditor;
        string error;
        public void draw(List<RouterWindowCondition> conditions, RouterWindowData routerData, Vector2 position, float connectPortSize, float connectPortOffset)
        {
            GUILayout.BeginVertical(CanvasLayout.Layout.canvas.ConditionBoxStyle);

                GUILayout.BeginHorizontal();

                if (GUILayout.Button(CanvasLayout.Layout.canvas.DelConditionContent,
                                     CanvasLayout.Layout.canvas.DelConditionStyle,GUILayout.Width(16),GUILayout.Height(16)))
                {
                    routerData.conditions.Remove(this.conditionData);
                    conditions.Remove(this);
                }
                
                GUIContent nameContent = new GUIContent(conditionData.name);
            if (conditionData.routerCondition == null)
                {
                    nameContent.tooltip = "script ref is none";
                    GUILayout.Label(nameContent
                        , CanvasLayout.Layout.canvas.ConditionUnExpandErrorLabelStyle);
                }
                else
                {
                    GUILayout.Label(nameContent
                        , CanvasLayout.Layout.canvas.ConditionUnExpandLabelStyle);
                }

                if (GUILayout.Button(CanvasLayout.Layout.canvas.ConditionExpandBtContent, 
                                     expand ? CanvasLayout.Layout.canvas.ConditionUnexpandBtStyle : CanvasLayout.Layout.canvas.ConditionExpandBtStyle,GUILayout.Width(16), GUILayout.Height(16)))
                {
                    expand = !expand;
                }

                GUILayout.EndHorizontal();

            if (expand)
            {
                GUILayout.Space(3);
                conditionData.name = EditorGUILayout.TextField(conditionData.name, CanvasLayout.Layout.canvas.ConditionNameText);
                var tempScript = (MonoScript)EditorGUILayout.ObjectField(monoScript, typeof(MonoScript), false);

                if (tempScript == null && tempScript != monoScript)
                {
                    monoScript = tempScript;
                    Object.DestroyImmediate(conditionData.routerCondition,true);
                    conditionData.routerCondition = null;
                    scriptEditor = null;
                }
                else
                {
                    if (tempScript != monoScript)
                    {
                        monoScript = tempScript;

                        var type = monoScript.GetClass();

                        if (type != null
                            && type.IsSubclassOf(typeof(RouterCondition))
                            && !type.IsAbstract
                            && !type.IsInterface)
                        {
                            var data = ScriptableObject.CreateInstance(type);
                            data.name = conditionData.ID;

                            AssetDatabase.AddObjectToAsset(data, mindData);
                            conditionData.routerCondition = data as RouterCondition;

                            scriptEditor = Editor.CreateEditor(conditionData.routerCondition);
                            error = null;
                        }
                        else
                        {
                            error = "script is invalid";
                        }
                    }

                    if (scriptEditor == null)
                    {
                        GUILayout.Label(error);
                    }
                    else
                    {
                        scriptFadeGroup.target = EditorGUILayout.Foldout(scriptFadeGroup.target, "Parameters");

                        if (EditorGUILayout.BeginFadeGroup(scriptFadeGroup.faded))
                        {
                            EditorGUI.indentLevel++;

                            scriptEditor.OnInspectorGUI();

                            EditorGUI.indentLevel--;
                        }
                        EditorGUILayout.EndFadeGroup();
                    }
                }

            }

            GUILayout.EndVertical();

            Rect rect = GUILayoutUtility.GetLastRect();
            if(rect.position!=Vector2.zero)
            {
                    connectRect.position = position + rect.position + new Vector2(rect.size.x + 2, -connectPortOffset + rect.size.y / 2);
                    connectRect.size = new Vector2(connectPortSize, connectPortSize);
            }

            return;
            if (expand)
            {
                GUILayout.BeginVertical(CanvasLayout.Layout.canvas.ConditionBoxStyle);

                conditionData.name = EditorGUILayout.TextField(conditionData.name, CanvasLayout.Layout.canvas.ConditionNameText);

                GUILayout.BeginHorizontal();
                GUILayout.Label("Script");
                
                monoScript = (MonoScript)EditorGUILayout.ObjectField(monoScript, typeof(MonoScript), false);
                GUILayout.EndHorizontal();

                GUILayout.EndVertical();

                //connectRect.position = position + r.position + new Vector2(r.size.x + 2, -connectPortOffset + r.size.y / 2);
                //connectRect.size = new Vector2(connectPortSize, connectPortSize);

                //GUILayout.Box("", CanvasLayout.Layout.canvas.ConditionBoxStyle, GUILayout.Height(expandConditionH), GUILayout.ExpandWidth(true));
                //Rect r = GUILayoutUtility.GetLastRect();
                //if (r.position != Vector2.zero)
                //{
                //    Rect rectBtDelete = new Rect();
                //    rectBtDelete.position = r.position + new Vector2(2, (r.size.y / 2) - 8);
                //    rectBtDelete.size = new Vector2(16, 16);

                //    if (GUI.Button(rectBtDelete, CanvasLayout.Layout.canvas.DelConditionContent, CanvasLayout.Layout.canvas.DelConditionStyle))
                //    {
                //        routerData.conditions.Remove(this.conditionData);
                //        conditions.Remove(this);
                //        return;
                //    }

                //    Rect rectNameText = new Rect();
                //    rectNameText.position = r.position + new Vector2(20, 5);
                //    rectNameText.size = new Vector2(r.size.x - 40, (r.size.y - 20) / 3);
                //    conditionData.name = EditorGUI.TextField(rectNameText, conditionData.name, CanvasLayout.Layout.canvas.ConditionNameText);

                //    Rect rectBt = new Rect();
                //    rectBt.position = r.position + new Vector2(20 + rectNameText.size.x + 2, 5);
                //    rectBt.size = new Vector2(16, 16);
                //    if (GUI.Button(rectBt, CanvasLayout.Layout.canvas.ConditionUnexpandBtContent, CanvasLayout.Layout.canvas.ConditionUnexpandBtStyle))
                //    {
                //        expand = false;
                //    }

                //    Rect rectID = new Rect();
                //    rectID.position = r.position + new Vector2(20, 5 + rectNameText.size.y + 5);
                //    rectID.size = new Vector2(r.size.x - 40, (r.size.y - 20) / 3);

                //    //monoScript = (MonoScript)EditorGUILayout.ObjectField(monoScript, typeof(MonoScript),false);
                //    //EditorGUI.LabelField(rectID, string.Format("ID: {0}", ID), CanvasLayout.Layout.canvas.IDLabelStyle);

                //    Rect rectCyBt = new Rect();
                //    rectCyBt.position = r.position + new Vector2(20 + rectID.size.x + 2, 5 + rectNameText.size.y + 5 + 2);
                //    rectCyBt.size = new Vector2(16, 16);

                //    Rect rectScript = new Rect();
                //    rectScript.position = r.position + new Vector2(20, 5 + rectID.size.y + 5 + rectID.size.y + 5);
                //    rectScript.size = new Vector2(r.size.x - 40, (r.size.y - 20) / 3);

                //    if (conditionData.className == null)
                //    {
                //        EditorGUI.LabelField(rectScript, CanvasLayout.Layout.common.scriptRefNone, CanvasLayout.Layout.canvas.ConditionErrorLabelStyle);
                //    }
                //    else
                //    {
                //        EditorGUI.LabelField(rectScript, string.Format("Ref: {0}", conditionData.className.name), CanvasLayout.Layout.canvas.ConditionLabelStyle);
                //    }

                //    connectRect.position = position + r.position + new Vector2(r.size.x + 2, -connectPortOffset + r.size.y / 2);
                //    connectRect.size = new Vector2(connectPortSize, connectPortSize);
                //}
            }
            else
            {
                GUILayout.Box("", CanvasLayout.Layout.canvas.ConditionBoxStyle, GUILayout.Height(ConditionH), GUILayout.ExpandWidth(true));
                Rect r = GUILayoutUtility.GetLastRect();
                if (r.position != Vector2.zero)
                {
                    Rect rectBtDelete = new Rect();
                    rectBtDelete.position = r.position + new Vector2(2, (r.size.y / 2) - 8);
                    rectBtDelete.size = new Vector2(16, 16);
                    if (GUI.Button(rectBtDelete, CanvasLayout.Layout.canvas.DelConditionContent
                        , CanvasLayout.Layout.canvas.DelConditionStyle))
                    {
                        routerData.conditions.Remove(this.conditionData);
                        conditions.Remove(this);
                        return;
                    }

                    Rect rectScript = new Rect();
                    rectScript.position = r.position + new Vector2(20, 5);
                    rectScript.size = new Vector2(r.size.x - 40, r.size.y - 10);

                    /*GUIContent nameContent = new GUIContent(conditionData.name);
                    if (conditionData.className == null)
                    {
                        nameContent.tooltip = "script ref is none";
                        EditorGUI.LabelField(rectScript, nameContent
                            , CanvasLayout.Layout.canvas.ConditionUnExpandErrorLabelStyle);
                    }
                    else
                    {
                        EditorGUI.LabelField(rectScript, nameContent
                            , CanvasLayout.Layout.canvas.ConditionUnExpandLabelStyle);
                    }*/


                    Rect rectBt = new Rect();
                    rectBt.position = r.position + new Vector2(20 + rectScript.size.x + 2, 5);
                    rectBt.size = new Vector2(16, 16);
                    if (GUI.Button(rectBt, CanvasLayout.Layout.canvas.ConditionExpandBtContent, CanvasLayout.Layout.canvas.ConditionExpandBtStyle))
                    {
                        expand = true;
                    }

                    connectRect.position = position + r.position + new Vector2(r.size.x + 2, -connectPortOffset + r.size.y / 2);
                    connectRect.size = new Vector2(connectPortSize, connectPortSize);
                }
            }
        }
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
        // protected Vector2 _size = new Vector2(200, defaultWindowHeigth);
        protected Vector2 _size = new Vector2(300, defaultWindowHeigth);
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

        RouterWindowData routerData;
        public RouterWindow(RouterWindowData itemData, List<BaseWindow> _windowList, CodeMindData _mindData)
            : base(itemData, _windowList, _mindData)
        {
            routerData = itemData;
        }

        public void SetDefault(BaseWindow defEntity)
        {
            if (defaultNextWindow != null)
            {
                defaultNextWindow.SetParent(null);
            }

            defaultNextWindow = defEntity;

            if (defEntity != null)
            {
                defEntity.SetParent(this);
                routerData.nextWindowId = defEntity.Id;
            }
            else
            {
                routerData.nextWindowId = null;
            }

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
                    if (!Application.isPlaying)
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
                if (!Application.isPlaying)
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

            condition.nextWindow = next;

            if (next != null)
            {
                next.SetParent(this);
                condition.conditionData.nextWindowId = next.Id;
            }
            else
            {
                condition.conditionData.nextWindowId = null;
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

        protected override void drawWindowContent()
        {
            base.drawWindowContent();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            EditorGUI.BeginDisabledGroup(conditions.Count == MaxCondition);

            GUI.color = Color.green;
            if (GUILayout.Button(CanvasLayout.Layout.canvas.AddConditionContent, CanvasLayout.Layout.canvas.AddConditionBtStyle))
            {
                RouterWindowConditionData con = new RouterWindowConditionData();
                routerData.conditions.Add(con);
                conditions.Add(new RouterWindowCondition(con,mindData));
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
                rc.draw(conditions, routerData, position, connectPortSize, connectPortOffset);
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

        public override void deleteWindow()
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
        }
    }
}
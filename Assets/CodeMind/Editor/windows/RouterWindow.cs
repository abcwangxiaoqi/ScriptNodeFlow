using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;
using Object = UnityEngine.Object;
using UnityEditor.AnimatedValues;

namespace EditorCodeMind
{
    public class RouterWindowCondition
    {
        CodeMindData mindData;

        public RouterWindowCondition(RouterWindowConditionProxy data, BaseCanvas canvas)
        {
            mindData = canvas.codeMindData;

            scriptFadeGroup = new AnimBool(true);
            scriptFadeGroup.valueChanged.AddListener(canvas.Repaint);

            conditionData = data;

            if (data.routerCondition != null)
            {
                monoScript = MonoScript.FromScriptableObject(data.routerCondition);
                scriptEditor = Editor.CreateEditor(data.routerCondition);
            }
        }

        public RouterWindowConditionProxy conditionData { get; private set; }
        public BaseWindow nextWindow;
        public Rect connectRect;
        public bool connectFlag = false;
        public bool expand = false;

        const float ConditionH = 22;
        const float expandConditionH = 66;

        AnimBool scriptFadeGroup;
        MonoScript monoScript;
        Editor scriptEditor;

        public void draw(List<RouterWindowCondition> conditions, RouterWindowProxy routerData, Vector2 position, float connectPortSize, float connectPortOffset)
        {
            GUILayout.BeginVertical(CanvasLayout.Layout.canvas.ConditionBoxStyle);

            GUILayout.BeginHorizontal();

            if (GUILayout.Button(CanvasLayout.Layout.canvas.DelConditionContent,
                                 CanvasLayout.Layout.canvas.DelConditionStyle, GUILayout.Width(16), GUILayout.Height(16)))
            {
                //routerData.conditions.Remove(this.conditionData);
                mindData.RemoveCondition(routerData, conditionData);
                conditions.Remove(this);
                return;
            }

            GUIContent nameContent = new GUIContent(conditionData.name);
            if (conditionData.routerCondition == null)
            {
                nameContent.tooltip = "script is invalid";
                GUILayout.Label(nameContent
                    , CanvasLayout.Layout.canvas.ConditionUnExpandErrorLabelStyle);
            }
            else
            {
                if(expand)
                {
                    conditionData.name = EditorGUILayout.TextField(conditionData.name, CanvasLayout.Layout.canvas.ConditionNameText);
                }
                else
                {
                    GUILayout.Label(nameContent
    , CanvasLayout.Layout.canvas.ConditionUnExpandLabelStyle);
                }
            }

            if (GUILayout.Button(CanvasLayout.Layout.canvas.ConditionExpandBtContent,
                                 expand ? CanvasLayout.Layout.canvas.ConditionUnexpandBtStyle : CanvasLayout.Layout.canvas.ConditionExpandBtStyle, GUILayout.Width(16), GUILayout.Height(16)))
            {
                expand = !expand;
            }

            GUILayout.EndHorizontal();

            if (expand)
            {
                //GUILayout.Space(3);
                //conditionData.name = EditorGUILayout.TextField(conditionData.name, CanvasLayout.Layout.canvas.ConditionNameText);
                /*var tempScript = (MonoScript)EditorGUILayout.ObjectField(monoScript, typeof(MonoScript), false);

                if (tempScript == null && tempScript != monoScript)
                {
                    monoScript = tempScript;
                    Object.DestroyImmediate(conditionData.routerCondition, true);
                    conditionData.routerCondition = null;
                    scriptEditor = null;
                }
                else
                {*/
                    /*if (tempScript != monoScript)
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
                        }
                    }*/

                    if (scriptEditor == null)
                    {
                        GUILayout.Label(CanvasLayout.Layout.canvas.RouterScriptErrorContent,CanvasLayout.Layout.canvas.RouterScriptErrorStyle);
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
                //}

            }

            GUILayout.EndVertical();

            Rect rect = GUILayoutUtility.GetLastRect();
            if (rect.position != Vector2.zero)
            {
                connectRect.position = position + rect.position + new Vector2(rect.size.x + 2, -connectPortOffset + rect.size.y / 2);
                connectRect.size = new Vector2(connectPortSize, connectPortSize);
            }
        }

        internal void OnDelete()
        {
            if(conditionData.routerCondition !=null )
            {
                Object.DestroyImmediate(conditionData.routerCondition, true);
            }
        }
    }

    public class RouterWindow : BaseWindow
    {
        static int MaxCondition = 10;

        public List<RouterWindowCondition> conditions { get; private set; }

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

        protected override float windowWidth
        {
            get
            {
                return 350f;
            }
        }

        protected BaseWindow defaultNextWindow = null;
        protected Rect defaultConnectRect;
        protected bool defaultConnectFlag = false;

        RouterWindowProxy routerData;
        public RouterWindow(RouterWindowProxy itemData, BaseCanvas canvas)
            : base(itemData, canvas)
        {
            routerData = itemData;

            conditions = new List<RouterWindowCondition>();

            foreach (var item in routerData.conditions)
            {
                RouterWindowCondition condition = new RouterWindowCondition(item, canvas);
                conditions.Add(condition);
            }
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

        /*public void SetConditions(List<RouterWindowCondition> conditionEntities)
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
        }*/

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
                    MainCanvas.m_DelegateManager.RemoveListener(BaseCanvas.HANDLECONNECTPORT, connectConditionAnotherPort);
                    condition.connectFlag = false;
                }
            }

            if (defaultConnectFlag && curEvent.button == 1)
            {
                // mouse right key
                MainCanvas.m_DelegateManager.RemoveListener(BaseCanvas.HANDLECONNECTPORT, connectDefaultAnotherPort);
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
                        MainCanvas.m_DelegateManager.AddListener(BaseCanvas.HANDLECONNECTPORT, connectConditionAnotherPort);
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
                    MainCanvas.m_DelegateManager.AddListener(BaseCanvas.HANDLECONNECTPORT, connectDefaultAnotherPort);
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
                    && (windowData as RouterWindowProxy).runtimeNextId == condition.nextWindow.Id)
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
                    && (windowData as RouterWindowProxy).runtimeNextId == defaultNextWindow.Id)
                {
                    color = CanvasLayout.Layout.canvas.runtimelineColor;
                }

                DrawArrow(GetOutPositionByPort(defaultConnectRect), defaultNextWindow.In, color);
            }

            #endregion
        }

        void connectConditionAnotherPort(object[] objs)
        {
            MainCanvas.m_DelegateManager.RemoveListener(BaseCanvas.HANDLECONNECTPORT, connectConditionAnotherPort);

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
            MainCanvas.m_DelegateManager.RemoveListener(BaseCanvas.HANDLECONNECTPORT, connectDefaultAnotherPort);

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

            GUI.color = CanvasLayout.Layout.canvas.AddConditionBtColor;
            if (GUILayout.Button(CanvasLayout.Layout.canvas.AddConditionContent, CanvasLayout.Layout.canvas.AddConditionBtStyle))
            {
                /*RouterWindowConditionData con = new RouterWindowConditionData();
                routerData.conditions.Add(con);
                conditions.Add(new RouterWindowCondition(con, MainCanvas));*/

                GenericMenu menu = new GenericMenu();

                foreach (var item in MainCanvas.customConditionStructList)
                {
                    GUIContent content = new GUIContent(item.attribute.viewText);
                    menu.AddItem(content, false, () => 
                    {
                        var data = mindData.AddCondition(item.type, item.attribute);
                        routerData.conditions.Add(data);

                        RouterWindowCondition condition = new RouterWindowCondition(data, MainCanvas);
                        conditions.Add(condition);
                    });
                }

                menu.ShowAsContext();
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
                rect.position += position + new Vector2(connectPortOffset, connectPortOffset);
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

                item.OnDelete();
            }
        }
    }
}
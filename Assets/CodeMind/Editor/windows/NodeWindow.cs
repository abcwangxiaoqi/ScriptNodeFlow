using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMind
{
    public class NodeWindow : BaseWindow
    {
        //下一节点
        public BaseWindow next { get; protected set; }

        protected override float windowWidth
        {
            get
            {
                return 350f;
            }
        }

        public override NodeType windowType
        {
            get
            {
                return NodeType.Node;
            }
        }

        MonoScript monoScript;

        public NodeWindowProxy nodeData;

        Editor scriptEditor;

        AnimBool scriptFadeGroup;

        public NodeWindow(NodeWindowProxy itemData, BaseCanvas canvas)
            : base(itemData, canvas)
        {
            nodeData = itemData;
            if(nodeData.node!= null)
            {
                monoScript = MonoScript.FromScriptableObject(nodeData.node);
                scriptEditor = Editor.CreateEditor(nodeData.node);
            }

            scriptFadeGroup = new AnimBool(true);
            scriptFadeGroup.valueChanged.AddListener(canvas.Repaint);
        }

        public void SetNext(BaseWindow entity)
        {
            if (next != null)
            {
                next.SetParent(null);
            }

            next = entity;

            if (entity != null)
            {
                entity.SetParent(entity);
                nodeData.nextWindowId = entity.Id;
            }     
            else
            {
                nodeData.nextWindowId = null;
            }
        }

        protected override void drawBefore()
        {
            base.drawBefore();

            curEvent = Event.current;

            if (connectFlag && curEvent.button == 1)
            {
                // mouse right key
                connectFlag = false;
                DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectAnotherPort);
            }
        }

        Event curEvent;

        protected override void drawAfter()
        {
            base.drawAfter();

            GUI.Button(InPortRect, "", parentRef == 0 ? CanvasLayout.Layout.canvas.ConnectBtStyle : CanvasLayout.Layout.canvas.ConnectedBtStyle);

            if (GUI.Button(OutPortRect, "", (connectFlag || next != null) ? CanvasLayout.Layout.canvas.ConnectedBtStyle : CanvasLayout.Layout.canvas.ConnectBtStyle))
            {
                if(!Application.isPlaying)
                {
                    SetNext(null);
                    DelegateManager.Instance.AddListener(DelegateCommand.HANDLECONNECTPORT, connectAnotherPort);
                    connectFlag = true;
                }
            }

            //draw line
            if (next != null)
            {
                if (!windowList.Contains(next))
                {
                    next = null;
                    return;
                }

                Color color = CanvasLayout.Layout.canvas.lineColor;

                if (Application.isPlaying && windowData.runtimeState == RuntimeState.Finished)
                {
                    color = CanvasLayout.Layout.canvas.runtimelineColor;
                }

                DrawArrow(GetOutPositionByPort(OutPortRect), next.In, color);
            }


            if (connectFlag)
            {
                DrawArrow(GetOutPositionByPort(OutPortRect), curEvent.mousePosition, CanvasLayout.Layout.canvas.lineColor);
            }
        }

        void connectAnotherPort(object[] objs)
        {
            DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectAnotherPort);

            BaseWindow window = objs[0] as BaseWindow;

            if (window != null && window.Id != Id)
            {
                SetNext(window);
            }

            connectFlag = false;
        }

        private bool connectFlag = false;
        protected override void drawWindowContent()
        {
            base.drawWindowContent();

           /*  var tempScript = (MonoScript)EditorGUILayout.ObjectField(monoScript, typeof(MonoScript), false);

            if(tempScript == null && tempScript != monoScript)
            {
                monoScript = tempScript;                
                Object.DestroyImmediate(nodeData.node,true);
                nodeData.node = null;
                scriptEditor = null;
            }
            else
            {
                if (tempScript != monoScript)
                {
                    monoScript = tempScript;

                    var type = monoScript.GetClass();

                    if (type != null
                        && type.IsSubclassOf(typeof(Node))
                        && !type.IsAbstract
                        && !type.IsInterface)
                    {
                        var data = ScriptableObject.CreateInstance(type);
                        data.name = Id;

                        AssetDatabase.AddObjectToAsset(data, mindData);
                        nodeData.node = data as Node;

                        scriptEditor = Editor.CreateEditor(nodeData.node);

                    }
                }
            }    */   

            if(scriptEditor == null)
            {
                GUILayout.Label(CanvasLayout.Layout.canvas.NodeScriptErrorContent, CanvasLayout.Layout.canvas.NodeScriptErrorStyle);
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

        public override void deleteWindow()
        {
            if (next != null)
            {
                next.SetParent(null);
            }

            if(nodeData.node != null)
            {
                Object.DestroyImmediate(nodeData.node, true);
            }
        }
    }
}

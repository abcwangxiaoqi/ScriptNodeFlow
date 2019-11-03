using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMind
{
    
    internal class NodeWindow : BaseWindow
    {
        //下一节点
        public BaseWindow next { get; protected set; }

        protected sealed override float windowWidth
        {
            get
            {
                return 350f;
            }
        }

        public sealed override NodeType windowType
        {
            get
            {
                return NodeType.Node;
            }
        }

        //public NodeWindowData nodeData;

        public Node node;

        protected Editor scriptEditor;

        protected AnimBool scriptFadeGroup;

        protected CodeMindNodeAttribute attribute;

        /*public NodeWindow(CodeMindNodeAttribute attr,NodeWindowData itemData, BaseCanvas canvas)
            : base(itemData, canvas)
        {
            attribute = attr;
            nodeData = itemData;
            if(nodeData.node!= null)
            {
                scriptEditor = Editor.CreateEditor(nodeData.node);
            }

            scriptFadeGroup = new AnimBool(true);
            scriptFadeGroup.valueChanged.AddListener(canvas.Repaint);
        }*/

        public NodeWindow(CodeMindNodeAttribute attr, Node _node, BaseCanvas canvas)
            : base(_node.ID,_node.position,attr.showName, attr.windowDes,canvas)
        {
            attribute = attr;
            node = _node;
            if (node != null)
            {
                scriptEditor = Editor.CreateEditor(node);
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
                node.nextWindowId = entity.Id;
            }     
            else
            {
                node.nextWindowId = null;
            }
        }

        protected sealed override void drawBefore()
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

        protected sealed override void drawAfter()
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

               /* if (Application.isPlaying && windowData.runtimeState == RuntimeState.Finished)
                {
                    color = CanvasLayout.Layout.canvas.runtimelineColor;
                }*/

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
        protected override void OnDrawContent()
        {
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

        public sealed override void deleteWindow()
        {
            if (next != null)
            {
                next.SetParent(null);
            }

            if(node != null)
            {
                Object.DestroyImmediate(node, true);
            }
        }
    }
}

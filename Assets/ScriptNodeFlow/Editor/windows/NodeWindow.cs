using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class NodeWindow : BaseWindow
    {
        protected string canvasID;

        protected string ClassName { get; private set; }

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
                return NodeType.Node;
            }
        }

        public NodeWindow(Vector2 pos, List<BaseWindow> _windowList, string _canvasID)
            : base(pos, _windowList)
        {
            canvasID = _canvasID;
            Name = "Node";
        }

        public NodeWindow(NodeWindowData itemData, List<BaseWindow> _windowList, string _canvasID)
            : base(itemData, _windowList)
        {
            canvasID = _canvasID;

            if (Application.isPlaying)
            {
                ClassName = itemData.className;
            }
            else
            {
                foreach (var item in Util.EngineTypes)
                {
                    if (item.IsSubclassOf(typeof(Node)) && !item.IsInterface && !item.IsAbstract)
                    {
                        object[] nodeBinings = item.GetCustomAttributes(typeof(NodeBinding), false);
                        if (nodeBinings != null
                            && nodeBinings.Length > 0
                            && (nodeBinings[0] as NodeBinding).WindowID == Id
                            && (nodeBinings[0] as NodeBinding).CanvasID == canvasID)
                        {
                            ClassName = item.FullName;
                            break;
                        }
                    }
                }
            }
        }

        public void SetNext(BaseWindow entity)
        {
            if (next != null)
            {
                next.SetParent(null);
            }

            if (entity != null)
            {
                entity.SetParent(entity);
            }

            next = entity;
        }

        public override WindowDataBase GetData()
        {
            NodeWindowData dataEntity = new NodeWindowData();
            dataEntity.position = position;
            dataEntity.name = Name;
            dataEntity.ID = Id;
            dataEntity.desc = describe;

            dataEntity.className = ClassName;

            if (next != null)
            {
                dataEntity.nextWindowId = next.Id;
            }

            return dataEntity;
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

            GUILayout.BeginHorizontal();

            if (string.IsNullOrEmpty(ClassName))
            {
                GUILayout.Label(CanvasLayout.Layout.common.scriptRefNone, CanvasLayout.Layout.canvas.NodeRefErrorLabelStyle);
            }
            else
            {
                GUILayout.Label(ClassName, CanvasLayout.Layout.canvas.NodeRefNameLabelStyle);
            }

            GUILayout.EndHorizontal();
        }

        public override void rightMouseClick(Vector2 mouseposition)
        {
            GenericMenu menu = new GenericMenu();
            
            menu.AddItem(CanvasLayout.Layout.canvas.DelWindowsContent, false, () =>
            {
                if (next != null)
                {
                    next.SetParent(null);
                }
                windowList.Remove(this);
            });

            menu.ShowAsContext();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class NodeWindow : BaseWindow
    {
        static Type[] tys;
        static NodeWindow()
        {
            Assembly _assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp.dll");
            tys = _assembly.GetTypes();
        }

        protected string flowID;

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

        public NodeWindow(Vector2 pos, List<BaseWindow> _windowList, int _flowID)
            : base(pos, _windowList)
        {
            flowID = _flowID.ToString();
            Name = "Node";
        }

        public NodeWindow(NodeWindowData itemData, List<BaseWindow> _windowList, int _flowID)
            : base(itemData, _windowList)
        {
            flowID = _flowID.ToString();

            if (Application.isPlaying)
            {
                ClassName = itemData.className;
            }
            else
            {
                foreach (var item in tys)
                {
                    if (item.IsSubclassOf(typeof(Node)) && !item.IsInterface && !item.IsAbstract)
                    {
                        object[] nodeBinings = item.GetCustomAttributes(typeof(NodeBinding), false);
                        if (nodeBinings != null
                            && nodeBinings.Length > 0
                            && (nodeBinings[0] as NodeBinding).WindowID == Id
                            && (nodeBinings[0] as NodeBinding).CanvasID == flowID)
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

            if (!Application.isPlaying && GUI.Button(OutPortRect, "", (connectFlag || next != null) ? CanvasLayout.Layout.canvas.ConnectedBtStyle : CanvasLayout.Layout.canvas.ConnectBtStyle))
            {
                SetNext(null);
                DelegateManager.Instance.AddListener(DelegateCommand.HANDLECONNECTPORT, connectAnotherPort);
                connectFlag = true;
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

        void refreshScript()
        {
            ClassName = string.Empty;
            foreach (var item in tys)
            {
                if (item.IsSubclassOf(typeof(Node)) && !item.IsInterface && !item.IsAbstract)
                {
                    object[] bindingNode = item.GetCustomAttributes(typeof(NodeBinding), false);
                    if (bindingNode != null
                        && bindingNode.Length > 0
                        && (bindingNode[0] as NodeBinding).WindowID == Id
                        && (bindingNode[0] as NodeBinding).CanvasID == flowID
                        )
                    {
                        ClassName = item.FullName;
                        break;
                    }
                }
            }
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

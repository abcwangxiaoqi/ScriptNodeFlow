using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class NodeWindow : BaseWindow
    {
        protected GUIContent deleteContent = new GUIContent("Delte");

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
                        object[] bindings = item.GetCustomAttributes(typeof(BindingFlow), false);
                        object[] nodeBinings = item.GetCustomAttributes(typeof(BindingNode), false);
                        if (bindings != null
                            && bindings.Length > 0
                            && (bindings[0] as BindingFlow).ID == flowID
                            //--node
                            && nodeBinings!=null
                            && nodeBinings.Length>0
                            && (nodeBinings[0] as BindingNode).ID == Id)
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

            GUI.Button(InPortRect, "", parentRef == 0 ? Styles.connectBtn : Styles.connectedBtn);

            if (GUI.Button(OutPortRect, "", (connectFlag || next != null) ? Styles.connectedBtn : Styles.connectBtn))
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

                Color color = Color.white;

                if (Application.isPlaying && windowData.runtimeState == RuntimeState.Finished)
                {
                    color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
                }

                DrawArrow(GetOutPositionByPort(OutPortRect), next.In, color);
            }


            if (connectFlag)
            {
                DrawArrow(GetOutPositionByPort(OutPortRect), curEvent.mousePosition, Color.white);
            }
        }

        void connectAnotherPort(object[] objs)
        {
            DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectAnotherPort);

            BaseWindow window = objs[0] as BaseWindow;

            if (window!=null && window.Id != Id)
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

            if(string.IsNullOrEmpty(ClassName))
            {
                GUILayout.Label(GUIContents.scriptRefNone,Styles.nodeErrorLabel);
            }
            else
            {
                GUILayout.Label(ClassName,Styles.nodeClassNameLabel);
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
                    object[] bindingFlow = item.GetCustomAttributes(typeof(BindingFlow), false);
                    object[] bindingNode = item.GetCustomAttributes(typeof(BindingNode), false);
                    if (bindingFlow != null
                        && bindingFlow.Length > 0
                        && (bindingFlow[0] as BindingFlow).ID == flowID
                        && bindingNode!=null
                        && bindingNode.Length > 0
                        && (bindingNode[0] as BindingNode).ID == Id
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

            menu.AddItem(deleteContent, false, () =>
            {
                if(next!=null)
                {
                    next.SetParent(null);
                }
                windowList.Remove(this);
            });


            menu.ShowAsContext();
        }
    }
}

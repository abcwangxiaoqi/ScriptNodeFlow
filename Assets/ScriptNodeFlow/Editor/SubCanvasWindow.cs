using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace ScriptNodeFlow
{
    public class SubCanvasWindow : BaseWindow,IDisposable
    {
        static GUIContent doubleClickContent = new GUIContent("double click to open canvas");

        static GUIContent deleteContent = new GUIContent("Delte");

        static SubCanvasWindow()
        {
        }

        protected SubNodeCanvasData canvas { get; private set; }

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
                return NodeType.SubCanvas;
            }
        }

        List<SubNodeCanvasData> subCanvasList= new List<SubNodeCanvasData>();
        List<string> subCanvasNameList = new List<string>();
        void refreshSublist(object[] objs)
        {
            subCanvasList.Clear();
            subCanvasNameList.Clear();

            Object[] subs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(orginData));
            foreach (var item in subs)
            {
                if (item is SubNodeCanvasData)
                {
                    subCanvasList.Add(item as SubNodeCanvasData);
                    subCanvasNameList.Add(item.name);
                }
            }
        }

        NodeCanvasData orginData;

        public SubCanvasWindow(Vector2 pos, List<BaseWindow> _windowList, NodeCanvasData _orginData)
            : base(pos, _windowList)
        {
            Name = "Canvas";
            orginData = _orginData;
            refreshSublist(null);
            DelegateManager.Instance.AddListener(DelegateCommand.REFRESHSUBLIST, refreshSublist);
        }

        public SubCanvasWindow(CanvasWindowData itemData, List<BaseWindow> _windowList, NodeCanvasData _orginData)
            : base(itemData, _windowList)
        {
            orginData = _orginData;
            canvas = itemData.canvasData;
            refreshSublist(null);            
            DelegateManager.Instance.AddListener(DelegateCommand.REFRESHSUBLIST, refreshSublist);
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
            CanvasWindowData dataEntity = new CanvasWindowData();
            dataEntity.position = position;
            dataEntity.name = Name;
            dataEntity.ID = Id;
            dataEntity.desc = describe;

            dataEntity.canvasData = canvas;

            if (next != null)
            {
                dataEntity.nextWindowId = next.Id;
            }

            return dataEntity;
        }

        private bool connectFlag = false;

        Event curEvent;

        protected override void drawBefore()
        {
            base.drawBefore();

            curEvent = Event.current;
            if (connectFlag && curEvent.button == 1)
            {
                // mouse right key
                DelegateManager.Instance.RemoveListener(DelegateCommand.HANDLECONNECTPORT, connectAnotherPort);
                connectFlag = false;
            }
        }

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

            if (connectFlag)
            {
                DrawArrow(GetOutPositionByPort(OutPortRect), curEvent.mousePosition, Color.white);
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

        private Object obj;
        private SubNodeCanvasData tempCanvas;
        protected override void drawWindowContent()
        {
            base.drawWindowContent();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            GUILayout.Space(4);

            int index = subCanvasList.IndexOf(canvas);
            index = EditorGUILayout.Popup(index, subCanvasNameList.ToArray());
            if(index>=0)
            {
                canvas = subCanvasList[index];
            }

            GUILayout.FlexibleSpace();

            if(canvas == null)
            {
                GUILayout.Label(NoneCanvasContent, Styles.subCanvasErrorLabel);
            }

            EditorGUI.EndDisabledGroup();
        }

        string tt;

        static GUIContent NoneCanvasContent = new GUIContent("no subcanvas");

        public override void rightMouseClick(Vector2 mouseposition)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(deleteContent, false, () =>
            {
                if (next != null)
                {
                    next.SetParent(null);
                }
                windowList.Remove(this);
            });

            menu.ShowAsContext();
        }

        public override void leftMouseDoubleClick()
        {
            if(canvas==null)
                return;

            
            DelegateManager.Instance.Dispatch(DelegateCommand.OPENSUBCANVAS,
                canvas);
            GUIUtility.ExitGUI();
        }

        public void Dispose()
        {
            DelegateManager.Instance.RemoveListener(DelegateCommand.REFRESHSUBLIST, refreshSublist);
        }
    }
}

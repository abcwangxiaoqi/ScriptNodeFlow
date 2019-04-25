using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

using Object = UnityEngine.Object;

namespace CodeMind
{
    public class SubCanvasWindow : BaseWindow,IDisposable
    {
        static SubCanvasWindow()
        {
        }

        protected SubCanvasData canvas { get; private set; }

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

        List<SubCanvasData> subCanvasList= new List<SubCanvasData>();
        List<string> subCanvasNameList = new List<string>();
        void refreshSublist(object[] objs)
        {
            subCanvasList.Clear();
            subCanvasNameList.Clear();

            Object[] subs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(orginData));
            foreach (var item in subs)
            {
                if (item is SubCanvasData)
                {
                    subCanvasList.Add(item as SubCanvasData);
                    subCanvasNameList.Add(item.name);
                }
            }
        }

        CodeMindData orginData;

        public SubCanvasWindow(Vector2 pos, List<BaseWindow> _windowList, CodeMindData _orginData)
            : base(pos, _windowList)
        {
            Name = "Canvas";
            orginData = _orginData;
            refreshSublist(null);
            DelegateManager.Instance.AddListener(DelegateCommand.REFRESHSUBLIST, refreshSublist);
        }

        public SubCanvasWindow(CanvasWindowData itemData, List<BaseWindow> _windowList, CodeMindData _orginData)
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

            if (connectFlag)
            {
                DrawArrow(GetOutPositionByPort(OutPortRect), curEvent.mousePosition, CanvasLayout.Layout.canvas.lineColor);
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
        private SubCanvasData tempCanvas;
        protected override void drawWindowContent()
        {
            base.drawWindowContent();

            EditorGUI.BeginDisabledGroup(Application.isPlaying);

            GUILayout.Space(4);

            int index = subCanvasList.IndexOf(canvas);

            //  index = EditorGUILayout.Popup(index, subCanvasNameList.ToArray());
            index = EditorGUILayout.Popup(index, subCanvasNameList.ToArray(),CanvasLayout.Layout.canvas.SubCanvasPopupStyle);
            if (index>=0)
            {
                canvas = subCanvasList[index];
            }

            GUILayout.FlexibleSpace();

            if(canvas == null)
            {
                GUILayout.Label(CanvasLayout.Layout.canvas.SubCanvasNoneContent, CanvasLayout.Layout.canvas.SubCanvasErrorLabel);
            }

            EditorGUI.EndDisabledGroup();
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

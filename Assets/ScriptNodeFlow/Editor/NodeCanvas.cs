using EditorTools;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptNodeFlow
{
    public enum CanvasType
    {
        Main,
        Sub
    }

    public abstract class NodeCanvas : EditorWindow
    {
        protected static EditorWindow window;

        protected NodeCanvasData nodeCanvasData;

        protected string Orgin;//nodeCanvasData's asset path

        protected SubNodeCanvasData subNodeCanvasData;

        //全部
        protected List<BaseWindow> windowList = null;

        protected ShareDataWindow shareDataWindow;
        protected SubCanvasListWindow subCanvasListWindow;
        protected SelectedWinWindow selectedWinWindow;

        protected CanvasType canvasType = CanvasType.Main;

        protected BaseWindow curSelect = null;

        protected virtual void Awake()
        {
            DelegateManager.Instance.RemoveListener(DelegateCommand.OPENMAINCANVAS, OpenMainCanvas);
            DelegateManager.Instance.AddListener(DelegateCommand.OPENMAINCANVAS, OpenMainCanvas);

            DelegateManager.Instance.RemoveListener(DelegateCommand.OPENSUBCANVAS, OpenSubCanvas);
            DelegateManager.Instance.AddListener(DelegateCommand.OPENSUBCANVAS, OpenSubCanvas);

        }

        protected virtual void OpenMainCanvas(object[] objs)
        {
            Focus();
            canvasType = CanvasType.Main;
            generateMainData();
            Repaint();
        }

        protected virtual void OpenSubCanvas(object[] objs)
        {
            Focus();
            subNodeCanvasData = objs[0] as SubNodeCanvasData;
            canvasType = CanvasType.Sub;
            generateSubData();
            Repaint();
        }

        protected virtual void Update()
        { }

        private const float border = 5;
        private const float toolWidth = 200;

        protected Rect leftArea = new Rect(border, border, 0, 0);
        protected Rect rightArea = new Rect(toolWidth + 3 * border, border, 0, 0);

        private const float navigationAreaH = 20;
        Rect navigationArea = new Rect(0, 0, 0, 0);
        protected Rect nodesArea = new Rect();

        protected virtual void OnGUI()
        {
            leftArea.size = new Vector2(toolWidth, position.height - 2 * border);
            rightArea.size = new Vector2(position.width - toolWidth - 4 * border, position.height - 2 * border);
            
            GUILayout.BeginArea(leftArea, EditorStyles.textArea);

            shareDataWindow.draw(leftArea);
            subCanvasListWindow.draw(leftArea);
            selectedWinWindow.draw(leftArea);

            GUILayout.EndArea();

            //GUILayout.BeginArea(rightArea, EditorStyles.textArea);
            GUILayout.BeginArea(rightArea, Styles.canvasArea);

            #region navigation

            navigationArea.size = new Vector2(rightArea.width, navigationAreaH);
            GUILayout.BeginArea(navigationArea);
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

            EditorGUI.BeginDisabledGroup(canvasType == CanvasType.Main);

            if (GUILayout.Button("Back", EditorStyles.toolbarButton))
            {
                DelegateManager.Instance.Dispatch(DelegateCommand.SELECTMAINCANVAS);
            }

            EditorGUI.EndDisabledGroup();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Focus", EditorStyles.toolbarButton))
            {
                toolbarFocus();
            }
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();

            #endregion

            #region nodesArea
            
            //BeginWindows();

            nodesArea = new Rect(border, navigationArea.height + border, rightArea.width - 2 * border,
                rightArea.height - navigationArea.height - 2 * border);
            GUILayout.BeginArea(nodesArea);


            if (windowList != null)
            {
                for (int i = 0; i < windowList.Count; i++)
                {
                    windowList[i].draw();
                }
            }

            Repaint();

           // EndWindows();

            GUILayout.Label(canvasType == CanvasType.Main ? nodeCanvasData.name : subNodeCanvasData.name,
                Styles.canvasTitleLabel);

            GUILayout.EndArea();

            #endregion

            GUILayout.EndArea();
        }

        protected virtual void OnEnable()
        {

        }

        protected virtual void OnDisable()
        {

        }

        protected virtual void OnLostFocus()
        {

        }

        protected virtual void OnDestroy()
        {

        }

        protected virtual void OnProjectChange()
        {

        }

        BaseWindow FindWindow(string id)
        {
            BaseWindow res = windowList.Find((BaseWindow w) =>
            {
                return w.Id == id;
            });
            return res;
        }

        protected void generateLeftArea()
        {
            shareDataWindow = new ShareDataWindow(nodeCanvasData.shareData);
            subCanvasListWindow = new SubCanvasListWindow(nodeCanvasData);
            selectedWinWindow = new SelectedWinWindow();
        }

        protected void generateMainData()
        {
            windowList = new List<BaseWindow>();

            windowList.Add(new StartWindow(Orgin, nodeCanvasData.start, windowList));

            foreach (var item in nodeCanvasData.nodelist)
            {
                windowList.Add(new NodeWindow(Orgin, item, windowList));
            }

            foreach (var item in nodeCanvasData.routerlist)
            {
                windowList.Add(new RouterWindow(Orgin, item, windowList));
            }

            foreach (var item in nodeCanvasData.subCanvaslist)
            {
                windowList.Add(new SubCanvasWindow(Orgin, item, windowList));
            }

            //set next Node
            foreach (var item in windowList)
            {
                if (item.windowType == NodeType.Node)
                {
                    NodeWindowData edata = nodeCanvasData.nodelist.Find(data => { return data.ID == item.Id; });

                    if (!string.IsNullOrEmpty(edata.nextWindowId))
                    {
                        BaseWindow next = FindWindow(edata.nextWindowId);

                        (item as NodeWindow).SetNext(next);
                    }
                }
                else if (item.windowType == NodeType.Router)
                {
                    RouterWindowData edata = nodeCanvasData.routerlist.Find(data => { return data.ID == item.Id; });
                    RouterWindow win = item as RouterWindow;

                    //set default
                    if (!string.IsNullOrEmpty(edata.nextWindowId))
                    {
                        BaseWindow def = FindWindow(edata.nextWindowId);
                        win.SetDefault(def);
                    }

                    //set conditions
                    List<RouterWindowCondition> conditions = new List<RouterWindowCondition>();
                    foreach (var con in edata.conditions)
                    {
                        RouterWindowCondition rcon = new RouterWindowCondition();
                        rcon.className = con.className;
                        rcon.nextWindow = FindWindow(con.nextWindowId);
                        conditions.Add(rcon);
                    }
                    win.SetConditions(conditions);
                }
                else if (item.windowType == NodeType.SubCanvas)
                {
                    CanvasWindowData edata = nodeCanvasData.subCanvaslist.Find(data => { return data.ID == item.Id; });

                    if (!string.IsNullOrEmpty(edata.nextWindowId))
                    {
                        BaseWindow next = FindWindow(edata.nextWindowId);

                        (item as SubCanvasWindow).SetNext(next);
                    }
                }
                else if (item.windowType == NodeType.Start)
                {
                    StartWindowData edata = nodeCanvasData.start;

                    if (!string.IsNullOrEmpty(edata.nextWindowId))
                    {
                        BaseWindow next = FindWindow(edata.nextWindowId);

                        (item as StartWindow).SetNext(next);
                    }
                }
            }
        }

        protected void generateSubData()
        {
            windowList = new List<BaseWindow>();

            windowList.Add(new SubStartWindow(Orgin, subNodeCanvasData.start, windowList));

            foreach (var item in subNodeCanvasData.nodelist)
            {
                windowList.Add(new SubNodeWindow(Orgin, item, windowList));
            }

            foreach (var item in subNodeCanvasData.routerlist)
            {
                windowList.Add(new SubRouterWindow(Orgin, item, windowList));
            }

            //set next Node
            foreach (var item in windowList)
            {
                if (item.windowType == NodeType.Node)
                {
                    NodeWindowData edata = subNodeCanvasData.nodelist.Find(data => { return data.ID == item.Id; });

                    if (!string.IsNullOrEmpty(edata.nextWindowId))
                    {
                        BaseWindow next = FindWindow(edata.nextWindowId);

                        (item as NodeWindow).SetNext(next);
                    }
                }
                else if (item.windowType == NodeType.Router)
                {
                    RouterWindowData edata = subNodeCanvasData.routerlist.Find(data => { return data.ID == item.Id; });
                    RouterWindow win = item as RouterWindow;

                    //set default
                    if (!string.IsNullOrEmpty(edata.nextWindowId))
                    {
                        BaseWindow def = FindWindow(edata.nextWindowId);
                        win.SetDefault(def);
                    }

                    //set conditions
                    List<RouterWindowCondition> conditions = new List<RouterWindowCondition>();
                    foreach (var con in edata.conditions)
                    {
                        RouterWindowCondition rcon = new RouterWindowCondition();
                        rcon.className = con.className;
                        rcon.nextWindow = FindWindow(con.nextWindowId);
                        conditions.Add(rcon);
                    }
                    win.SetConditions(conditions);
                }
                else if (item.windowType == NodeType.Start)
                {
                    StartWindowData edata = subNodeCanvasData.start;

                    if (!string.IsNullOrEmpty(edata.nextWindowId))
                    {
                        BaseWindow next = FindWindow(edata.nextWindowId);

                        (item as StartWindow).SetNext(next);
                    }
                }
            }
        }

        void toolbarFocus()
        {
            BaseWindow start = windowList.Find(baseWindow => { return baseWindow.windowType == NodeType.Start; });

            Vector2 center = Vector2.Scale(nodesArea.size, new Vector2(0.2f, 0.4f));

            Vector2 dis = center - start.position;

            foreach (var itemWindow in windowList)
            {
                itemWindow.position += dis;
            }
        }
    }
}
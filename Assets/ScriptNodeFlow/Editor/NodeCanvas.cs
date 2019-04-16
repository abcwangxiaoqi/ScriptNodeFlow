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

        protected SubNodeCanvasData subNodeCanvasData;

        //全部
        protected List<BaseWindow> windowList = null;

        protected InfoDataWindow infoDataWindow;
        protected SubCanvasListWindow subCanvasListWindow;
        protected SelectedWinWindow selectedWinWindow;

        protected CanvasType canvasType = CanvasType.Main;

        protected BaseWindow curSelect = null;

        protected BaseWindow connectWin = null;

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
        
        protected Rect rightArea;

        protected virtual void OnGUI()
        {   
            if(Event.current.type != EventType.Ignore)
            {
                drawRight();

                drawLeft();
            }
        }

        void drawLeft()
        {
            infoDataWindow.draw();
            selectedWinWindow.draw(curSelect);
            subCanvasListWindow.draw(position.height);     
        }

        void drawRight()
        {
            rightArea = CanvasLayout.Layout.GetNodeCanvasRect(position.size);
            GUILayout.BeginArea(rightArea, Styles.window);

            drawRightNodesArea();

            drawRightNavigation();

            GUILayout.EndArea();
        }

        void drawRightNavigation()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(canvasType == CanvasType.Main);

            if (GUILayout.Button("Back", EditorStyles.toolbarButton))
            {
                DelegateManager.Instance.Dispatch(DelegateCommand.SELECTMAINCANVAS);
            }

            EditorGUI.EndDisabledGroup();
            
            if (GUILayout.Button("Focus", EditorStyles.toolbarButton))
            {
                toolbarFocus();
            }
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            GUILayout.Label(canvasType == CanvasType.Main ? nodeCanvasData.name : subNodeCanvasData.name,
Styles.canvasTitleLabel);
        }

        void drawRightNodesArea()
        {
            if (windowList != null)
            {
                for (int i = 0; i < windowList.Count; i++)
                {
                    windowList[i].draw();
                }
            }
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
            infoDataWindow = new InfoDataWindow(nodeCanvasData.GetInstanceID(), nodeCanvasData.shareData);
            subCanvasListWindow = new SubCanvasListWindow(nodeCanvasData);
            selectedWinWindow = new SelectedWinWindow();
        }

        protected void generateMainData()
        {
            windowList = new List<BaseWindow>();

            windowList.Add(new StartWindow( nodeCanvasData.start, windowList));

            foreach (var item in nodeCanvasData.nodelist)
            {
                windowList.Add(new NodeWindow(item, windowList, nodeCanvasData.GetInstanceID()));
            }

            foreach (var item in nodeCanvasData.routerlist)
            {
                windowList.Add(new RouterWindow(item, windowList, nodeCanvasData.GetInstanceID()));
            }

            foreach (var item in nodeCanvasData.subCanvaslist)
            {
                windowList.Add(new SubCanvasWindow(item, windowList, nodeCanvasData));
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
                        rcon.ID = con.ID;
                        rcon.name = con.name;
                        rcon.nextWindow = FindWindow(con.nextWindowId);
                        rcon.updateClassName(nodeCanvasData.GetInstanceID(), win.Id, con.className);
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

            windowList.Add(new SubStartWindow(subNodeCanvasData.start, windowList));

            foreach (var item in subNodeCanvasData.nodelist)
            {
                windowList.Add(new SubNodeWindow(item, windowList, nodeCanvasData.GetInstanceID()));
            }

            foreach (var item in subNodeCanvasData.routerlist)
            {
                windowList.Add(new SubRouterWindow(item, windowList, nodeCanvasData.GetInstanceID()));
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
                        rcon.ID = con.ID;
                        rcon.name = con.name;
                        rcon.nextWindow = FindWindow(con.nextWindowId);
                        rcon.updateClassName(nodeCanvasData.GetInstanceID(), win.Id, con.className);
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

            Vector2 center = Vector2.Scale(rightArea.size, new Vector2(0.2f, 0.4f));

            Vector2 dis = center - start.position;

            foreach (var itemWindow in windowList)
            {
                itemWindow.position += dis;
            }
        }
    }
}
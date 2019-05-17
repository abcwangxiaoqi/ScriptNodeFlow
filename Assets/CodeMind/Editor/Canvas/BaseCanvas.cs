using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeMind
{
    public enum CanvasType
    {
        Main,
        Sub
    }

    public abstract class BaseCanvas : EditorWindow
    {
        protected static EditorWindow window;

        protected CodeMindData codeMindData;

        protected SubCanvasData subCanvasData;

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
            subCanvasData = objs[0] as SubCanvasData;
            canvasType = CanvasType.Sub;
            generateSubData();
            Repaint();
        }
            
        protected virtual void Update()
        {

        }

        private const float border = 5;
        
        protected Rect rightArea;

        protected virtual void OnGUI()
        {   
            if(Event.current.type != EventType.Ignore)
            {
                Rect rect = new Rect(Vector2.zero, position.size);
                GUILayout.BeginArea(rect,CanvasLayout.Layout.common.CanvasBgStyle);

                drawRight();

                drawLeft();

                GUILayout.EndArea();
            }
        }

        void drawLeft()
        {
            infoDataWindow.draw();
            
            //have to remain sort subCanvasListWindow->selectWinWindow
            //whether something is wrong
            subCanvasListWindow.draw(position.height);            
            selectedWinWindow.draw(curSelect);   
        }

        void drawRight()
        {
            rightArea = CanvasLayout.Layout.GetNodeCanvasRect(position.size);
            GUILayout.BeginArea(rightArea, CanvasLayout.Layout.common.window);

            drawRightNodesArea();

            drawRightNavigation();

            GUILayout.EndArea();
        }

        void drawRightNavigation()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            EditorGUI.BeginDisabledGroup(canvasType == CanvasType.Main);

            if (GUILayout.Button(CanvasLayout.Layout.canvas.BackBtContent, CanvasLayout.Layout.canvas.NavigationBtStyle))
            {
                DelegateManager.Instance.Dispatch(DelegateCommand.SELECTMAINCANVAS);
            }

            EditorGUI.EndDisabledGroup();
            
            if (GUILayout.Button(CanvasLayout.Layout.canvas.FocusBtContent, CanvasLayout.Layout.canvas.NavigationBtStyle))
            {
                toolbarFocus();
            }
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

            GUILayout.Label(canvasType == CanvasType.Main ? codeMindData.name : subCanvasData.name,
CanvasLayout.Layout.canvas.CanvasNamelabelStyle);
        }

        void drawRightNodesArea()
        {
            if (windowList != null)
            {

                foreach (var item in windowList)
                {
                    if (item.windowType != NodeType.Start)
                        continue;
                    item.draw();
                }

                //must keep subcanvas not lastest
                foreach (var item in windowList)
                {
                    if (item.windowType != NodeType.SubCanvas)
                        continue;
                    item.draw();
                }

                foreach (var item in windowList)
                {
                    if (item.windowType != NodeType.Node)
                        continue;
                    item.draw();
                }

                foreach (var item in windowList)
                {
                    if (item.windowType != NodeType.Router)
                        continue;
                    item.draw();
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
            infoDataWindow = new InfoDataWindow(codeMindData.ID, codeMindData);
            subCanvasListWindow = new SubCanvasListWindow(codeMindData);
            selectedWinWindow = new SelectedWinWindow(codeMindData.ID);
        }

        protected void generateMainData()
        {
            windowList = new List<BaseWindow>();

            windowList.Add(new StartWindow( codeMindData.start, windowList,codeMindData));

            foreach (var item in codeMindData.nodelist)
            {
                windowList.Add(new NodeWindow(item, windowList,codeMindData));
            }

            foreach (var item in codeMindData.routerlist)
            {
                windowList.Add(new RouterWindow(item, windowList, codeMindData));
            }

            foreach (var item in codeMindData.subCanvaslist)
            {
                windowList.Add(new SubCanvasWindow(item, windowList, codeMindData));
            }

            //set next Node
            foreach (var item in windowList)
            {
                if (item.windowType == NodeType.Node)
                {
                    NodeWindowData edata = codeMindData.nodelist.Find(data => { return data.ID == item.Id; });

                    if (!string.IsNullOrEmpty(edata.nextWindowId))
                    {
                        BaseWindow next = FindWindow(edata.nextWindowId);

                        (item as NodeWindow).SetNext(next);
                    }
                }
                else if (item.windowType == NodeType.Router)
                {
                    RouterWindowData edata = codeMindData.routerlist.Find(data => { return data.ID == item.Id; });
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
                        RouterWindowCondition rcon = new RouterWindowCondition(con);
                        rcon.nextWindow = FindWindow(con.nextWindowId);
                        conditions.Add(rcon);
                    }
                    win.SetConditions(conditions);
                }
                else if (item.windowType == NodeType.SubCanvas)
                {
                    CanvasWindowData edata = codeMindData.subCanvaslist.Find(data => { return data.ID == item.Id; });

                    if (!string.IsNullOrEmpty(edata.nextWindowId))
                    {
                        BaseWindow next = FindWindow(edata.nextWindowId);

                        (item as SubCanvasWindow).SetNext(next);
                    }
                }
                else if (item.windowType == NodeType.Start)
                {
                    StartWindowData edata = codeMindData.start;

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

            windowList.Add(new SubStartWindow(subCanvasData.start, windowList,codeMindData));

            foreach (var item in subCanvasData.nodelist)
            {
                windowList.Add(new SubNodeWindow(item, windowList,codeMindData));
            }

            foreach (var item in subCanvasData.routerlist)
            {
                windowList.Add(new SubRouterWindow(item, windowList, codeMindData));
            }

            //set next Node
            foreach (var item in windowList)
            {
                if (item.windowType == NodeType.Node)
                {
                    NodeWindowData edata = subCanvasData.nodelist.Find(data => { return data.ID == item.Id; });

                    if (!string.IsNullOrEmpty(edata.nextWindowId))
                    {
                        BaseWindow next = FindWindow(edata.nextWindowId);

                        (item as NodeWindow).SetNext(next);
                    }
                }
                else if (item.windowType == NodeType.Router)
                {
                    RouterWindowData edata = subCanvasData.routerlist.Find(data => { return data.ID == item.Id; });
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
                        RouterWindowCondition rcon = new RouterWindowCondition(con);
                        rcon.nextWindow = FindWindow(con.nextWindowId);
                        conditions.Add(rcon);
                    }
                    win.SetConditions(conditions);
                }
                else if (item.windowType == NodeType.Start)
                {
                    StartWindowData edata = subCanvasData.start;

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
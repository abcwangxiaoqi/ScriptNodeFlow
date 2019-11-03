using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeMind
{
    public abstract class BaseCanvas : EditorWindow
    {
        protected static Vector2 defaultPos = new Vector2(100, 100);
        protected static Vector2 defualtSize = new Vector2(1000, 800);

        internal CodeMindData codeMindData;

        //全部
        internal List<BaseWindow> windowList = null;

        protected InfoDataWindow infoDataWindow;

        protected BaseWindow curSelect = null;

        protected BaseWindow connectWin = null;

        protected bool initilizeFlag = false;

        public virtual void initilize(CodeMindData mindData)
        {
            codeMindData = mindData;
            initilizeFlag = true;
        }

        protected virtual void Awake()
        {
        }

        private const float border = 5;

        protected Rect rightArea;

        protected virtual void BeforeDraw()
        { }

        void Draw()
        {
            if (Event.current.type != EventType.Ignore)
            {
                drawRight();

                infoDataWindow.draw();
            }
        }

        void OnGUI()
        {
            if (!initilizeFlag)
                return;

            BeforeDraw();
            Draw();
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

            if (GUILayout.Button(CanvasLayout.Layout.canvas.FocusBtContent, CanvasLayout.Layout.canvas.NavigationBtStyle))
            {
                toolbarFocus();
            }
            GUILayout.FlexibleSpace();

            EditorGUILayout.EndHorizontal();

           // GUILayout.Label(codeMindData.name, CanvasLayout.Layout.canvas.CanvasNamelabelStyle);
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
                    if (item.windowType != NodeType.SubCodeMind)
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

        protected void generateData()
        {
            infoDataWindow = new InfoDataWindow(this);

            windowList = new List<BaseWindow>();

            windowList.Add(new StartWindow(codeMindData.start, this));

            foreach (var item in codeMindData.nodelist)
            {
                var attr = item.GetType().GetCustomAttributes(typeof(CodeMindNodeAttribute), false)[0] as CodeMindNodeAttribute;
                windowList.Add(new NodeWindow(attr,item, this));
            }

            foreach (var item in codeMindData.routerlist)
            {
                windowList.Add(new RouterWindow(item, this));
            }

            foreach (var item in codeMindData.subCodeMindlist)
            {
                windowList.Add(new SubCanvasWindow(item, this));
            }

            //set next Node
            foreach (var item in windowList)
            {
                if (item.windowType == NodeType.Node)
                {
                    Node edata = codeMindData.nodelist.Find(data => { return data.ID == item.Id; });

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
                        RouterWindowCondition rcon = new RouterWindowCondition(con, this);
                        rcon.nextWindow = FindWindow(con.nextWindowId);
                        conditions.Add(rcon);
                    }
                    win.SetConditions(conditions);
                }
                else if (item.windowType == NodeType.SubCodeMind)
                {
                    CodeMindWindowData edata = codeMindData.subCodeMindlist.Find(data => { return data.ID == item.Id; });

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

        const int GRID_SIZE = 15;
        //Draw a simple grid
        static void DrawGrid(Rect container, Vector2 offset, float zoomFactor)
        {

            if (Event.current.type != EventType.Repaint)
            {
                return;
            }

            Handles.color = new Color(0, 0, 0, 0.15f);

            var drawGridSize = zoomFactor > 0.5f ? GRID_SIZE : GRID_SIZE * 5;
            var step = drawGridSize * zoomFactor;

            var xDiff = offset.x % step;
            var xStart = container.xMin + xDiff;
            var xEnd = container.xMax;
            for (var i = xStart; i < xEnd; i += step)
            {
                Handles.DrawLine(new Vector3(i, container.yMin, 0), new Vector3(i, container.yMax, 0));
            }

            var yDiff = offset.y % step;
            var yStart = container.yMin + yDiff;
            var yEnd = container.yMax;
            for (var i = yStart; i < yEnd; i += step)
            {
                Handles.DrawLine(new Vector3(0, i, 0), new Vector3(container.xMax, i, 0));
            }

            Handles.color = Color.white;
        }
    }
}
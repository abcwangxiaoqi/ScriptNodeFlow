using EditorTools;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptNodeFlow
{
    public abstract class NodeCanvas : EditorWindow
    {
        protected static EditorWindow window;
        //全部
        protected List<BaseWindow> windowList = null;

        protected FixedWindow fixedWindow;


        protected virtual void Awake()
        {

        }

        protected virtual void Update()
        { }

        protected virtual void OnGUI()
        {
            // Note：GUI.Window must is between BeginWindows() and EndWindows()
            BeginWindows();

            if (fixedWindow != null)
            {
                fixedWindow.draw();
            }

            if (windowList != null)
            {
                for (int i = 0; i < windowList.Count; i++)
                {
                    windowList[i].draw();
                }
            }

            EndWindows();
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

        BaseWindow FindWindow(int id)
        {
            BaseWindow res = windowList.Find((BaseWindow w) =>
            {
                return w.Id == id;
            });
            return res;
        }

        T FindWindow<T>(int id)
            where T : BaseWindow
        {
            BaseWindow res = windowList.Find((BaseWindow w) =>
            {
                return w.Id == id;
            });

            if (res == null)
                return null;

            return res as T;
        }

        protected void generateWindowData(NodeCanvasData windowData)
        {
            windowList = new List<BaseWindow>();
            fixedWindow = new FixedWindow(windowData.shareData);
            
            windowList.Add(new StartWindow(windowData.start, windowList));

            foreach (var item in windowData.nodelist)
            {
                windowList.Add(new NodeWindow(item, windowList));
            }

            foreach (var item in windowData.routerlist)
            {
                windowList.Add(new RouterWindow(item, windowList));
            }

            foreach (var item in windowData.canvaslist)
            {
                windowList.Add(new CanvasWindow(item, windowList));
            }

            //set next Node
            foreach (var item in windowList)
            {
                if (item.windowType == NodeType.Node)
                {
                    NodeWindowData edata = windowData.nodelist.Find(data => { return data.ID == item.Id; });

                    if (edata.nextWindowId > 0)
                    {
                        BaseWindow next = FindWindow(edata.nextWindowId);

                        (item as NodeWindow).SetNext(next);
                    }
                }
                else if(item.windowType == NodeType.Router)
                {
                    RouterWindowData edata = windowData.routerlist.Find(data => { return data.ID == item.Id; });
                    RouterWindow win = item as RouterWindow;

                    //set default
                    if (edata.nextWindowId >= 0)
                    {
                        NodeWindow def = FindWindow<NodeWindow>(edata.nextWindowId);
                        win.SetDefault(def);
                    }

                    //set conditions
                    List<RouterWindowCondition> conditions = new List<RouterWindowCondition>();
                    foreach (var con in edata.conditions)
                    {
                        RouterWindowCondition rcon = new RouterWindowCondition();
                        rcon.className = con.className;
                        rcon.entity = FindWindow<NodeWindow>(con.nextWindowId);
                        conditions.Add(rcon);
                    }
                    win.SetConditions(conditions);
                }
                else if(item.windowType == NodeType.Canvas)
                {
                    CanvasWindowData edata = windowData.canvaslist.Find(data => { return data.ID == item.Id; });

                    if (edata.nextWindowId > 0)
                    {
                        BaseWindow next = FindWindow(edata.nextWindowId);

                        (item as CanvasWindow).SetNext(next);
                    }
                }
                else if (item.windowType == NodeType.Start)
                {
                    StartWindowData edata = windowData.start;

                    if (edata.nextWindowId > 0)
                    {
                        BaseWindow next = FindWindow(edata.nextWindowId);

                        (item as StartWindow).SetNext(next);
                    }
                }
            }
        }
    }
}





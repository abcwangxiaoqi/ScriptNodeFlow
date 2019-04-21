using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public enum State
    {
        Idle,
        Running,
        Error
    }

    public abstract class BaseWindow
    {
        protected static float connectPortSize = 12;
        protected static float connectPortOffset = 4;
        
        public Vector2 position { get; set; }
        protected abstract Vector2 size { get; }

        //whether be selected
        public bool selected { get; protected set; }

        public string describe;

        protected Rect windowRect;

        protected List<BaseWindow> windowList;
        public abstract NodeType windowType { get; }       

        public string Id { get; private set; }
        public string Name;

        public int parentRef { get; protected set; }

        public Vector2 In
        {
            get
            {
                return position + new Vector2(0, size.y / 2) + new Vector2(-connectPortSize+connectPortOffset, 0);
            }
        }

        public Vector2 Out
        {
            get
            {
                return position + new Vector2(size.x, size.y / 2);
            }
        }

        public Rect OutPortRect
        {
            get
            {
                Rect rect = new Rect();
                rect.position = position + new Vector2(size.x- connectPortOffset, (size.y- connectPortSize) / 2);
                rect.size = new Vector2(connectPortSize,connectPortSize);
                return rect;
            }
        }

        public Rect InPortRect
        {
            get
            {
                Rect rect = new Rect();
                rect.position = position + new Vector2(-connectPortSize+ connectPortOffset, (size.y - connectPortSize) / 2);
                rect.size = new Vector2(connectPortSize, connectPortSize);
                return rect;
            }
        }

        public BaseWindow( Vector2 pos, List<BaseWindow> _windowList)
        {
            position = pos;
            windowList = _windowList;

            Id = DateTime.Now.ToString("yyMMddHHmmssff");
        }

        protected WindowDataBase windowData { get; private set; }
      
        public BaseWindow(WindowDataBase _data, List<BaseWindow> _windowList)
        {
            windowData = _data;
            position = _data.position;
            windowList = _windowList;
            describe = _data.desc;
            Id = _data.ID;
            Name = _data.name;
        }

        public Rect selectRect
        {
            get
            {
                Rect rect = windowRect;
                rect.size += new Vector2(10, 10);
                rect.position -= new Vector2(5, 5);
                return rect;
            }
        }

        protected virtual void drawBefore()
        {}

        protected virtual void drawAfter()
        {
            
        }

        public void draw()
        {
            drawBefore();

            windowRect.position = position;
            windowRect.size = size;

            if (Application.isPlaying)
            {
                Rect rect = new Rect(windowRect.position + new Vector2(0, -30), new Vector2(size.x, 20));
                if (windowData.runtimeState == RuntimeState.Running)
                {
                    GUI.Label(rect, CanvasLayout.Layout.common.RunningLabelContent, CanvasLayout.Layout.common.RunningLabelStyle);
                }
                else if (windowData.runtimeState == RuntimeState.Error)
                {
                    GUI.Label(rect, CanvasLayout.Layout.common.ErrorLabelContent, CanvasLayout.Layout.common.ErrorLabelStyle);
                }
            }

            GUIContent c = new GUIContent(windowType.ToString(), describe);                 

            if(selected)
            {
               // GUI.Box(selectRect, "", CanvasLayout.Layout.canvas.SelectedWidnowsStyle);
                GUI.Box(selectRect, "", CanvasLayout.Layout.canvas.BaseWindowsStyle);
            }
            else
            {
                GUI.Box(selectRect, "", CanvasLayout.Layout.canvas.BaseWindowsStyle);
            }
             
            GUILayout.BeginArea(windowRect, c, CanvasLayout.Layout.canvas.WindowStyle);    

            drawWindowContent();

            GUILayout.EndArea();

            drawAfter();
        }

        protected virtual void drawWindowContent()
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            Name = GUILayout.TextField(Name, CanvasLayout.Layout.canvas.windowNameTextStyle);
            EditorGUI.EndDisabledGroup();
        }

        protected Vector2 GetOutPositionByPort(Rect rect)
        {
            return rect.position + new Vector2(connectPortSize, connectPortSize / 2);
        }

        public virtual void rightMouseClick(Vector2 mouseposition)
        {
        }

        public virtual void leftMouseDrag(Vector2 delta)
        {
             position += delta;
        }

        public virtual void leftMouseDoubleClick()
        {

        }

        public void Selected(bool select)
        {
            selected = select;
        }

        public bool isClick(Vector2 mouseposition)
        {
            return selectRect.Contains(mouseposition);
            //return windowRect.Contains(mouseposition);
        }

        public bool isClickInPort(Vector2 mouseposition)
        {
            return InPortRect.Contains(mouseposition);
        }

        public abstract WindowDataBase GetData();
        

        protected void DrawArrow(Vector2 from, Vector2 to, Color color)
        {
            Vector3 startPos = new Vector3(from.x, from.y, 0);
            Vector3 endPos = new Vector3(to.x, to.y, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;
            Handles.DrawBezier(startPos, endPos, startTan, endTan, color, null, 4);
        }

        public void SetParent(BaseWindow entity)
        {
            if(entity == null)
            {
                if(parentRef>0)
                {
                    parentRef--;
                }
            }
            else
            {
                parentRef++;
            }

        }
    }
}


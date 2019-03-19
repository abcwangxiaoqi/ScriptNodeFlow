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
        protected static GUIStyle NameTextStyle;
        protected static float connectPortSize = 12;
        protected static float connectPortOffset = 3;

        static BaseWindow()
        {
            NameTextStyle = new GUIStyle(UnityEditor.EditorStyles.textField);
            NameTextStyle.fixedHeight = 15;
            NameTextStyle.fontSize = 12;
            NameTextStyle.fontStyle = FontStyle.Bold;
            NameTextStyle.alignment = TextAnchor.MiddleCenter;
            NameTextStyle.normal.textColor = Color.white;
            NameTextStyle.focused.textColor = Color.white;
        }
        
        protected GUIStyle buttonStyle = EditorStyles.miniButton;
        protected GUIStyle popupStyle = EditorStyles.popup;
        
        public Vector2 position { get; set; }
        protected abstract Vector2 size { get; }

        protected Rect windowRect;

        protected List<BaseWindow> windowList;
        public abstract NodeType windowType { get; }

        public int Id { get; private set; }
        public string Name { get; protected set; }

        public BaseWindow parent { get; protected set; }

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

        public BaseWindow(string orgin, Vector2 pos, List<BaseWindow> _windowList)
        {
            Orgin = orgin;
            position = pos;
            windowList = _windowList;

            System.Random r = new System.Random(int.Parse(System.DateTime.Now.ToString("HHmmssfff")));
            Id = r.Next(1, int.MaxValue);
        }

        protected WindowDataBase windowData { get; private set; }
        protected string Orgin;
        public BaseWindow(string orgin,WindowDataBase _data, List<BaseWindow> _windowList)
        {
            Orgin = orgin;
            windowData = _data;
            position = _data.position;
            windowList = _windowList;

            Id = _data.ID;
            Name = _data.name;
        }

        public virtual void draw()
        {
            windowRect.position = position;
            windowRect.size = size;

            if (Application.isPlaying)
            {
                if (windowData.runtimeState == RuntimeState.Running)
                {
                    Rect rect = new Rect(windowRect.position + new Vector2(0, -30), new Vector2(size.x, 20));
                    //GUI.Label(rect, "Running...", BigLabelStyle);
                    GUI.Label(rect, "Running...");
                }
                else if (windowData.runtimeState == RuntimeState.Error)
                {
                    Rect rect = new Rect(windowRect.position + new Vector2(0, -30), new Vector2(size.x, 20));
                    //GUI.Label(rect, "Error", BigLabelStyle);
                    GUI.Label(rect, "Error");
                }
            }

            GUI.Window(Id, windowRect, gui, windowType.ToString());

            //GUILayout.BeginArea(windowRect, windowType.ToString(), GUI.skin.window);
            //gui(Id);
            //GUILayout.EndArea();
        }

        protected virtual void gui(int id)
        {
            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            Name = GUILayout.TextField(Name, NameTextStyle);
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

        public bool isClick(Vector2 mouseposition)
        {
            return windowRect.Contains(mouseposition);
        }

        public abstract WindowDataBase GetData();

        protected void DrawArrow2(Vector2 from, Vector2 to, Color color)
        {
            Handles.BeginGUI();
            Handles.color = color;
            Handles.DrawAAPolyLine(3, from, to);
            Vector2 v0 = from - to;
            v0 *= 10 / v0.magnitude;
            Vector2 v1 = new Vector2(v0.x * 0.866f - v0.y * 0.5f, v0.x * 0.5f + v0.y * 0.866f);
            Vector2 v2 = new Vector2(v0.x * 0.866f + v0.y * 0.5f, v0.x * -0.5f + v0.y * 0.866f); ;
            Handles.DrawAAPolyLine(3, to + v1, to, to + v2);
            Handles.EndGUI();
        }

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
            parent = entity;
        }
    }
}


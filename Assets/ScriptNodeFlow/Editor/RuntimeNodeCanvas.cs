using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class RuntimeNodeCanvas : NodeCanvas
    {
        static NodeController target;
        public static void Open(NodeController obj)
        {
            if (obj == null)
                return;

            if (window != null)
                window.Close();

            window = null;

            target = obj;
            window = GetWindow<RuntimeNodeCanvas>(string.Format("{0}({1})", obj.name, obj.nodeFlowData.name));
        }

        protected override void Awake()
        {
            base.Awake();

            EditorApplication.playModeStateChanged -= playModeStateChanged;
            EditorApplication.playModeStateChanged += playModeStateChanged;

            nodeCanvasData = target.nodeFlowData;

            generateLeftArea();

            generateMainData();
        }

        // close the window when playModeStateChanged
        private void playModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingPlayMode)
            {
                window.Close();
            }
        }

        Event curEvent;
        Vector2 mousePosition;

        protected override void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                Close();
            }

            curEvent = Event.current;

            if (rightArea.Contains(curEvent.mousePosition))
            {
                //must minus rightArea.position
                mousePosition = curEvent.mousePosition - rightArea.position;

                // mouse left key
                if (curEvent.button == 0 && curEvent.isMouse)
                {
                    //a window is whether selected
                    if (curEvent.type == EventType.MouseDown)
                    {
                        curSelect = windowList.Find((BaseWindow w) =>
                        {
                            return w.isClick(mousePosition);
                        });
                        if (curSelect != null
                            && curEvent.clickCount == 2)
                        {
                            if (curSelect != null)
                            {
                                curSelect.leftMouseDoubleClick();
                            }
                        }
                    }
                    else if (curEvent.type == EventType.MouseUp)
                    {
                        curSelect = null;
                    }
                    else if(curEvent.type == EventType.MouseDrag)
                    {
                        if (curSelect != null)
                        {
                            curSelect.leftMouseDrag(curEvent.delta);
                        }
                        else
                        {
                            if (rightArea.Contains(curEvent.mousePosition))
                            {
                                //drag the panel
                                foreach (var item in windowList)
                                {
                                    item.leftMouseDrag(curEvent.delta);
                                }
                            }
                        }
                    }
                }
                this.Repaint();
            }

            base.OnGUI();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }

}
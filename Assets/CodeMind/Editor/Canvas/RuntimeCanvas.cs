using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeMind
{
    public class RuntimeCanvas : BaseCanvas
    {
        static CodeMindController target;
        public static void Open(CodeMindController obj)
        {
            if (obj == null)
                return;

            if (window != null)
                window.Close();

            window = null;

            target = obj;
            window = GetWindow<RuntimeCanvas>(string.Format("{0}({1})", obj.name, obj.nodeFlowData.name));
        }

        protected override void Awake()
        {
            base.Awake();

            EditorApplication.playModeStateChanged -= playModeStateChanged;
            EditorApplication.playModeStateChanged += playModeStateChanged;

            codeMindData = target.nodeFlowData;

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
        bool clickArea;
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

                if (curEvent.button == 0 && curEvent.isMouse)
                {
                    //a window is whether selected
                    if (curEvent.type == EventType.MouseDown)
                    {
                        clickArea = true;

                        curSelect = windowList.Find((BaseWindow w) =>
                        {
                            return w.isClick(mousePosition);
                        });

                        if (curSelect != null)
                        {
                            curSelect.Selected(true);

                            foreach (var item in windowList)
                            {
                                if (item == curSelect)
                                    continue;

                                item.Selected(false);
                            }

                            if (curEvent.clickCount == 2)
                            {
                                curSelect.leftMouseDoubleClick();
                            }
                        }
                        else
                        {
                            GUI.FocusControl("");

                            foreach (var item in windowList)
                            {
                                item.Selected(false);
                            }
                        }

                    }
                    else if (curEvent.type == EventType.MouseUp)
                    {
                        clickArea = false;
                    }
                    else if (curEvent.type == EventType.MouseDrag)
                    {
                        if (curSelect != null)
                        {
                            curSelect.leftMouseDrag(curEvent.delta);
                        }
                        else if (clickArea)
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
                Repaint();
            }
            base.OnGUI();
        }

        protected override void OpenMainCanvas(object[] objs)
        {
            save();
            base.OpenMainCanvas(objs);
        }

        protected override void OpenSubCanvas(object[] objs)
        {
            save();

            base.OpenSubCanvas(objs);
        }

        private void save()
        {
            if (canvasType == CanvasType.Main)
            {
                saveMain();
            }
            else
            {
                saveSub();
            }
        }

        void saveMain()
        {
            for (int i = 0; i < windowList.Count; i++)
            {
                BaseWindow win = windowList[i];

                var cur = codeMindData.nodelist.Find((w) => { return win.Id == w.ID; });

                if(cur!=null)
                {
                    cur.position = win.position;
                    continue;
                }

                var router = codeMindData.routerlist.Find((w) => { return win.Id == w.ID; });

                if (router != null)
                {
                    router.position = win.position;
                    continue;
                }

                var sub = codeMindData.subCanvaslist.Find((w) => { return win.Id == w.ID; });

                if (sub != null)
                {
                    sub.position = win.position;
                    continue;
                }

                //set start windows position
                if(win.windowType == NodeType.Start)
                {
                    codeMindData.start.position = win.position;
                }
            }
        }

        void saveSub()
        {
            for (int i = 0; i < windowList.Count; i++)
            {
                BaseWindow win = windowList[i];

                var cur = subCanvasData.nodelist.Find((w) => { return win.Id == w.ID; });

                if (cur != null)
                {
                    cur.position = win.position;
                    continue;
                }

                var router = subCanvasData.routerlist.Find((w) => { return win.Id == w.ID; });

                if (router != null)
                {
                    router.position = win.position;
                    continue;
                }

                //set start windows position
                if (win.windowType == NodeType.Start)
                {
                    subCanvasData.start.position = win.position;
                }
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }

}
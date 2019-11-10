using EditorTools;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeMind
{
    internal class RuntimeCanvas : BaseCanvas
    {
        static Dictionary<string, RuntimeCanvas> windowMap = new Dictionary<string, RuntimeCanvas>();

        public static void Open(CodeMindData data)
        {
            string path = AssetDatabase.GetAssetPath(data);

            RuntimeCanvas window = null;

            Rect windowRect = new Rect(defaultPos, defualtSize);

            if (!windowMap.ContainsKey(path))
            {
                window = CreateInstance<RuntimeCanvas>();
                window.titleContent = new GUIContent(data.name);
                window.initilize(data);
                windowMap.Add(path, window);
            }
            else if (windowMap[path] == null)
            {
                window = CreateInstance<RuntimeCanvas>();
                window.titleContent = new GUIContent(data.name);
                window.initilize(data);
                windowMap[path] = window;
            }
            else
            {
                windowRect = windowMap[path].position;
            }

            windowMap[path].ShowUtility();
            windowMap[path].position = windowRect;
        }

        protected override void Awake()
        {
            base.Awake();


            EditorApplication.playModeStateChanged += playModeStateChanged;
        }

        public override void initilize(CodeMindData mindData)
        {
            base.initilize(mindData);

            generateData();
        }

        protected override void OnDestroy()
        {
            EditorApplication.playModeStateChanged -= playModeStateChanged;
        }

        // close the window when playModeStateChanged
        private void playModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingPlayMode)
            {
                this.Close();
            }
        }

        Event curEvent;
        Vector2 mousePosition;
        bool clickArea;

        protected override void BeforeDraw()
        {
            if (EditorApplication.isCompiling)
            {
                Close();
            }

            curEvent = Event.current;

            if (curEvent.clickCount > 0)
            {
                //must minus rightArea.position
                mousePosition = curEvent.mousePosition - rightArea.position;
            }

            if (curEvent.button == 0 && curEvent.isMouse
                     && rightArea.Contains(curEvent.mousePosition)
                     && GUIUtility.hotControl <= 0)
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
                else if (curEvent.type == EventType.MouseDrag
                         && clickArea)
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

            Repaint();
        }

        private void save()
        {
            saveMain();
        }

        void saveMain()
        {
            for (int i = 0; i < windowList.Count; i++)
            {
                BaseWindow win = windowList[i];

                var cur = codeMindData.nodelist.Find((w) => { return win.Id == w.ID; });

                if (cur != null)
                {
                    cur.winPos = win.position;
                    continue;
                }

                var router = codeMindData.routerlist.Find((w) => { return win.Id == w.ID; });

                if (router != null)
                {
                    router.position = win.position;
                    continue;
                }

                var sub = codeMindData.subCodeMindlist.Find((w) => { return win.Id == w.ID; });

                if (sub != null)
                {
                    sub.position = win.position;
                    continue;
                }

                //set start windows position
                if (win.windowType == NodeType.Start)
                {
                    codeMindData.start.position = win.position;
                }
            }
        }
    }

}
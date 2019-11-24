using EditorTools;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMind
{
    public class EditorCanvas : BaseCanvas
    {
        static Dictionary<string, EditorCanvas> windowMap = new Dictionary<string, EditorCanvas>();

        [MenuItem("Assets/Code Mind/Edit", true)]
        static bool ValidateSelection()
        {
            Object asset = Selection.activeObject;

            return (asset is CodeMindData) && !Application.isPlaying;
        }

        [MenuItem("Assets/Code Mind/Edit", false, priority = 49)]
        static void Edit()
        {
            Object asset = Selection.activeObject;

            Open(asset as CodeMindData);
        }

        [MenuItem("Assets/Code Mind/Create", false, priority = 49)]
        static void New()
        {
            EditorUtil.CreatAssetCurPath<CodeMindData>("New CodeMind Canvas");
        }

        public static void Open(CodeMindData data)
        {
            string path = AssetDatabase.GetAssetPath(data);

            EditorCanvas window = null;

            Rect windowRect = new Rect(defaultPos, defualtSize);

            if (!windowMap.ContainsKey(path))
            {
                window = CreateInstance<EditorCanvas>();
                window.titleContent = new GUIContent(data.name);
                window.initilize(data);
                windowMap.Add(path, window);
            }
            else if(windowMap[path] == null)
            {
                window = CreateInstance<EditorCanvas>();
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

       // static GUIContent addNode = new GUIContent("Add Node");
        static GUIContent addRouter = new GUIContent("Routers/Empty");
        static GUIContent addCanvas = new GUIContent("Add Canvas");
        static GUIContent comiling = new GUIContent("...Comiling...");
        



        string codeMindAssePath;
        protected override void Awake()
        {
            base.Awake();

            EditorApplication.playModeStateChanged += playModeStateChanged;
        }

        public override void initilize(CodeMindData mindData)
        {
            base.initilize(mindData);

            try
            {
                // quit unity editor when this window is active
                // reopen unity have to close this error window

                codeMindAssePath = AssetDatabase.GetAssetPath(codeMindData);

                assetKey = string.Format("NODEASSETPATH_{0}", codeMindData.GetInstanceID());

                updateCustoms();

                generateData();


            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(ex.Message);
                initilizeFlag = false;
                Close();
            }
        }

        //close window when playModeStateChanged
        private void playModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingEditMode ||
                obj == PlayModeStateChange.EnteredPlayMode)
            {
                Close();
            }
        }


        Event curEvent;
        Vector2 mousePosition;

        // the key of the asset's path
        // need be saved when compiling
        string assetKey = "NODEASSETPATH";


        bool clickArea = false;

        protected override void BeforeDraw()
        {
            if (EditorApplication.isCompiling)
            {
                ShowNotification(comiling);

                if (!EditorPrefs.HasKey(assetKey))
                {
                    EditorPrefs.SetString(assetKey, codeMindAssePath);

                    OnDestroy();
                }

                return;
            }

            if (EditorPrefs.HasKey(assetKey))
            {
                // once compiled
                string path = EditorPrefs.GetString(assetKey);
                EditorPrefs.DeleteKey(assetKey);

                codeMindData = AssetDatabase.LoadAssetAtPath<CodeMindData>(path);

                initilize(codeMindData);

                Awake();

                Repaint();
            }

            curEvent = Event.current;

            if (curEvent.clickCount > 0)
            {
                //must minus rightArea.position
                mousePosition = curEvent.mousePosition - rightArea.position;
            }

            if (curEvent.button == 1
                && rightArea.Contains(curEvent.mousePosition)) // mouse right key
            {
                RightMouseSelect();
            }
            else if (curEvent.button == 0 && curEvent.isMouse
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

                    connectWin = windowList.Find((BaseWindow w) =>
                    {
                        return w.isClickInPort(mousePosition);
                    });

                    m_DelegateManager.Dispatch(BaseCanvas.HANDLECONNECTPORT, connectWin);

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



        protected override void OnDestroy()
        {
            base.OnDestroy();

            EditorApplication.playModeStateChanged -= playModeStateChanged;

            EditorUtility.SetDirty(codeMindData);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            codeMindData.Compile();
        }

        private void RightMouseSelect()
        {
            GUI.FocusControl("");
            // add a new Node
            if (curEvent.type == EventType.MouseDown)
            {
                curSelect = windowList.Find((BaseWindow w) =>
                {
                    return w.isClick(mousePosition);
                });

                if (curSelect != null)
                {
                    if (curSelect.windowType == NodeType.Start)
                        return;

                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(CanvasLayout.Layout.canvas.DelWindowsContent, false, () =>
                    {
                        curSelect.deleteWindow();

                        if (curSelect.windowType == NodeType.Node)
                        {
                            codeMindData.RemoveNode(curSelect.windowData as NodeWindowProxy);
                        }
                        else if (curSelect.windowType == NodeType.Router)
                        {
                            codeMindData.routerlist.Remove(curSelect.windowData as RouterWindowProxy);
                        }
                        else if (curSelect.windowType == NodeType.SubCodeMind)
                        {
                            codeMindData.subCodeMindlist.Remove(curSelect.windowData as CodeMindWindowData);
                        }

                        windowList.Remove(curSelect);
                    });

                    menu.ShowAsContext();
                }
                else
                {
                    GenericMenu menu = new GenericMenu();
                    /*menu.AddItem(addNode, false, () =>
                    {
                        var node = codeMindData.AddNode(mousePosition);
                        windowList.Add(new NodeWindow(node, this));
                    });*/


                    menu.AddItem(addCanvas, false, () =>
                    {
                        var sub = codeMindData.AddSubCanvas(mousePosition);
                        windowList.Add(new SubCanvasWindow(sub, this));
                    });

                    foreach (var custom in customNodeStructList)
                    {
                        GUIContent content = new GUIContent(string.Format("Nodes/{0}", custom.attribute.viewText));
                            menu.AddItem(content, false, ()=>
                            {
                                var node = codeMindData.AddNode(custom.type, mousePosition,custom.attribute);
                                windowList.Add(new NodeWindow(node, this));
                            });
                    }

                    menu.AddItem(addRouter, false, () =>
                    {
                        var router = codeMindData.AddRouter(mousePosition);
                        windowList.Add(new RouterWindow(router, this));
                    });

                    foreach (var custom in customRouterStructList)
                    {
                        GUIContent content = new GUIContent(string.Format("Routers/{0}", custom.attribute.viewText));
                        menu.AddItem(content, false, () =>
                        {
                            var router = codeMindData.AddRouter(custom.type, mousePosition, custom.attribute);
                            windowList.Add(new RouterWindow(router, this));
                        });
                    }

                    menu.ShowAsContext();
                }
            }
        }
    }
}
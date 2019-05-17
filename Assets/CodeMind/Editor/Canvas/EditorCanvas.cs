using EditorTools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMind
{
    public class EditorCanvas : BaseCanvas
    {
        [MenuItem("Assets/Code Mind/Edit", true)]
        static bool ValidateSelection()
        {
            Object asset = Selection.activeObject;

            return (asset is CodeMindData);
        }

        [MenuItem("Assets/Code Mind/Edit", false, priority = 49)]
        static void Edit()
        {
            if (window != null)
                window.Close();

            window = null;


            Object asset = Selection.activeObject;
            scriptable = new ScriptableItem(AssetDatabase.GetAssetPath(asset));
            window = GetWindow<EditorCanvas>(asset.name);
        }

        [MenuItem("Assets/Code Mind/Create", false, priority = 49)]
        static void New()
        {
            EditorUtil.CreatAssetCurPath<CodeMindData>("New CodeMind Canvas");
        }

        public static void Open(Object obj)
        {
            if (window != null)
                window.Close();

            window = null;

            scriptable = new ScriptableItem(AssetDatabase.GetAssetPath(obj));
            window = GetWindow<EditorCanvas>(obj.name);
        }

        static ScriptableItem scriptable;

        static GUIContent addNode = new GUIContent("Add Node");
        static GUIContent addRouter = new GUIContent("Add Router");
        static GUIContent addCanvas = new GUIContent("Add Canvas");
        static GUIContent comiling = new GUIContent("...Comiling...");

        bool initilizeSuccess = true;

        protected override void Awake()
        {
            base.Awake();

            EditorApplication.playModeStateChanged -= playModeStateChanged;
            EditorApplication.playModeStateChanged += playModeStateChanged;

            try
            {
                // quit unity editor when this window is active
                // reopen unity have to close this error window

                codeMindData = scriptable.Load<CodeMindData>();

                generateLeftArea();

                generateMainData();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning(ex.Message);
                initilizeSuccess = false;
                Close();
            }
        }

        //close window when playModeStateChanged
        private void playModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingEditMode ||
                obj == PlayModeStateChange.EnteredPlayMode)
            {
                if (window != null)
                {
                    window.Close();
                }
            }
        }


        Event curEvent;
        Vector2 mousePosition;

        // the key of the asset's path
        // need be saved when compiling
        string nodeAssetPath = "NODEASSETPATH";


        bool clickArea = false;

        protected override void OnGUI()
        {
            if (EditorApplication.isCompiling)
            {
                ShowNotification(comiling);

                if (!EditorPrefs.HasKey(nodeAssetPath))
                {
                    EditorPrefs.SetString(nodeAssetPath, scriptable.path);

                    DelegateManager.Instance.Dispatch(DelegateCommand.OPENMAINCANVAS);

                    OnDestroy();
                }

                return;
            }

            if (EditorPrefs.HasKey(nodeAssetPath))
            {
                // once compiled
                string path = EditorPrefs.GetString(nodeAssetPath);
                EditorPrefs.DeleteKey(nodeAssetPath);

                scriptable = new ScriptableItem(path);

                window = this;

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

                    DelegateManager.Instance.Dispatch(DelegateCommand.HANDLECONNECTPORT, connectWin);

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

            base.OnGUI();
        }

        protected override void OnDestroy()
        {
            if (!initilizeSuccess)
                return;

            base.OnDestroy();

            save();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
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
                            codeMindData.nodelist.Remove(curSelect.windowData as NodeWindowData);
                        }
                        else if (curSelect.windowType == NodeType.Router)
                        {
                            codeMindData.routerlist.Remove(curSelect.windowData as RouterWindowData);
                        }
                        else if (curSelect.windowType == NodeType.SubCanvas)
                        {
                            codeMindData.subCanvaslist.Remove(curSelect.windowData as CanvasWindowData);
                        }

                        windowList.Remove(curSelect);
                    });

                    menu.ShowAsContext();
                }
                else
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(addNode, false, () =>
                    {
                        var node = codeMindData.AddNode(mousePosition);
                        windowList.Add(new NodeWindow(node, windowList,codeMindData));
                    });

                    menu.AddItem(addRouter, false, () =>
                    {
                        var router = codeMindData.AddRouter(mousePosition);
                        windowList.Add(new RouterWindow(router, windowList, codeMindData));
                    });

                    if (canvasType == CanvasType.Main)
                    {
                        menu.AddItem(addCanvas, false, () =>
                        {
                            var sub = codeMindData.AddSubCanvas(mousePosition);
                            windowList.Add(new SubCanvasWindow(sub, windowList, codeMindData));
                        });
                    }

                    menu.ShowAsContext();
                }
            }
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
            EditorUtility.SetDirty(codeMindData);
            //return;
            //codeMindData.nodelist.Clear();
            //codeMindData.routerlist.Clear();
            //codeMindData.subCanvaslist.Clear();

            //codeMindData.shareData = infoDataWindow.shareData;

            //for (int i = 0; i < windowList.Count; i++)
            //{
            //    if (windowList[i].windowType == NodeType.Node)
            //    {
            //        codeMindData.nodelist.Add((NodeWindowData)windowList[i].GetData());
            //    }
            //    else if (windowList[i].windowType == NodeType.Router)
            //    {
            //        codeMindData.routerlist.Add((RouterWindowData)windowList[i].GetData());
            //    }
            //    else if (windowList[i].windowType == NodeType.SubCanvas)
            //    {
            //        codeMindData.subCanvaslist.Add((CanvasWindowData)windowList[i].GetData());
            //    }
            //    else if (windowList[i].windowType == NodeType.Start)
            //    {
            //        codeMindData.start = (StartWindowData)windowList[i].GetData();
            //    }
            //}

            //EditorUtility.SetDirty(codeMindData);
        }

        void saveSub()
        {
            EditorUtility.SetDirty(codeMindData);

            //subCanvasData.nodelist.Clear();
            //subCanvasData.routerlist.Clear();

            //for (int i = 0; i < windowList.Count; i++)
            //{
            //    if (windowList[i].windowType == NodeType.Node)
            //    {
            //        subCanvasData.nodelist.Add((NodeWindowData)windowList[i].GetData());
            //    }
            //    else if (windowList[i].windowType == NodeType.Router)
            //    {
            //        subCanvasData.routerlist.Add((RouterWindowData)windowList[i].GetData());
            //    }
            //    else if (windowList[i].windowType == NodeType.Start)
            //    {
            //        subCanvasData.start = (StartWindowData)windowList[i].GetData();
            //    }
            //}

            //EditorUtility.SetDirty(codeMindData);
        }
    }
}
using EditorTools;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptNodeFlow
{
    public class EditorNodeCanvas : NodeCanvas
    {
        [MenuItem("Assets/Script Node Flow/Edit", true)]
        static bool ValidateSelection()
        {
            Object asset = Selection.activeObject;

            return (asset is NodeCanvasData);
        }

        [MenuItem("Assets/Script Node Flow/Edit", false, priority = 49)]
        static void Edit()
        {
            if (window != null)
                window.Close();

            window = null;


            Object asset = Selection.activeObject;
            scriptable = new ScriptableItem(AssetDatabase.GetAssetPath(asset));
            window = GetWindow<EditorNodeCanvas>(asset.name);
        }

        [MenuItem("Assets/Script Node Flow/Create", false, priority = 49)]
        static void New()
        {
            EditorUtil.CreatAssetCurPath<NodeCanvasData>("New Node Canvas");
        }

        public static void Open(Object obj)
        {
            if (window != null)
                window.Close();

            window = null;

            scriptable = new ScriptableItem(AssetDatabase.GetAssetPath(obj));
            window = GetWindow<EditorNodeCanvas>(obj.name);
        }

        static ScriptableItem scriptable;

        static GUIContent addNode = new GUIContent("Add Node");
        static GUIContent addRouter = new GUIContent("Add Router");
        static GUIContent addCanvas = new GUIContent("Add Canvas");
        static GUIContent comiling = new GUIContent("...Comiling...");

        protected override void Awake()
        {
            base.Awake();

            EditorApplication.playModeStateChanged -= playModeStateChanged;
            EditorApplication.playModeStateChanged += playModeStateChanged;

            nodeCanvasData = scriptable.Load<NodeCanvasData>();

            generateLeftArea();

            generateMainData();
        }

        //close window when playModeStateChanged
        private void playModeStateChanged(PlayModeStateChange obj)
        {
            if (obj == PlayModeStateChange.ExitingEditMode)
            {
                window.Close();
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

                Awake();

                Repaint();
            }

            curEvent = Event.current;

            if (rightArea.Contains(curEvent.mousePosition))
            {
                //must minus rightArea.position
                mousePosition = curEvent.mousePosition - rightArea.position;

                if (curEvent.button == 1) // mouse right key
                {
                    RightMouseSelect();
                }
                else if (curEvent.button == 0 && curEvent.isMouse)
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

        protected override void OnDestroy()
        {
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
                    curSelect.rightMouseClick(mousePosition);
                }
                else
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(addNode, false, () =>
                    {
                        windowList.Add(new NodeWindow(mousePosition, windowList, nodeCanvasData.GetInstanceID()));
                    });

                    menu.AddItem(addRouter, false, () =>
                    {
                        windowList.Add(new RouterWindow( mousePosition, windowList,nodeCanvasData.GetInstanceID()));
                    });

                    if (canvasType == CanvasType.Main)
                    {
                        menu.AddItem(addCanvas, false, () =>
                        {
                            windowList.Add(new SubCanvasWindow(mousePosition, windowList, nodeCanvasData));
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
            nodeCanvasData.nodelist.Clear();
            nodeCanvasData.routerlist.Clear();
            nodeCanvasData.subCanvaslist.Clear();

            nodeCanvasData.shareData = infoDataWindow.shareData;

            for (int i = 0; i < windowList.Count; i++)
            {
                if (windowList[i].windowType == NodeType.Node)
                {
                    nodeCanvasData.nodelist.Add((NodeWindowData)windowList[i].GetData());
                }
                else if (windowList[i].windowType == NodeType.Router)
                {
                    nodeCanvasData.routerlist.Add((RouterWindowData)windowList[i].GetData());
                }
                else if (windowList[i].windowType == NodeType.SubCanvas)
                {
                    nodeCanvasData.subCanvaslist.Add((CanvasWindowData)windowList[i].GetData());
                }
                else if (windowList[i].windowType == NodeType.Start)
                {
                    nodeCanvasData.start = (StartWindowData)windowList[i].GetData();
                }
            }

            EditorUtility.SetDirty(nodeCanvasData);
        }

        void saveSub()
        {
            subNodeCanvasData.nodelist.Clear();
            subNodeCanvasData.routerlist.Clear();

            for (int i = 0; i < windowList.Count; i++)
            {
                if (windowList[i].windowType == NodeType.Node)
                {
                    subNodeCanvasData.nodelist.Add((NodeWindowData)windowList[i].GetData());
                }
                else if (windowList[i].windowType == NodeType.Router)
                {
                    subNodeCanvasData.routerlist.Add((RouterWindowData)windowList[i].GetData());
                }
                else if (windowList[i].windowType == NodeType.Start)
                {
                    subNodeCanvasData.start = (StartWindowData)windowList[i].GetData();
                }
            }

            EditorUtility.SetDirty(nodeCanvasData);
        }
    }
}
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptNodeFlow
{
    public class SubCanvasListWindow
    {
        private NodeCanvasData mainData;

        List<SubNodeCanvasData> subCanvasList = new List<SubNodeCanvasData>();
        public SubCanvasListWindow(NodeCanvasData _mainData)
        {
            mainData = _mainData;

            Object[] subs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(mainData));

            foreach (var item in subs)
            {
                if (item is SubNodeCanvasData)
                {
                    subCanvasList.Add(item as SubNodeCanvasData);
                }
            }

            selectHash = mainData.GetInstanceID();

            DelegateManager.Instance.RemoveListener(DelegateCommand.OPENSUBCANVAS, openSubCanvas);
            DelegateManager.Instance.AddListener(DelegateCommand.OPENSUBCANVAS, openSubCanvas);

            DelegateManager.Instance.RemoveListener(DelegateCommand.SELECTMAINCANVAS, selectMainCanvas);
            DelegateManager.Instance.AddListener(DelegateCommand.SELECTMAINCANVAS, selectMainCanvas);
        }

        void openSubCanvas(object[] obj)
        {
            SubNodeCanvasData d = obj[0] as SubNodeCanvasData;
            selectHash = d.GetInstanceID();
        }

        void selectMainCanvas(params object[] obj)
        {
            selectHash = mainData.GetHashCode();
            DelegateManager.Instance.Dispatch(DelegateCommand.OPENMAINCANVAS);
        }

        private const float border = 5;
        Vector2 position = new Vector2(border, 150);
        private float height = 500;

        private Rect rect;

        private int selectHash;

        private string addSubName = string.Empty;

        private bool mainFlag = true;
        public void draw(Rect mainRect)
        {
            rect.position = position;
            rect.size = new Vector2(mainRect.width - 2 * border, height);

            GUILayout.BeginArea(rect);

            GUILayout.Label("CanvasList", Styles.titleLabel);

            EditorGUI.BeginDisabledGroup(selectHash == mainData.GetInstanceID());
            if (GUILayout.Button("MAIN"))
            {
                selectMainCanvas();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(10);

            for (int i = 0; i < subCanvasList.Count; i++)
            {
                GUILayout.BeginHorizontal();

                EditorGUI.BeginDisabledGroup(selectHash == subCanvasList[i].GetInstanceID());
                if (GUILayout.Button(subCanvasList[i].name,EditorStyles.miniButton))
                {
                    DelegateManager.Instance.Dispatch(DelegateCommand.OPENSUBCANVAS, subCanvasList[i]);
                }
                EditorGUI.EndDisabledGroup();

                if (!Application.isPlaying)
                {
                    if (GUILayout.Button("", Styles.miniDelButton))
                    {
                        if (selectHash == subCanvasList[i].GetInstanceID())
                        {
                            DelegateManager.Instance.Dispatch(DelegateCommand.OPENMAINCANVAS);
                        }

                        Object.DestroyImmediate(subCanvasList[i], true);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(mainData));

                        subCanvasList.RemoveAt(i);
                        i--;

                        continue;
                    }
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            addSubName = EditorGUILayout.TextField(addSubName);
            GUILayout.FlexibleSpace();

            if (!Application.isPlaying)
            {
                if (GUILayout.Button("New", EditorStyles.miniButton))
                {
                    if (!string.IsNullOrEmpty(addSubName))
                    {
                        SubNodeCanvasData sub = ScriptableObject.CreateInstance<SubNodeCanvasData>();
                        sub.name = addSubName;

                        AssetDatabase.AddObjectToAsset(sub, mainData);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(sub));

                        subCanvasList.Add(sub);

                        addSubName = "";

                        GUI.FocusControl("");
                    }
                }
            }
            
            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}

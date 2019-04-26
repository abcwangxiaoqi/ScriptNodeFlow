using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMind
{
    public class SubCanvasListWindow
    {
        private CodeMindData mainData;

        List<SubCanvasData> subCanvasList = new List<SubCanvasData>();
        public SubCanvasListWindow(CodeMindData _mainData)
        {
            mainData = _mainData;

            Object[] subs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(mainData));

            foreach (var item in subs)
            {
                if (item is SubCanvasData)
                {
                    subCanvasList.Add(item as SubCanvasData);
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
            SubCanvasData d = obj[0] as SubCanvasData;
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

        private int selectHash;

        private bool mainFlag = true;
        Vector2 scrollPostion;
        public void draw(float mainH)
        {
            Rect mainRect = CanvasLayout.Layout.GetCanvasListRect(mainH);
            GUILayout.BeginArea(mainRect, CanvasLayout.Layout.common.window);

            GUILayout.Label(CanvasLayout.Layout.sublist.TitleContent, CanvasLayout.Layout.common.WindowTitleStyle);

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();



            if (!Application.isPlaying
                && GUILayout.Button(CanvasLayout.Layout.sublist.AddSubBtContent, CanvasLayout.Layout.sublist.AddSubBtStyle, GUILayout.Width(13)))
            {
                SubCanvasData sub = ScriptableObject.CreateInstance<SubCanvasData>();

                string addSubName = "SubCanvas";

                int index = 1;

                while (subCanvasList.Exists((s) => { return s.name == (addSubName + index); }))
                {
                    index++;
                }

                addSubName += index;

                sub.name = addSubName;

                AssetDatabase.AddObjectToAsset(sub, mainData);
                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(sub));

                subCanvasList.Add(sub);

                DelegateManager.Instance.Dispatch(DelegateCommand.REFRESHSUBLIST);
            }

            EditorGUI.BeginDisabledGroup(selectHash == mainData.GetInstanceID());
            if (GUILayout.Button(CanvasLayout.Layout.sublist.MainBtContent, CanvasLayout.Layout.sublist.MainBtStyle))
            {
                selectMainCanvas();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            scrollPostion = GUILayout.BeginScrollView(scrollPostion, false,
                subCanvasList.Count * EditorGUIUtility.singleLineHeight + 50 > mainRect.size.y);

            for (int i = 0; i < subCanvasList.Count; i++)
            {
                GUILayout.BeginHorizontal();

                if (!Application.isPlaying)
                {
                    if (GUILayout.Button(CanvasLayout.Layout.sublist.DelSubBtContent, CanvasLayout.Layout.sublist.DelSubBtStyle))
                    {
                        if (selectHash == subCanvasList[i].GetInstanceID())
                        {
                            DelegateManager.Instance.Dispatch(DelegateCommand.OPENMAINCANVAS);
                        }

                        Object.DestroyImmediate(subCanvasList[i], true);
                        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(mainData));

                        DelegateManager.Instance.Dispatch(DelegateCommand.REFRESHSUBLIST);

                        subCanvasList.RemoveAt(i);
                        i--;

                        continue;
                    }
                }

                EditorGUI.BeginDisabledGroup(selectHash == subCanvasList[i].GetInstanceID());
                GUIContent content = new GUIContent(subCanvasList[i].name, "open the subcanvas");
                if (GUILayout.Button(content, CanvasLayout.Layout.sublist.OpenSubBtStyle))
                {
                    GUI.FocusControl("");
                    DelegateManager.Instance.Dispatch(DelegateCommand.OPENSUBCANVAS, subCanvasList[i]);
                }
                EditorGUI.EndDisabledGroup();

                GUILayout.EndHorizontal();

                if (EditorGUILayout.BeginFadeGroup(selectHash == subCanvasList[i].GetInstanceID() ? 1 : 0))
                {
                    EditorGUI.BeginDisabledGroup(Application.isPlaying);
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);

                    GUI.SetNextControlName(subCanvasList[i].name);
                    string tempName = EditorGUILayout.TextField(subCanvasList[i].name, CanvasLayout.Layout.sublist.SubNameTextStyle);
                    if (tempName != subCanvasList[i].name
                        && !string.IsNullOrEmpty(tempName)
                        && !subCanvasList.Exists((sub) => { return sub.name.Equals(tempName); }))
                    {
                        subCanvasList[i].name = tempName;
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    subCanvasList[i].desc = EditorGUILayout.TextArea(subCanvasList[i].desc, CanvasLayout.Layout.sublist.SubDesTextStyle, GUILayout.Height(100));
                    GUILayout.EndHorizontal();
                    EditorGUI.EndDisabledGroup();
                }
                EditorGUILayout.EndFadeGroup();
            }

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }
    }
}

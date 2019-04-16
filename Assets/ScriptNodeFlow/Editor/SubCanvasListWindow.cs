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
        
        private int selectHash;

        private bool mainFlag = true;
        public void draw(float mainH)
        {
            Rect mainRect = CanvasLayout.Layout.GetCanvasListRect(mainH);
            GUILayout.BeginArea(mainRect, Styles.window);

            GUILayout.Label("CanvasList", Styles.titleLabel);

            EditorGUI.BeginDisabledGroup(selectHash == mainData.GetInstanceID());
            if (GUILayout.Button("MAIN"))
            {
                selectMainCanvas();
            }
            EditorGUI.EndDisabledGroup();

            GUILayout.Space(5);

            if (!Application.isPlaying
                && GUILayout.Button("", Styles.addSubCanvasButton))
            {
                SubNodeCanvasData sub = ScriptableObject.CreateInstance<SubNodeCanvasData>();

                string addSubName = "SubCanvas";

                int index = 1;

                while(subCanvasList.Exists((s)=> { return s.name == (addSubName + index); }))
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

            GUILayout.Space(5);

            for (int i = 0; i < subCanvasList.Count; i++)
            {
                GUILayout.BeginHorizontal();

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

                        DelegateManager.Instance.Dispatch(DelegateCommand.REFRESHSUBLIST);

                        subCanvasList.RemoveAt(i);
                        i--;

                        continue;
                    }
                }

                EditorGUI.BeginDisabledGroup(selectHash == subCanvasList[i].GetInstanceID());
                if (GUILayout.Button(subCanvasList[i].name, EditorStyles.miniButton))
                {
                    DelegateManager.Instance.Dispatch(DelegateCommand.OPENSUBCANVAS, subCanvasList[i]);
                }
                EditorGUI.EndDisabledGroup();               

                GUILayout.EndHorizontal();

                if(selectHash == subCanvasList[i].GetInstanceID())
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    string tempName = EditorGUILayout.TextField(subCanvasList[i].name);
                    if(tempName!= subCanvasList[i].name
                        && !string.IsNullOrEmpty(tempName)
                        && !subCanvasList.Exists((sub) => { return sub.name.Equals(tempName); }))
                    {
                        subCanvasList[i].name = tempName;
                    }

                    GUILayout.EndHorizontal();

                    GUILayout.BeginHorizontal();
                    GUILayout.Space(20);
                    subCanvasList[i].desc = EditorGUILayout.TextArea(subCanvasList[i].desc, GUILayout.Height(100)); ;
                    GUILayout.EndHorizontal();
                }
            }            
            
            //GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}

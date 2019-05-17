using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMind
{
    public class InfoDataWindow
    {
        string ID;

        MonoScript monoScript;
        Editor editor;
        CodeMindData mindData;

        public InfoDataWindow(string id, CodeMindData codeMindData)
        {
            ID = id;
            mindData = codeMindData;

            if (mindData.shareData != null)
            {
                monoScript = MonoScript.FromScriptableObject(mindData.shareData);

                editor = Editor.CreateEditor(mindData.shareData);
            }
        }

        Vector2 scroll = Vector2.zero;

        public void draw()
        {
            GUILayout.BeginArea(CanvasLayout.Layout.info.rect, CanvasLayout.Layout.common.window);

            GUILayout.Label(CanvasLayout.Layout.info.TitleContent, CanvasLayout.Layout.common.WindowTitleStyle);

            var tempScript = EditorGUILayout.ObjectField("script", monoScript, typeof(MonoScript), false) as MonoScript;

            if (tempScript != null)
            {
                if (monoScript != tempScript)
                {
                    Type t = tempScript.GetClass();

                    if (t != null && t.IsSubclassOf(typeof(SharedData))
                    && !t.IsAbstract
                    && !t.IsInterface)
                    {
                        monoScript = tempScript;

                        var data = ScriptableObject.CreateInstance(t);
                        data.name = "ShareData";

                        AssetDatabase.AddObjectToAsset(data, mindData);
                        mindData.shareData = data as SharedData;

                        editor = Editor.CreateEditor(mindData.shareData);
                    }
                    else
                    {
                        monoScript = null;

                        Object.DestroyImmediate(mindData.shareData, true);

                        mindData.shareData = null;
                    }
                }
            }
            else
            {
                monoScript = null;

                Object.DestroyImmediate(mindData.shareData, true);

                mindData.shareData = null;
            }

            scroll = GUILayout.BeginScrollView(scroll);

            if (mindData.shareData != null)
            {
                editor.OnInspectorGUI();
            }

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }
    }
}
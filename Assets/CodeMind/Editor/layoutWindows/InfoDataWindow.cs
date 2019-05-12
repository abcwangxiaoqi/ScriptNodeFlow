using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMind
{
    public class InfoDataWindow
    {
        public SharedData shareData { get; private set; }

        string ID;

        MonoScript monoScript;
        Editor editor;

        public InfoDataWindow(string id,SharedData shareDataName)
        {
            ID = id;

            shareData = shareDataName;

            monoScript = MonoScript.FromScriptableObject(shareData);

            editor = Editor.CreateEditor(shareData);
        }

        Vector2 scroll = Vector2.zero;

        public void draw()
        {
            GUILayout.BeginArea(CanvasLayout.Layout.info.rect, CanvasLayout.Layout.common.window);

            GUILayout.Label(CanvasLayout.Layout.info.TitleContent, CanvasLayout.Layout.common.WindowTitleStyle);

            var tempScript = EditorGUILayout.ObjectField("script", monoScript, typeof(MonoScript), false) as MonoScript;

            if (tempScript != null)
            {
                if(monoScript != tempScript)
                {
                    Type t = tempScript.GetClass();

                    if (t!=null && t.IsSubclassOf(typeof(SharedData))
                    && !t.IsAbstract
                    && !t.IsInterface)
                    {
                        monoScript = tempScript;

                        shareData = ScriptableObject.CreateInstance(t) as SharedData;

                        editor = Editor.CreateEditor(shareData);
                    }
                    else
                    {
                        monoScript = null;
                        shareData = null;
                    }
                }
            }
            else
            {
                monoScript = null;
                shareData = null;
            }

            scroll = GUILayout.BeginScrollView(scroll);

            if (shareData != null)
            {
                editor.OnInspectorGUI();
            }

            GUILayout.EndScrollView();

            GUILayout.EndArea();
        }
    }
}
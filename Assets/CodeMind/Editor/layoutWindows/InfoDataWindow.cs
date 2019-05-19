using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMind
{
    public class InfoDataWindow
    {
        MonoScript monoScript;
        Editor editor;
        CodeMindData mindData;

        AnimBool scriptFadeGroup;

        public InfoDataWindow(CodeMindData codeMindData)
        {
            scriptFadeGroup = new AnimBool(true);

            mindData = codeMindData;



            if (mindData.shareData != null)
            {
                Debug.Log(">>>>>" + mindData.shareData.name);
                
                monoScript = MonoScript.FromScriptableObject(mindData.shareData);

                editor = Editor.CreateEditor(mindData.shareData);
            }
        }

        Vector2 scroll = Vector2.zero;

        public void draw()
        {
            GUILayout.BeginArea(CanvasLayout.Layout.info.rect, CanvasLayout.Layout.common.window);

            GUILayout.Label(CanvasLayout.Layout.info.TitleContent, CanvasLayout.Layout.common.WindowTitleStyle);

            var tempScript = EditorGUILayout.ObjectField(monoScript, typeof(MonoScript), false) as MonoScript;

            if(tempScript == null && tempScript != monoScript)
            {
                monoScript = tempScript;                
                Object.DestroyImmediate(mindData.shareData,true);
                mindData.shareData = null;
                editor = null;
            }
            else
            {
                if (tempScript != monoScript)
                {
                    monoScript = tempScript;

                    var type = monoScript.GetClass();

                    if (type != null
                        && type.IsSubclassOf(typeof(SharedData))
                        && !type.IsAbstract
                        && !type.IsInterface)
                    {
                        var data = ScriptableObject.CreateInstance(type);
                        data.name = "ShareData";

                        AssetDatabase.AddObjectToAsset(data, mindData);
                        mindData.shareData = data as SharedData;

                        editor = Editor.CreateEditor(mindData.shareData);
                    }
                }
            }     

            //scroll = GUILayout.BeginScrollView(scroll);

            if (editor != null)
            {
                scriptFadeGroup.target = EditorGUILayout.Foldout(scriptFadeGroup.target, "Parameters");

                if (EditorGUILayout.BeginFadeGroup(scriptFadeGroup.faded))
                {
                    EditorGUI.indentLevel++;

                    editor.OnInspectorGUI();

                    EditorGUI.indentLevel--;
                }
                EditorGUILayout.EndFadeGroup();
            }

            //GUILayout.EndScrollView();

            GUILayout.EndArea();

        }
    }
}
using System;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EditorCodeMind
{
    public class InfoDataWindow
    {
        MonoScript monoScript;
        Editor editor;
        CodeMindData mindData;

        AnimBool scriptFadeGroup;

        Rect rect;

        BaseCanvas MainCanvas;
        public InfoDataWindow(BaseCanvas canvas)
        {
            MainCanvas = canvas;
            scriptFadeGroup = new AnimBool(true);
            scriptFadeGroup.valueChanged.AddListener(canvas.Repaint);

            mindData = MainCanvas.codeMindData;

            if (mindData.shareData != null)
            {                
                monoScript = MonoScript.FromScriptableObject(mindData.shareData);

                editor = Editor.CreateEditor(mindData.shareData);
            }

            rect = CanvasLayout.Layout.info.rect;
          
        }

        Vector2 scroll = Vector2.zero;


        void selectShareData()
        {
            var list = MainCanvas.customSharedDataStructList;

            GenericMenu menu = new GenericMenu();

            foreach (var item in list)
            {
                GUIContent content = new GUIContent(item.attribute.viewText, item.attribute.name);
                menu.AddItem(content, false, () =>
                {
                    if(mindData.shareData!=null)
                    {
                        Object.DestroyImmediate(mindData.shareData, true);
                        mindData.shareData = null;
                    }
                    
                    var data = ScriptableObject.CreateInstance(item.type);
                    data.name = "ShareData";

                    AssetDatabase.AddObjectToAsset(data, mindData);
                    mindData.shareData = data as SharedData;

                    editor = Editor.CreateEditor(mindData.shareData);
                });
            }

            menu.ShowAsContext();
        }

        public void draw()
        {
            rect.x = MainCanvas.position.size.x - 15 - rect.size.x;

            GUILayout.BeginArea(rect, CanvasLayout.Layout.info.TitleContent,CanvasLayout.Layout.info.windowStyle);

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Mode",GUILayout.Width(60));
            mindData.mode = (PlayMode)EditorGUILayout.EnumPopup(mindData.mode,GUILayout.ExpandWidth(true));
            GUILayout.EndHorizontal();

            GUILayout.Space(3);

            if(GUILayout.Button("ShareData"))
            {
                selectShareData();
            }

            GUILayout.Space(3);

            /*var tempScript = EditorGUILayout.ObjectField(monoScript, typeof(MonoScript), false) as MonoScript;

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
            }     */



            if (editor != null)
            {
                
                scriptFadeGroup.target = EditorGUILayout.Foldout(scriptFadeGroup.target, "Parameters");

                if (EditorGUILayout.BeginFadeGroup(scriptFadeGroup.faded))
                {
                    //scroll = GUILayout.BeginScrollView(scroll, false, rect.size.y > 1000);

                    EditorGUI.indentLevel++;

                    editor.OnInspectorGUI();

                    EditorGUI.indentLevel--;

                    //GUILayout.EndScrollView();
                }
                EditorGUILayout.EndFadeGroup();

                GUILayout.Space(3);

                if(GUILayout.Button("Clear"))
                {
                    if (mindData.shareData != null)
                    {
                        Object.DestroyImmediate(mindData.shareData, true);
                        mindData.shareData = null;

                        editor = null;
                    }
                }
              
            }
            else
            {
                EditorGUILayout.LabelField(CanvasLayout.Layout.info.SharedScriptMessageContent, CanvasLayout.Layout.info.SharedScriptMessageStyle);
            }


                var lastrect = GUILayoutUtility.GetLastRect();

                if (lastrect.position != Vector2.zero)
                {
                    rect.size = new Vector2
                        (
                        CanvasLayout.Layout.info.rect.size.x,
                        lastrect.position.y + 30
                        );
                }

            GUILayout.EndArea();
        }
    }
}
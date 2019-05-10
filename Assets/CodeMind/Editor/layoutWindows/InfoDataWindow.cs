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

        public InfoDataWindow(string id,SharedData shareDataName)
        {
            ID = id;

            shareData = shareDataName;
        }

        MonoScript script;
        public void draw()
        {
            GUILayout.BeginArea(CanvasLayout.Layout.info.rect, CanvasLayout.Layout.common.window);

            var tempScript = EditorGUILayout.ObjectField("script", script, typeof(MonoScript), false) as MonoScript;

            if(tempScript!=null)
            {
                Type t = tempScript.GetClass();
                if (script != tempScript && t.IsSubclassOf(typeof(SharedData)))
                {
                    script = tempScript;
                }
            }
            


            GUILayout.Label(CanvasLayout.Layout.info.TitleContent, CanvasLayout.Layout.common.WindowTitleStyle);

            GUILayout.BeginHorizontal();

            GUILayout.Label(CanvasLayout.Layout.info.IDContent,CanvasLayout.Layout.common.TextTitleStyle);
            GUILayout.Label(ID, CanvasLayout.Layout.info.IDLabelStyle);

            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(CanvasLayout.Layout.info.CopyBtContent, CanvasLayout.Layout.common.CopyBtStyle))
            {
                EditorGUIUtility.systemCopyBuffer = string.Format(ShareDataBinding.Format, ID);
            }

            GUILayout.EndHorizontal();

            //GUILayout.BeginHorizontal();

            //GUILayout.Label(CanvasLayout.Layout.info.ShareDataContent,CanvasLayout.Layout.common.TextTitleStyle);

            //if(string.IsNullOrEmpty(shareData))
            //{
            //    GUILayout.Label(CanvasLayout.Layout.common.scriptRefNone, CanvasLayout.Layout.info.ShareDataErrorLabelStyle);
            //}
            //else
            //{
            //    GUILayout.Label(shareData, CanvasLayout.Layout.info.ShareDataLabelStyle);
            //}

            if (shareData == null)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(CanvasLayout.Layout.info.ShareDataContent, CanvasLayout.Layout.common.TextTitleStyle);
                GUILayout.Label(CanvasLayout.Layout.common.scriptRefNone, CanvasLayout.Layout.info.ShareDataErrorLabelStyle);
                GUILayout.EndHorizontal();
            }
            else
            {
                SerializedProperty sp = null;

                SerializedObject serializedObject = new SerializedObject(shareData);
                
                sp = serializedObject.GetIterator();

                while (sp.NextVisible(true))
                {
                    Debug.Log(">>>" + sp.name+">>>"+sp.propertyType);
                    if (sp.propertyType == SerializedPropertyType.String)
                    {
                        sp.stringValue = EditorGUILayout.TextField(sp.name, sp.stringValue);
                    }
                    else if(sp.propertyType == SerializedPropertyType.Integer)
                    {
                        sp.intValue = EditorGUILayout.IntField(sp.name, sp.intValue);
                    }
                    else if(sp.propertyType == SerializedPropertyType.AnimationCurve)
                    {
                        sp.animationCurveValue = EditorGUILayout.CurveField(sp.name, sp.animationCurveValue);
                    }
                    else if(sp.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        sp.objectReferenceValue = EditorGUILayout.ObjectField(sp.name, sp.objectReferenceValue, typeof(Object), true);
                    }
                }
                serializedObject.ApplyModifiedProperties();

                //GUILayout.Label(shareData, CanvasLayout.Layout.info.ShareDataLabelStyle);
            }


            //GUILayout.FlexibleSpace();

            //GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}
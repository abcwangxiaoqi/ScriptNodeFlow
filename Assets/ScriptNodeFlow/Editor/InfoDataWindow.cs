using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptNodeFlow
{
    public class InfoDataWindow
    {
        public string shareData { get; private set; }

        string ID;

        public InfoDataWindow(int id,string shareDataName)
        {
            ID = id.ToString();
            Assembly _assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp.dll");
            Type[] tys = _assembly.GetTypes();

            foreach (var item in tys)
            {
                if (item.IsSubclassOf(typeof(SharedData)) && !item.IsInterface && !item.IsAbstract)
                {
                    object[] bindings = item.GetCustomAttributes(typeof(BindingFlow), false);
                    if (bindings != null
                        && bindings.Length > 0
                        && (bindings[0] as BindingFlow).ID == ID)
                    {
                        shareData = item.FullName;
                        break;
                    }
                }
            }
        }        

        public void draw()
        {
            GUILayout.BeginArea(CanvasLayout.Layout.ShareDataRect, Styles.window);           

            GUILayout.Label("Info", Styles.titleLabel);

            GUILayout.BeginHorizontal();

            GUILayout.Label("ID:");
            GUILayout.Label(ID,Styles.subTitleLabel);

            GUILayout.FlexibleSpace();

            if(GUILayout.Button(GUIContents.copyID,Styles.copyButton))
            {
                EditorGUIUtility.systemCopyBuffer = ID;
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("ShareData:");

            if(string.IsNullOrEmpty(shareData))
            {
                GUILayout.Label(GUIContents.scriptRefNone,Styles.infoErrorLabel);
            }
            else
            {
                GUILayout.Label(shareData, Styles.subTitleLabel);
            }


            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}
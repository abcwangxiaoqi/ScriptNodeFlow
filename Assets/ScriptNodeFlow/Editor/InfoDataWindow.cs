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

        int ID;

        Type[] tys;
        public InfoDataWindow(int id,string shareDataName)
        {
            shareData = shareDataName;
            ID = id;
            Assembly _assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp.dll");
            tys = _assembly.GetTypes();
        }        

        public void draw()
        {
            GUILayout.BeginArea(CanvasLayout.Layout.ShareDataRect, Styles.window);            

            GUILayout.Label("Info", Styles.titleLabel);

            GUILayout.BeginHorizontal();

            GUILayout.Label("ID:", GUILayout.Width(25));
            GUILayout.Label(ID.ToString());

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label("ShareData:", GUILayout.Width(25));
            GUILayout.Label(string.IsNullOrEmpty(shareData) ? "None" : shareData);

             if(!Application.isPlaying && GUILayout.Button("", Styles.refreshButton))
            //if (GUILayout.Button(GUIContents.refresh,EditorStyles.miniButton))
            {
                refreshShareData();
            }

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }

        void refreshShareData()
        {            
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
    }
}
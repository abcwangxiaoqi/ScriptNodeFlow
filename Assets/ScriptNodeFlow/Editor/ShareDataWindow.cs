using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace ScriptNodeFlow
{
    public class ShareDataWindow
    {
        public string shareData { get; private set; }

        int shareDataIndex = 0;

        List<string> ShareDataList = new List<string>();
        
        public ShareDataWindow(string shareDataName)
        {
            Assembly _assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp.dll");
            Type[] tys = _assembly.GetTypes();

            ShareDataList = new List<string>() { "None" };
            foreach (var item in tys)
            {
                if (item.IsSubclassOf(typeof(SharedData)) && !item.IsInterface && !item.IsAbstract)
                {
                    ShareDataList.Add(item.FullName);
                }
            }

            if (!string.IsNullOrEmpty(shareDataName))
            {
                shareDataIndex = ShareDataList.IndexOf(shareDataName);
            }
        }

        private const float border = 5;
        Vector2 position = new Vector2(border, 30);
        private float height = 100;
        

        public void draw()
        {
            GUILayout.BeginArea(CanvasLayout.Layout.ShareDataRect, Styles.window);

            GUILayout.Label("ShareData", Styles.titleLabel);

            EditorGUI.BeginDisabledGroup(Application.isPlaying);
            shareDataIndex = EditorGUILayout.Popup(shareDataIndex, ShareDataList.ToArray());
            EditorGUI.EndDisabledGroup();

            if (shareDataIndex > 0)
            {
                shareData = ShareDataList[shareDataIndex];
            }
            else
            {
                shareData = null;
            }

            GUILayout.EndArea();
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace CodeMind
{
    public class InitializeOnLoad
    {
        static InitializeOnLoad()
        {
            //this is important, some error if cancel
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private static void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
            Selection.activeGameObject = null;
        }

        /// <summary>
        /// inspect every config when compling
        /// </summary>
        [InitializeOnLoadMethod]
        static void UpdateConfigWhenCompling()
        {
            List<string> list = PathHelper.getAllChildFiles("Assets/", ".asset");

            foreach (var item in list)
            {
                var data = AssetDatabase.LoadAssetAtPath<CodeMindData>(item);
                if (data == null)
                    continue;


                data.Compile();
            }
        }
    }
}

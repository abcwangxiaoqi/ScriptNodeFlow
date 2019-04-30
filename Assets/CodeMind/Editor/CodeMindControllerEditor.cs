using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace CodeMind
{
    [CustomEditor(typeof(CodeMindController))]
    public class CodeMindControllerEditor : Editor
    {
        CodeMindController Target;

        private void Awake()
        {
            Target = target as CodeMindController;
        }

        int selectIndex = 0;
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            GUILayout.Space(10);

            GUI.color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;


            //true : gameobject in Hierarchy
            //false : prefab
            bool isGameObject = Target.gameObject.activeInHierarchy;

            EditorGUI.BeginDisabledGroup(!(Application.isPlaying && isGameObject)); 
            
            if (GUILayout.Button("Graph"))
            {
                RuntimeCanvas.Open(Target);
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}

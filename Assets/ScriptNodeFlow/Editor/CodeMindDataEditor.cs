using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace CodeMind
{
    [CustomEditor(typeof(CodeMindData))]
    public class CodeMindDataEditor : Editor
    {
        CodeMindData data;
        private void Awake()
        {
            data = target as CodeMindData;
        }

        public override void OnInspectorGUI()
        {           
            //EditorGUILayout.LabelField("ID", data.GetInstanceID().ToString());
            EditorGUILayout.LabelField("ID", data.ID.ToString());
            //EditorGUILayout.LabelField("ID", data.GetHashCode().ToString());
            //data.GetHashCode

            EditorGUILayout.PrefixLabel("Desc");
            data.desc = GUILayout.TextArea(data.desc, GUILayout.Height(100));

            GUILayout.Space(10);
            
            GUI.color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;

            EditorGUI.BeginDisabledGroup(Application.isPlaying || EditorApplication.isCompiling);

            if (GUILayout.Button("OpenGraph", GUILayout.Height(50)))
            {
                EditorNodeCanvas.Open(target);
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
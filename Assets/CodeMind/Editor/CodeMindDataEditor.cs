using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace EditorCodeMind
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
            GUILayout.Label(CanvasLayout.Layout.AssetTitleContent, CanvasLayout.Layout.AssetTitleStyle);

            GUILayout.Space(20);

            //base.OnInspectorGUI();

            EditorGUILayout.PrefixLabel("Desc");
            data.desc = GUILayout.TextArea(data.desc, GUILayout.Height(100));

            GUILayout.Space(10);

            GUI.color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;

            EditorGUI.BeginDisabledGroup(Application.isPlaying || EditorApplication.isCompiling);

            if (GUILayout.Button("OpenGraph", GUILayout.Height(50)))
            {
                EditorCanvas.Open(data);
            }

            EditorGUI.EndDisabledGroup();
        }
    }
}
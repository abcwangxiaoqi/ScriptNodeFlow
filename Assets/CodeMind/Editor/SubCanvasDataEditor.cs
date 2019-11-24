using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace EditorCodeMind
{
    [CustomEditor(typeof(SubCanvasData))]
    public class SubCanvasDataEditor : Editor
    {
        SubCanvasData data;
        private void Awake()
        {
            data = target as SubCanvasData;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label("Desc");
            data.desc = GUILayout.TextArea(data.desc, GUILayout.Height(100));
        }
    }
}
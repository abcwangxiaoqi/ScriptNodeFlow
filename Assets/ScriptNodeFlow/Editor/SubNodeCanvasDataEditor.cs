using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace ScriptNodeFlow
{
    [CustomEditor(typeof(SubNodeCanvasData))]
    public class SubNodeCanvasDataEditor : Editor
    {
        SubNodeCanvasData data;
        private void Awake()
        {
            data = target as SubNodeCanvasData;
        }

        public override void OnInspectorGUI()
        {
            GUILayout.Label("Desc");
            data.desc = GUILayout.TextArea(data.desc, GUILayout.Height(100));
        }
    }
}
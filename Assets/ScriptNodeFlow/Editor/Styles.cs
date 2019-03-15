using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class Styles
    {
        public static GUISkin skin;
        static Styles()
        {
            //skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/ScriptNodeFlow/Editor/GUISkin.guiskin");
            skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/GUISkin.guiskin");
        }

        public static GUIStyle delButton
        {
            get { return skin.GetStyle("delButton"); }
        }

        public static GUIStyle miniDelButton
        {
            get { return skin.GetStyle("miniDelButton"); }
        }

        public static GUIStyle titleLabel
        {
            get { return skin.GetStyle("titleLabel"); }
        }

        public static GUIStyle winTitleLabel
        {
            get { return skin.GetStyle("winTitleLabel"); }
        }

        public static GUIStyle canvasTitleLabel
        {
            get { return skin.GetStyle("canvasTitleLabel"); }
        }
    }
}

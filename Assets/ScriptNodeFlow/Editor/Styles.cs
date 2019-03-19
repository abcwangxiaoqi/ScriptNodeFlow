using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class GUIContents
    {
        public static GUIContent add
        {
            get
            {
                return EditorGUIUtility.IconContent("SVN_AddedLocal");
            }
        }
    }
    
    public class Styles
    {
        public static GUISkin skin;
        static Styles()
        {
            if (EditorGUIUtility.isProSkin)
            {
                skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/GUISkinPro.guiskin");
            }
            else
            {
                skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/GUISkinPersonal.guiskin");
            }
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

        public static GUIStyle wrongLabel
        {
            get { return skin.GetStyle("wrongLabel"); }
        }

        public static GUIStyle rightLabel
        {
            get { return skin.GetStyle("rightLabel"); }
        }

        public static GUIStyle tipLabel
        {
            get { return skin.GetStyle("tipLabel"); }
        }

        public static GUIStyle tipErrorLabel
        {
            get { return skin.GetStyle("tipErrorLabel"); }
        }

        public static GUIStyle addButton
        {
            get { return skin.GetStyle("addButton"); }
        }

        public static GUIStyle winNameText
        {
            get { return skin.GetStyle("winNameText"); }
        }

        public static GUIStyle canvasArea
        {
            get { return skin.GetStyle("canvasArea"); }
        }

        public static GUIStyle connectBtn
        {
            get { return skin.GetStyle("connectBtn"); }
        }

        public static GUIStyle connectedBtn
        {
            get { return skin.GetStyle("connectedBtn"); }
        }

        public static GUIStyle nodeWindow
        {
            get { return skin.GetStyle("nodeWindow"); }
        }

        public static GUIStyle nodeWindowHeader
        {
            get { return skin.GetStyle("nodeWindowHeader"); }
        }
    }
}

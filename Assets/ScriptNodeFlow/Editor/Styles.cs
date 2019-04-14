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

        public static GUIContent refresh
        {
            get
            {
                return EditorGUIUtility.IconContent("Refresh");
            }
        }

        public static GUIContent copyID = new GUIContent("", "Copy the ID");

        public static GUIContent scriptRefNone = new GUIContent("script ref is none", "you need binding a script");
    }
    
    public class Styles
    {
        public static GUISkin skin;
        static Styles()
        {
            if (EditorGUIUtility.isProSkin)
            {
               // skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/GUISkinPro.guiskin");
                skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/GUISkinPersonal.guiskin");
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

        public static GUIStyle refreshButton
        {
            get { return skin.GetStyle("refreshButton"); }
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

        public static GUIStyle selectedBox
        {
            get { return skin.GetStyle("selectedBox"); }
        }

        public static GUIStyle expandButton
        {
            get { return skin.GetStyle("expandButton"); }
        }

        public static GUIStyle unexpandButton
        {
            get { return skin.GetStyle("unexpandButton"); }
        }

        public static GUIStyle copyButton
        {
            get { return skin.GetStyle("copyButton"); }
        }

        public static GUIStyle subTitleLabel
        {
            get { return skin.GetStyle("subTitleLabel"); }
        }

        public static GUIStyle nodeErrorLabel
        {
            get { return skin.GetStyle("nodeErrorLabel"); }
        }

        public static GUIStyle nodeClassNameLabel
        {
            get { return skin.GetStyle("nodeClassNameLabel"); }
        }

        public static GUIStyle infoErrorLabel
        {
            get { return skin.GetStyle("infoErrorLabel"); }
        }

        public static GUIStyle routerconditionErrorLabel
        {
            get { return skin.GetStyle("routerconditionErrorLabel"); }
        }

        public static GUIStyle routerconditionLabel
        {
            get { return skin.GetStyle("routerconditionLabel"); }
        }

        public static GUIStyle routerconditionNameLabel
        {
            get { return skin.GetStyle("routerconditionNameLabel"); }
        }

        public static GUIStyle routerconditionNameErrorLabel
        {
            get { return skin.GetStyle("routerconditionNameErrorLabel"); }
        }


        public static GUIStyle window
        {
            get { return skin.window; }
        }

        public static GUIStyle textField
        {
            get { return skin.textField; }
        }

        public static GUIStyle label
        {
            get { return skin.label; }
        }
    }
}

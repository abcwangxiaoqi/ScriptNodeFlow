using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

namespace ScriptNodeFlow
{   
    public class Util
    {
        static Type[] engineTypes = null;
        public static Type[] EngineTypes
        {
            get
            {
                if(engineTypes == null)
                {
                    Assembly _assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp.dll");
                    engineTypes = _assembly.GetTypes();
                }
                return engineTypes;
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
                skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/GUISkinPersonal.guiskin");
            }
            else
            {
                skin = AssetDatabase.LoadAssetAtPath<GUISkin>("Assets/GUISkinPersonal.guiskin");
            }
        }

        public static GUIStyle window
        {
            get { return skin.window; }
        }

        public static GUIStyle textField
        {
            get { return EditorStyles.textField; }
        }

        public static GUIStyle textArea
        {
            get { return EditorStyles.textArea; }
        }

        public static GUIStyle label
        {
            get { return EditorStyles.label; }
        }


        //-------------------specific----------------------
        static GUIStyle _windowNameText = null;
        public static GUIStyle windowNameText
        {
            get
            {
                if(_windowNameText == null)
                {
                    _windowNameText = new GUIStyle(UnityEditor.EditorStyles.textField);
                    _windowNameText.fontSize = 12;
                    _windowNameText.fontStyle = FontStyle.Bold;
                    _windowNameText.alignment = TextAnchor.MiddleCenter;

                    if(EditorGUIUtility.isProSkin)
                    {
                        _windowNameText.normal.textColor = Color.white;
                        _windowNameText.active.textColor = Color.white;
                    }
                    else
                    {
                        _windowNameText.normal.textColor = Color.white;
                        _windowNameText.focused.textColor = Color.white;
                    }
                    
                }
                return _windowNameText;
            }
        }


        static GUIStyle _conditionNameText = null;
        public static GUIStyle conditionNameText
        {
            get
            {
                if (_conditionNameText == null)
                {
                    _conditionNameText = new GUIStyle(UnityEditor.EditorStyles.textField);
                    _conditionNameText.fontSize = 12;
                    _conditionNameText.fontStyle = FontStyle.Bold;
                    _conditionNameText.alignment = TextAnchor.MiddleCenter;

                    if (EditorGUIUtility.isProSkin)
                    {
                        _conditionNameText.normal.textColor = Color.white;
                        _conditionNameText.active.textColor = Color.white;
                    }
                    else
                    {
                        _conditionNameText.normal.textColor = Color.white;
                        _conditionNameText.focused.textColor = Color.white;
                    }

                }
                return _conditionNameText;
            }
        }
    }
}

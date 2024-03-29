﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeMind
{
    public class InfoDataWindow
    {
        public string shareData { get; private set; }

        string ID;

        public InfoDataWindow(string id,string shareDataName)
        {
            ID = id;

            shareData = shareDataName;

            //if (Application.isPlaying)
            //{
            //    shareData = shareDataName;
            //}
            //else
            //{
            //    foreach (var item in Util.EngineTypes)
            //    {
            //        if (item.IsSubclassOf(typeof(SharedData)) && !item.IsInterface && !item.IsAbstract)
            //        {
            //            object[] bindings = item.GetCustomAttributes(typeof(ShareDataBinding), false);
            //            if (bindings != null
            //                && bindings.Length > 0
            //                && (bindings[0] as ShareDataBinding).CanvasID == ID)
            //            {
            //                shareData = item.FullName;
            //                break;
            //            }
            //        }
            //    }
            //}
        }        

        public void draw()
        {
            GUILayout.BeginArea(CanvasLayout.Layout.info.rect, CanvasLayout.Layout.common.window);

            
            GUILayout.Label(CanvasLayout.Layout.info.TitleContent, CanvasLayout.Layout.common.WindowTitleStyle);

            GUILayout.BeginHorizontal();

            GUILayout.Label(CanvasLayout.Layout.info.IDContent,CanvasLayout.Layout.common.TextTitleStyle);
            GUILayout.Label(ID, CanvasLayout.Layout.info.IDLabelStyle);

            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button(CanvasLayout.Layout.info.CopyBtContent, CanvasLayout.Layout.common.CopyBtStyle))
            {
                EditorGUIUtility.systemCopyBuffer = string.Format(ShareDataBinding.Format, ID);
            }

            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();

            GUILayout.Label(CanvasLayout.Layout.info.ShareDataContent,CanvasLayout.Layout.common.TextTitleStyle);

            if(string.IsNullOrEmpty(shareData))
            {
                GUILayout.Label(CanvasLayout.Layout.common.scriptRefNone, CanvasLayout.Layout.info.ShareDataErrorLabelStyle);
            }
            else
            {
                GUILayout.Label(shareData, CanvasLayout.Layout.info.ShareDataLabelStyle);
            }


            GUILayout.FlexibleSpace();

            GUILayout.EndHorizontal();

            GUILayout.EndArea();
        }
    }
}
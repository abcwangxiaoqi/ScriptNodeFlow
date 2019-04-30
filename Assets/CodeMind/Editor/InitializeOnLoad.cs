﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System;

namespace CodeMind
{
    public class InitializeOnLoad
    {
        static Assembly assembly;
        static Type[] types;
        static InitializeOnLoad()
        {

            assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp.dll");
            types = assembly.GetTypes();

            //this is important, some error if cancel
            EditorApplication.playModeStateChanged += EditorApplication_playModeStateChanged;
        }

        private static void EditorApplication_playModeStateChanged(PlayModeStateChange obj)
        {
            Selection.activeGameObject = null;
        }

        /// <summary>
        /// inspect every config when compling
        /// </summary>
        [InitializeOnLoadMethod]
        static void UpdateConfigWhenCompling()
        {
            List<string> list = PathHelper.getAllChildFiles("Assets/", ".asset");

            foreach (var item in list)
            {
                var data = AssetDatabase.LoadAssetAtPath<CodeMindData>(item);
                if (data == null)
                    continue;


                handleCfg(data);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// handle every single config
        /// </summary>
        /// <param name="data"></param>
        static void handleCfg(CodeMindData data)
        {
            handleShareData(data);

            foreach (var item in data.nodelist)
            {
                handleNode(item, data.ID);
            }

            List<RouterWindowData> routers = data.routerlist;
            foreach (var item in routers)
            {
                foreach (var condItem in item.conditions)
                {
                    handleCondition(condItem, data.ID,item.ID);
                }
            }

            foreach (var item in data.subCanvaslist)
            {
                handleSubCfg(item,data.ID);
            }
            
            EditorUtility.SetDirty(data);
        }

        /// <summary>
        /// handle every single sub canvas
        /// </summary>
        /// <param name="sub"></param>
        /// <param name="ID"></param>
        static void handleSubCfg(CanvasWindowData sub,string ID)
        {
            if (sub.canvasData == null)
                return;

            var data = sub.canvasData;
            foreach (var item in data.nodelist)
            {
                handleNode(item,ID);
            }

            List<RouterWindowData> routers = data.routerlist;
            foreach (var item in routers)
            {
                foreach (var condItem in item.conditions)
                {
                    handleCondition(condItem,ID,item.ID);
                }
            }
        }

        /// <summary>
        /// handle every single node
        /// </summary>
        /// <param name="node"></param>
        /// <param name="ID"></param>
        static void handleNode(NodeWindowData node,string ID)
        {
            string fullClassName = node.className;

            if (string.IsNullOrEmpty(fullClassName))
            {
                foreach (var item in types)
                {
                    if (item.IsSubclassOf(typeof(Node)) && !item.IsInterface && !item.IsAbstract)
                    {
                        object[] nodeBinings = item.GetCustomAttributes(typeof(NodeBinding), false);
                        if (nodeBinings != null
                            && nodeBinings.Length > 0
                            && (nodeBinings[0] as NodeBinding).WindowID == node.ID
                            && (nodeBinings[0] as NodeBinding).CanvasID == ID)
                        {
                            node.className = item.FullName;
                            break;
                        }
                    }
                }
                return;
            }

            var type = assembly.GetType(fullClassName);

            if(type == null)
            {
                node.className = string.Empty;
            }
            else
            {
                object[] nodeBinings = type.GetCustomAttributes(typeof(NodeBinding), false);
                if(nodeBinings == null || nodeBinings.Length == 0)
                {
                    node.className = string.Empty;
                }
                else if((nodeBinings[0] as NodeBinding).WindowID != node.ID
                    || (nodeBinings[0] as NodeBinding).CanvasID != ID)
                {
                    node.className = string.Empty;
                }
            }
        }

        /// <summary>
        /// handle every single condition of router
        /// </summary>
        /// <param name="condition"></param>
        /// <param name="ID"></param>
        /// <param name="routerID"></param>
        static void handleCondition(RouterWindowConditionData condition,string ID,string routerID)
        {
            string fullClassName = condition.className;

            if (string.IsNullOrEmpty(fullClassName))
            {
                foreach (var item in types)
                {
                    if (item.IsSubclassOf(typeof(RouterCondition)) && !item.IsInterface && !item.IsAbstract)
                    {
                        object[] routerBindings = item.GetCustomAttributes(typeof(RouterBinding), false);
                        if (routerBindings != null
                            && routerBindings.Length > 0
                            && (routerBindings[0] as RouterBinding).CanvasID == ID
                            && (routerBindings[0] as RouterBinding).RouterID == routerID
                            && (routerBindings[0] as RouterBinding).ConditionID == condition.ID)
                        {
                            condition.className = item.FullName;
                            break;
                        }
                    }
                }

                return;
            }

            var type = assembly.GetType(fullClassName);

            if (type == null)
            {
                condition.className = string.Empty;
            }
            else
            {
                object[] routerBinings = type.GetCustomAttributes(typeof(RouterBinding), false);
                if (routerBinings == null || routerBinings.Length == 0)
                {
                    condition.className = string.Empty;
                }
                else if ((routerBinings[0] as RouterBinding).ConditionID != condition.ID
                    || (routerBinings[0] as RouterBinding).RouterID != routerID
                    || (routerBinings[0] as RouterBinding).CanvasID != ID)
                {
                    condition.className = string.Empty;
                }
            }
        }

        /// <summary>
        /// handle shared data
        /// </summary>
        /// <param name="data"></param>
        static void handleShareData(CodeMindData data)
        {
            string fullClassName = data.shareData;

            if (string.IsNullOrEmpty(fullClassName))
            {
                foreach (var item in types)
                {
                    if (item.IsSubclassOf(typeof(SharedData)) && !item.IsInterface && !item.IsAbstract)
                    {
                        object[] bindings = item.GetCustomAttributes(typeof(ShareDataBinding), false);
                        if (bindings != null
                            && bindings.Length > 0
                            && (bindings[0] as ShareDataBinding).CanvasID == data.ID)
                        {
                            data.shareData = item.FullName;
                            break;
                        }
                    }
                }

                return;
            }                

            var type = assembly.GetType(fullClassName);

            if (type == null)
            {
                data.shareData = string.Empty;
            }
            else
            {
                object[] sharedataBinings = type.GetCustomAttributes(typeof(ShareDataBinding), false);

                if (sharedataBinings == null || sharedataBinings.Length == 0)
                {
                    data.shareData = string.Empty;
                }
                else if ((sharedataBinings[0] as ShareDataBinding).CanvasID != data.ID)
                {
                    data.shareData = string.Empty;
                }
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using System.Reflection;
using System;

namespace EditorCodeMind
{
    public class CustomNodeStruct:CustomStruct<NodeUsageAttribute>
    {
    }

    public class CustomRouterStruct:CustomStruct<RouterUsageAttribute>
    {
    }

    public class CustomConditionStruct:CustomStruct<ConditionUsageAttribute>
    {
        
    }

    public class CustomSharedDataStruct : CustomStruct<SharedDataUsageAttribute>
    {

    }

    public class CustomStruct<T>
        where T:Attribute
    {
        public Type type;

        public T attribute;
    }


    
    public sealed class Util
    {
        static Type[] engineTypes = null;
        public static Type[] EngineTypes
        {
            get
            {
                if (engineTypes == null)
                {
                    Assembly _assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp.dll");
                    engineTypes = _assembly.GetTypes();
                }
                return engineTypes;
            }
        }

        static Type[] editorTypes = null;
        public static Type[] EditorTypes
        {
            get
            {
                if (engineTypes == null)
                {
                    Assembly _assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp-Editor.dll");
                    editorTypes = _assembly.GetTypes();
                }
                return editorTypes;
            }
        }

        public static void LoadAllCusmtom(out List<CustomNodeStruct> customNodes
                                          ,out List<CustomRouterStruct> customRouters
                                          , out List<CustomConditionStruct> customConditions
                                          , out List<CustomSharedDataStruct> customSharedDatas)
        {
            customNodes = new List<CustomNodeStruct>();
            customRouters = new List<CustomRouterStruct>();
            customConditions = new List<CustomConditionStruct>();
            customSharedDatas = new List<CustomSharedDataStruct>();

            var types = EngineTypes;

            foreach (var ty in types)
            {

                if (ty.IsAbstract)
                    continue;
                


                if(ty.IsSubclassOf(typeof(NodeWindowProxy)))
                {
                    var serialattrs = ty.GetCustomAttributes(typeof(System.SerializableAttribute), false);
                    if (serialattrs == null || serialattrs.Length == 0)
                        continue;
                    
                    var attrs = ty.GetCustomAttributes(typeof(NodeUsageAttribute), false);

                    if (attrs == null || attrs.Length == 0)
                        continue;

                    var attr = attrs[0] as NodeUsageAttribute;

                    CustomNodeStruct custom = new CustomNodeStruct();
                    custom.type = ty;
                    custom.attribute = attr;
                    customNodes.Add(custom);
                }
                else if(ty.IsSubclassOf(typeof(RouterWindowProxy)))
                {
                    var serialattrs = ty.GetCustomAttributes(typeof(System.SerializableAttribute), false);
                    if (serialattrs == null || serialattrs.Length == 0)
                        continue;
                    
                    var attrs = ty.GetCustomAttributes(typeof(RouterUsageAttribute), false);

                    if (attrs == null || attrs.Length == 0)
                        continue;

                    var attr = attrs[0] as RouterUsageAttribute;

                    CustomRouterStruct custom = new CustomRouterStruct();
                    custom.type = ty;
                    custom.attribute = attr;
                    customRouters.Add(custom);
                }
                else if(ty.IsSubclassOf(typeof(RouterWindowConditionProxy)))
                {
                    var serialattrs = ty.GetCustomAttributes(typeof(System.SerializableAttribute), false);
                    if (serialattrs == null || serialattrs.Length == 0)
                        continue;
                    
                    var attrs = ty.GetCustomAttributes(typeof(ConditionUsageAttribute), false);

                    if (attrs == null || attrs.Length == 0)
                        continue;

                    var attr = attrs[0] as ConditionUsageAttribute;

                    CustomConditionStruct custom = new CustomConditionStruct();
                    custom.type = ty;
                    custom.attribute = attr;
                    customConditions.Add(custom);
                }
                else if(ty.IsSubclassOf(typeof(SharedData)))
                {
                    var attrs = ty.GetCustomAttributes(typeof(SharedDataUsageAttribute), false);

                    SharedDataUsageAttribute attr = null;
                    if(attrs == null || attrs.Length == 0)
                    {
                        attr = new SharedDataUsageAttribute(ty.Name, ty.FullName);
                    }
                    else
                    {
                        attr = attrs[0] as SharedDataUsageAttribute;
                    }

                    CustomSharedDataStruct custom = new CustomSharedDataStruct();
                    custom.type = ty;
                    custom.attribute = attr;
                    customSharedDatas.Add(custom);
                }
            }
        }
    }
}

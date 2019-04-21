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
                if (engineTypes == null)
                {
                    Assembly _assembly = Assembly.LoadFile("Library/ScriptAssemblies/Assembly-CSharp.dll");
                    engineTypes = _assembly.GetTypes();
                }
                return engineTypes;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class BindData
{
    public string className;
    public List<ParamatersBind> parmaters;
}

public class ParamatersBind
{
    public string name;
    public Type type;

    public int intValue;
    public float floatValue;
    public long longValue;
    public string stringValue;
    public double doubleValue;

    public Vector3 vector3Value;
    public Vector2 vector2Value;
    public Vector4 vector4Value;

    public Quaternion quaternionValue;

    public bool isObject = false;
    public Object ObjectValue;
}



using System;
using CodeMind;
using UnityEngine;


[CodeMindNode("myTestNode","myTestNode","myTestNode Des")]
public class myCustomNode:Node
{
    public string debugStr;

    public int num;

    public bool bo;
    public string str1;
    public string str2;
    public string str3;
    public string str4;
    public string str5;
    protected override void OnNodeCreate(SharedData sharedData)
    {
        base.OnNodeCreate(sharedData);

        Debug.Log(">>>OnNodeCreate " + debugStr);
    }

    protected override void OnNodeDestroy(SharedData sharedData)
    {
        base.OnNodeDestroy(sharedData);

        Debug.Log(">>>OnNodeDestroy " + debugStr);
    }

    protected override void OnNodePlay(SharedData sharedData)
    {
        base.OnNodePlay(sharedData);

        Debug.Log(">>>OnNodePlay " + debugStr);
    }

    protected override void OnNodeUpdate(SharedData sharedData)
    {
        base.OnNodeUpdate(sharedData);

        Debug.Log(">>>OnNodeUpdate " + debugStr);
    }
}

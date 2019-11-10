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
    protected override void OnInit(SharedData sharedData)
    {
        base.OnInit(sharedData);

        Debug.Log(">>>OnInit " + debugStr);
    }

    protected override void OnDestroy(SharedData sharedData)
    {
        base.OnDestroy(sharedData);

        Debug.Log(">>>OnDestroy " + debugStr);
    }

    protected override void OnPlay(SharedData sharedData)
    {
        base.OnPlay(sharedData);

        Debug.Log(">>>OnPlay " + debugStr);
    }

    protected override void OnProcessUpdate(SharedData sharedData)
    {
        base.OnProcessUpdate(sharedData);

        Debug.Log(">>>OnProcessUpdate " + debugStr);
    }
}

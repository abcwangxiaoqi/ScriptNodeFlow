using CodeMind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frankNode : Node
{
    public string str;

    [Range(0,10)]
    public int index;

    protected override void OnNodePlay(SharedData sharedData)
    {
        Debug.Log(">>>>" + str);

        moveNext();
    }


    protected override void OnNodeCreate(SharedData sharedData)
    {
        Debug.Log(">>>frankNode OnCreate");
    }

    protected override void OnNodeDestroy(SharedData sharedData)
    {
        Debug.Log(">>>frankNode OnDelete");
    }
}
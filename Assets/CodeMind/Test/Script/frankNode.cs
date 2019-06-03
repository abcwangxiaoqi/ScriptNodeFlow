using CodeMind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frankNode : Node
{
    public string str;

    [Range(0,10)]
    public int index;

    public override void OnPlay(SharedData sharedData)
    {
        Debug.Log(">>>>" + str);

        moveNext();
    }


    public override void OnCreate(SharedData sharedData)
    {
        Debug.Log(">>>frankNode OnCreate");
    }

    public override void OnObjectDestroy(SharedData sharedData)
    {
        Debug.Log(">>>frankNode OnDelete");
    }
}
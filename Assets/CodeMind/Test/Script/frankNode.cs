using CodeMind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class frankNode : Node
{
    public string str;

    [Range(0,10)]
    public int index;

    public override void Play(SharedData sharedData)
    {
        Debug.Log(">>>>" + str);

        finish();
    }
}
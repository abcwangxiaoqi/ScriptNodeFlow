using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMind;

public class myCondition : RouterCondition
{
    public string str;
    public int intValue;
    
    public override bool Justify(SharedData sharedData)
    {
        return false;
    }
}

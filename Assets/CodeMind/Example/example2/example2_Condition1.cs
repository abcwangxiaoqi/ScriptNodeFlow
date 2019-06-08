using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMind;

public class example2_Condition1 : RouterCondition
{
    public override bool Justify(SharedData shareData)
    {
        example2_SharedData data = shareData as example2_SharedData;

        return data.runMode == example2_SharedData.RunMode.Mode1;
    }
}

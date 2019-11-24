using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMind;

public class example2_Condition2 : RouterCondition
{
    public override bool Justify()
    {
        example2_SharedData data = m_SharedData as example2_SharedData;

        return data.runMode == example2_SharedData.RunMode.Mode2;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMind;

public class testCustomCondition1 : RouterCondition {

    public example2_SharedData.RunMode targetMode;

    public override bool Justify()
    {
        return GetSharedDate<example2_SharedData>().runMode == targetMode;
    }
}

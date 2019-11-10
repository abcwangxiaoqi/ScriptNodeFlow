using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMind;

public class testCustomCondition1 : RouterCondition {

    public override bool Justify(SharedData shareData)
    {
        return true;
    }
}

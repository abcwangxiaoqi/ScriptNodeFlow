using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMind;

[System.Serializable]
[RouterUsage("myRouter1","myRouterName","myRouterDes",false)]
public class testCustomRouterProxy : RouterWindowProxy {
    
    protected override void OnAssetCreate()
    {
        base.OnAssetCreate();


        AddPreCondition<testCustomConditionProxy>();
        AddPreCondition<testCustomConditionProxy>();
    }

}

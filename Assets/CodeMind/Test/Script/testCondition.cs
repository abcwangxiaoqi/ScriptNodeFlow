using CodeMind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RouterBinding("19042812312373", "19043017415025", "19043017415156")]
public class Condition1 : RouterCondition
{
    //public Condition1(SharedData data) : base(data) { }

    public override bool justify()
    {
        Debug.Log("Condition1");

        //get shared data
        testShareData data = shareData as testShareData;
        
        return data.state == 1;
    }
}

[RouterBinding("19042812312373", "19043017415025", "19043017415183")]
public class Condition2 : RouterCondition
{
    //public Condition2(SharedData data) : base(data) { }

    public override bool justify()
    {
        Debug.Log("Condition2");
        return (shareData as testShareData).state == 2;
    }
}

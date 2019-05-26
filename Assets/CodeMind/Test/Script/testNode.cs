using CodeMind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[NodeBinding("19042812312373", "19043017425885")]
public class StepOneEnity : Node
{
    //public StepOneEnity(SharedData data) : base(data) { }

    public override void Play(SharedData sharedData)
    {
        Debug.Log("Step One");

        finish();
    }

    public override void ProcessUpdate(SharedData sharedData)
    {
        base.ProcessUpdate(sharedData);

        /*
         * pre frame action
        */
    }
}

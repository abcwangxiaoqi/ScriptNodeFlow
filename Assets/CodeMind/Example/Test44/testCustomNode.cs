using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMind;

public class testCustomNode : Node {

    public string str;

    protected override void OnEnter()
    {
        Debug.Log(str);
        moveNext();
    }

}

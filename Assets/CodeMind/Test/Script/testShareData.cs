using CodeMind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ShareDataBinding("19042812312373")]
public class testShareData : SharedData
{
    public int state = 0;
    public string str = "sdf";
    public float f = 0;
    public Transform tran;
    public GameObject gameobject;

    public TTEE ttee;

}

[System.Serializable]
public class TTEE
{
    public string ttt;
}

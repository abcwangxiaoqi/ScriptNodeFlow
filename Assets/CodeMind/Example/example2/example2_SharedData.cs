using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMind;

public class example2_SharedData : SharedData 
{
    public enum RunMode
    {
        Mode1,
        Mode2,
        Mode3
    }

    public RunMode runMode = RunMode.Mode1;
}

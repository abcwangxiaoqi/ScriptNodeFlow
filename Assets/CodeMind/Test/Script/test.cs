using CodeMind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test:MonoBehaviour
{
    public CodeMindData mindData;


    CodeMindController controller;
    GameObject controllerGamobject;
    private void Awake()
    {
        controllerGamobject = mindData.Instantiate(out controller);
        controller.onFinish += Test_onFinish;
    }

    private void Test_onFinish()
    {
        Debug.Log(">>>>>Finish");
    }

}
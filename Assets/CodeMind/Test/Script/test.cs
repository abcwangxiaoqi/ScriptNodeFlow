using CodeMind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test:MonoBehaviour
{
    public CodeMindData mindData;

    public TestC GetC;

    public TestS testS;

    CodeMindController controller;
    GameObject controllerGamobject;
    private void Awake()
    {
        controllerGamobject = mindData.Instantiate(out controller);
        controller.onFinish += Test_onFinish;
    }

    private void Test_onFinish()
    {
        Debug.Log("Finish => success");
        Destroy(controllerGamobject);
    }

}

[System.Serializable]
public class TestC
{
    public string str;
}

[System.Serializable]
public struct TestS
{
    public string str;
}

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
    private void Awake()
    {
        controller = mindData.Instantiate();
        controller.onFinish += Test_onFinish;
    }

    private void Test_onFinish(bool obj)
    {
        Debug.Log("Finish => success:" + obj);
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

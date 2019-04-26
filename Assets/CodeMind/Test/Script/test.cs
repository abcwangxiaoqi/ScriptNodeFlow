using CodeMind;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test:MonoBehaviour
{
    public CodeMindData mindData;

    CodeMindController controller;
    private void Awake()
    {
        controller = mindData.Instantiate();
        controller.onFinish += Test_onFinish;
    }

    private void Test_onFinish(bool obj)
    {
        Debug.Log("State=>" + obj);
        //Destroy(controller.gameObject);
    }
}

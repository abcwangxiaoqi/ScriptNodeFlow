using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CodeMind;

public class example2_ChangeColor : Node 
{
    public Color color;

    protected override void OnNodePlay(SharedData sharedData)
    {
        base.OnNodePlay(sharedData);

        try
        {
            GameObject player = GameObject.Find("mainPlayer");

            var renderer = player.GetComponent<MeshRenderer>();
            renderer.sharedMaterial.color = color;
            moveNext();

        }
        catch (System.Exception ex)
        {
            moveNext(ex.Message);
        }
       
    }
}

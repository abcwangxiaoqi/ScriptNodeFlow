using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetModify : UnityEditor.AssetModificationProcessor
{
    static string[] OnWillSaveAssets(string[] paths)
    {
        //Debug.Log("OnWillSaveAssets");
        //foreach (string path in paths)
        //    Debug.Log(path);
        return paths;
    }
}

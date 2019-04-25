using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetModify : UnityEditor.AssetModificationProcessor
{
    /// <summary>
    /// detect the asset save action
    /// </summary>
    /// <param name="paths"></param>
    /// <returns></returns>
    static string[] OnWillSaveAssets(string[] paths)
    {
        return paths;
    }
}

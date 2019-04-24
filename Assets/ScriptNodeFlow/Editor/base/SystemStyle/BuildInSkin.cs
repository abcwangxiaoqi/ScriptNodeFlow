using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[Serializable]
public class BuildInSkin:ScriptableObject
{
    [MenuItem("Assets/GUISkin/Create BuildInSkin")]
    static void creat()
    {
        EditorUtil.CreatAssetCurPath<BuildInSkin>("BuildInSkin",
                                        (skin,map)=>
                                        {
            skin.gameSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Game);
            skin.sceneSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
            skin.inspectorSkin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
                                        });
    }
    
    public GUISkin sceneSkin;
    public GUISkin gameSkin;
    public GUISkin inspectorSkin;
}

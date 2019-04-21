using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class BuildInSkinEditor 
{

    [MenuItem("Assets/BuildInSkin/Get GameSkin Icon", false, priority = 46)]
    static void Game()
    {
        GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Game);
        createSkin(skin);
    }

    [MenuItem("Assets/BuildInSkin/Get SceneSkin Icon", false, priority = 45)]
    static void Scene()
    {
        GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Scene);
        createSkin(skin);
    }

    [MenuItem("Assets/BuildInSkin/Get InspectorSkin Icon", false, priority = 44)]
    static void Inspector()
    {
        GUISkin skin = EditorGUIUtility.GetBuiltinSkin(EditorSkin.Inspector);
        createSkin(skin);
    }

    static void createSkin(GUISkin skin)
    {
        List<Texture2D> textureList = GUISkinTool.GetAllTexture(skin);

        string fold = EditorUtility.OpenFolderPanel("save icons", "Assets", "");
        if (string.IsNullOrEmpty(fold))
            return;

        foreach (var item in textureList)
        {
            GUISkinTool.saveIcon2PNG(item, fold);
        }

        AssetDatabase.Refresh();
    }
}

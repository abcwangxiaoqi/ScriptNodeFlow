using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(BuildInSkin))]
public class BuildInSkinEditor:Editor
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

    static void createSkin(GUISkin skin,string fold)
    {
        List<Texture2D> textureList = GUISkinTool.GetAllTexture(skin);
        
        if (string.IsNullOrEmpty(fold))
            return;

        foreach (var item in textureList)
        {
            GUISkinTool.saveIcon2PNG(item, fold);
        }

       // AssetDatabase.Refresh();
    }

    BuildInSkin buildInSkin;

    string gameSkinFold;
    string sceneSkinFold;
    string inspectorFold;
    private void Awake()
    {
        buildInSkin = target as BuildInSkin;

        
        string root = AssetDatabase.GetAssetPath(buildInSkin);
        root = Path.GetDirectoryName(root);
        gameSkinFold = root + "/gameSkinIcon";
        sceneSkinFold = root + "/sceneSkinIcon";
        inspectorFold = root + "/inspectorIcon";
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(10);

        if(GUILayout.Button("Get Icons"))
        {
            createSkin(buildInSkin.gameSkin, gameSkinFold);
            createSkin(buildInSkin.sceneSkin, sceneSkinFold);
            createSkin(buildInSkin.inspectorSkin, inspectorFold);
            AssetDatabase.Refresh();
        }
    }
}

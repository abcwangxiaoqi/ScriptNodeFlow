using EditorTools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class EditorStyleTool:EditorWindow
{
    [MenuItem("Assets/EditorStyles/GetIcons",true)]
    static bool CanGetIcons()
    {
        return Selection.activeObject != null && Selection.activeObject is EditorStylesConfig;
    }

    [MenuItem("Assets/EditorStyles/GetIcons", false, priority = 47)]
    static void GetIcons()
    {
        EditorStylesConfig config = Selection.activeObject as EditorStylesConfig;

        List<Texture2D> textureList = new List<Texture2D>();

        foreach (var item in config.styles)
        {
            GUISkinTool.ParseStyle(item, textureList);
        }

        string fold = EditorUtility.OpenFolderPanel("save icons", "Assets", "");
        if (string.IsNullOrEmpty(fold))
            return;

        foreach (var item in textureList)
        {
            GUISkinTool.saveIcon2PNG(item, fold);
        }

        AssetDatabase.Refresh();
    }

    
    [MenuItem("Assets/EditorStyles/Creat")]
    static void Creat()
    {
        string path = EditorUtility.SaveFilePanelInProject("create editorstyles asset", "EditorStyles", "asset","");

        if (string.IsNullOrEmpty(path))
            return;

        List<GUIStyle> list = new List<GUIStyle>();
        foreach (PropertyInfo fi in typeof(EditorStyles).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            object o = fi.GetValue(null, null);
            if (o is GUIStyle)
            {
                list.Add(o as GUIStyle);
            }
        }

        ScriptableItem item = new ScriptableItem(path);
        item.Creat<EditorStylesConfig>((config, objects) => { config.styles = list; });
        item.Save();
        AssetDatabase.Refresh();
    }


    private static GUIStyle[] styles;
    [MenuItem("Assets/EditorStyles/Show")]
    static void OpenWindow()
    {
        List<GUIStyle> list = new List<GUIStyle>();
        foreach (PropertyInfo fi in typeof(EditorStyles).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            object o = fi.GetValue(null, null);
            if (o is GUIStyle)
            {
                list.Add(o as GUIStyle);
            }
        }

        styles = list.ToArray();

        GetWindow<EditorStyleTool>("EditorStyles");
    }

    Vector2 scrollPosition = Vector2.zero;
    void OnGUI()
    {
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);
        GUILayout.BeginVertical();
        for (int i = 0; i < styles.Length; i++)
        {
            GUILayout.Label(styles[i].name, styles[i]);
        }
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
    }
}
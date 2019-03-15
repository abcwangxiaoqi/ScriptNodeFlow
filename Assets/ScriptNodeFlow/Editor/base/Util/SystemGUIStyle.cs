using EditorTools;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

public class SystemGUIStyle:EditorWindow
{
    private const string Path = "Assets/SystemGUIStyle.guiskin";

    [MenuItem("Tools/SystemGUIStyle/Creat")]
    static void Creat()
    {
        List<GUIStyle> styles = new List<GUIStyle>();
        foreach (PropertyInfo fi in typeof(EditorStyles).GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        {
            object o = fi.GetValue(null, null);
            if (o.GetType() == typeof(GUIStyle))
            {
                styles.Add(o as GUIStyle);
            }
        }

        ScriptableItem item = new ScriptableItem(Path);
        item.Creat<GUISkin>((skin, objects) => { skin.customStyles = styles.ToArray(); });
        item.Save();
        AssetDatabase.Refresh();
    }

    private static GUIStyle[] styles;
    [MenuItem("Tools/SystemGUIStyle/Show")]
    static void OpenWindow()
    {
        ScriptableItem item = new ScriptableItem(Path);
        GUISkin skin = item.Load<GUISkin>();
        if (null == skin)
        {
            Creat();
            skin= item.Load<GUISkin>();
        }

        styles = skin.customStyles;
        GetWindow<SystemGUIStyle>("SystemGUIStyle");
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
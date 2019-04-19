using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(menuName = "CanvasLayout/Create")]
public class CanvasLayout : ScriptableObject
{
    public Rect GetCanvasListRect(float mainHeight)
    {
        float h = mainHeight
            - selected.rect.position.y - selected.rect.size.y
            - 10
            - 20;
        sublist.rect.size = new Vector2(sublist.rect.size.x, h);

        return sublist.rect;
    }

    public Rect GetNodeCanvasRect(Vector2 mainSize)
    {
        float h = mainSize.y
            - 10
            - 10;
        float w = mainSize.x - 5 - 200 - 10 - 5;

        canvas.rect.size = new Vector2(w, h);

        return canvas.rect;
    }

    static CanvasLayout layout;
    public static CanvasLayout Layout
    {
        get
        {
            if (layout == null)
            {
                layout = AssetDatabase.LoadAssetAtPath<CanvasLayout>
                    ("Assets/ScriptNodeFlow/Editor/CanvasLayout.asset");
            }
            return layout;
        }
    }

    [Space(10)]
    [Header("Common Config")]
    public CommonCfg common = new CommonCfg();


    [Space(10)]
    [Header("Info Windows Config")]
    public InfoWindowCfg info = new InfoWindowCfg();

    [Space(10)]
    [Header("Actived Windows Config")]
    public SelectedWindowCfg selected = new SelectedWindowCfg();

    [Space(10)]
    [Header("Sub Canvas Config")]
    public SubListWindowCfg sublist = new SubListWindowCfg();

    [Space(10)]
    [Header("Canvas Config")]
    public CanvasCfg canvas = new CanvasCfg();
}

[Serializable]
public class CommonCfg
{
    public GUIStyle WindowTitleStyle = new GUIStyle();
    //public GUIStyle window = GUI.skin.window;
    public GUIStyle CopyBtStyle = new GUIStyle();

    public GUIContent scriptRefNone = new GUIContent("script ref is none", "you need binding a script");
}

[Serializable]
public class SubListWindowCfg
{
    [Header("Rect")]
    public Rect rect;


    [Header("Style")]
    public GUIContent TitleContent = new GUIContent("SubCanvasList");

    public GUIContent MainBtContent = new GUIContent("MAIN");

    public GUIContent AddSubBtContent = new GUIContent("", "add a sub canvas");
    public GUIStyle AddSubBtStyle = new GUIStyle();

    public GUIContent DelSubBtContent = new GUIContent("");
    public GUIStyle DelSubBtStyle = new GUIStyle();

    // public GUIStyle OpenSubBtStyle = new GUIStyle(EditorStyles.miniButton);

    public GUIStyle SubNameTextStyle = new GUIStyle();

    // public GUIStyle SubDesTextStyle = new GUIStyle(EditorStyles.textArea);

}

[Serializable]
public class SelectedWindowCfg
{
    [Header("Rect")]
    public Rect rect;

    [Header("Style")]
    public GUIContent TitleContent = new GUIContent("Current");

    public GUIContent IDContent = new GUIContent("ID:");
    public GUIStyle IDLabelStyle = new GUIStyle();

    public GUIContent CopyBtContent = new GUIContent("", "Copy the ID");


    public GUIContent TypeContent = new GUIContent("Type:");
    public GUIStyle TypeLabelStyle = new GUIStyle();

    public GUIContent NameContent = new GUIContent("Name:");

    public GUIContent DesContent = new GUIContent("Describe:");
}

[Serializable]
public class InfoWindowCfg
{
    [Header("Info Rect")]
    public Rect rect;

    [Header("Style")]
    public GUIContent TitleContent = new GUIContent("Info");

    public GUIContent IDContent = new GUIContent("ID:");
    public GUIStyle IDLabelStyle = new GUIStyle();

    public GUIContent ShareDataContent = new GUIContent("ShareData:");
    public GUIStyle ShareDataLabelStyle = new GUIStyle();

    public GUIStyle ShareDataErrorLabelStyle = new GUIStyle();

    public GUIContent CopyBtContent = new GUIContent("", "copy the id");
}

[Serializable]
public class CanvasCfg
{
    [Header("Rect")]
    public Rect rect;

    [Header("Line")]
    public Color lineColor = Color.white;
    public Color runtimelineColor = Color.white;

    [Header("Style")]
    public GUIContent BackBtContent = new GUIContent("Back", "back to the main canvas");
    public GUIContent FocusBtContent = new GUIContent("Focuse");
    public GUIStyle CanvasNamelabelStyle = new GUIStyle();

    [Header("Common")]
    public GUIContent DelWindowsContent = new GUIContent("Delete", "delete the windows");
    public GUIStyle ConnectedBtStyle = new GUIStyle();
    public GUIStyle ConnectBtStyle = new GUIStyle();

    [Space(10)]
    [Header("Start Window")]
    public GUIContent StartContent = new GUIContent("Start");
    public GUIStyle StartLabelStyle = new GUIStyle();

    [Space(10)]
    [Header("Node Window")]
    public GUIStyle NodeRefNameLabelStyle = new GUIStyle();
    public GUIStyle NodeRefErrorLabelStyle = new GUIStyle();

    [Space(10)]
    [Header("SubCanvas Window")]
    public GUIContent SubCanvasNoneContent = new GUIContent("no subcanvas", "you must select a valid subcanvas");
    public GUIStyle SubCanvasErrorLabel = new GUIStyle();

    [Space(10)]
    [Header("Router Window")]
    public GUIContent DefaultContent = new GUIContent("Defualt", "router default condition");
    public GUIStyle DefaultLabelStyle = new GUIStyle();
    public GUIContent DefaultErrorContent = new GUIContent("Defualt", "router default link is none");
    public GUIStyle DefaultErrorLabelStyle = new GUIStyle();

    public GUIContent AddConditionContent = new GUIContent("", "add a new condition");

    public GUIContent DelConditionContent = new GUIContent("", "delete this condition");
    public GUIStyle DelConditionStyle = new GUIStyle();
    public GUIStyle ConditionNameText = new GUIStyle();
    public GUIContent ConditionExpandBtContent = new GUIContent("","expand the plane");
    public GUIStyle ConditionExpandBtStyle = new GUIStyle();
    public GUIContent ConditionUnexpandBtContent = new GUIContent("","unexpand the plane");
    public GUIStyle ConditionUnexpandBtStyle = new GUIStyle();

    public GUIStyle IDLabelStyle = new GUIStyle();

    public GUIContent CopyBtContent = new GUIContent("", "copy current message");

    public GUIStyle ConditionErrorLabelStyle = new GUIStyle();
    public GUIStyle ConditionLabelStyle = new GUIStyle();
}

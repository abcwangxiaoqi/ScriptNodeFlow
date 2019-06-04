using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CreateAssetMenu(menuName = "CanvasLayout/Create")]
public class CanvasLayout : ScriptableObject
{
    public Rect GetNodeCanvasRect(Vector2 mainSize)
    {
        float h = mainSize.y
            - 5
            - 5;
        float w = mainSize.x - 5 - 5;

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
                if(EditorGUIUtility.isProSkin)
                {
                    layout = AssetDatabase.LoadAssetAtPath<CanvasLayout>
    ("Assets/CodeMind/Editor/CanvasLayout/CanvasLayout.asset");
                }
                else
                {
                    layout = AssetDatabase.LoadAssetAtPath<CanvasLayout>
("Assets/CodeMind/Editor/CanvasLayout/CanvasLayout_Light.asset");
                }
            }
            return layout;
        }
    }

    [Header("Asset Title")]
    public GUIContent AssetTitleContent = new GUIContent("Code Mind");
    public GUIStyle AssetTitleStyle = new GUIStyle();

    [Space(10)]
    [Header("Common Config")]
    public CommonCfg common = new CommonCfg();


    [Space(10)]
    [Header("Info Windows Config")]
    public InfoWindowCfg info = new InfoWindowCfg();

    [Space(10)]
    [Header("Canvas Config")]
    public CanvasCfg canvas = new CanvasCfg();
}

[Serializable]
public class CommonCfg
{
    public GUIStyle CanvasBgStyle = new GUIStyle();
    
    public GUIStyle WindowTitleStyle = new GUIStyle();
    public GUIStyle window = new GUIStyle();

    public GUIContent RunningLabelContent = new GUIContent("Running...");
    public GUIStyle RunningLabelStyle = new GUIStyle();
    public GUIContent ErrorLabelContent = new GUIContent("Error...");
    public GUIStyle ErrorLabelStyle = new GUIStyle();
}

[Serializable]
public class InfoWindowCfg
{
    [Header("Info Rect")]
    public Rect rect;

    [Header("Style")]
    public GUIStyle windowStyle = new GUIStyle();
    public GUIContent TitleContent = new GUIContent("Info");

    public GUIContent SharedScriptMessageContent = new GUIContent();
    public GUIStyle SharedScriptMessageStyle = new GUIStyle();
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
    public GUIContent FocusBtContent = new GUIContent("Focuse");
    public GUIStyle CanvasNamelabelStyle = new GUIStyle();
    public GUIStyle windowNameTextStyle = new GUIStyle();
    public GUIStyle WindowStyle = new GUIStyle();

    [Header("Common")]
    public GUIContent DelWindowsContent = new GUIContent("Delete", "delete the windows");
    public GUIStyle ConnectedBtStyle = new GUIStyle();
    public GUIStyle ConnectBtStyle = new GUIStyle();
    public GUIStyle NavigationBtStyle = new GUIStyle();
    public GUIStyle BaseWindowsStyle = new GUIStyle();
    public GUIStyle SelectedWidnowsStyle = new GUIStyle();
    public GUIStyle DesTextStyle = new GUIStyle();
    public GUIStyle ErrorStyle = new GUIStyle();
    public GUIContent ScriptErrorContent = new GUIContent();

    [Space(10)]
    [Header("Start Window")]
    public GUIContent StartContent = new GUIContent("Start");
    public GUIStyle StartLabelStyle = new GUIStyle();

    [Space(10)]
    [Header("Node Window")]
    public GUIStyle NodeScriptErrorStyle = new GUIStyle();
    public GUIContent NodeScriptErrorContent = new GUIContent();

    [Space(10)]
    [Header("Router Window")]
    public GUIContent DefaultContent = new GUIContent("Defualt", "router default condition");
    public GUIStyle DefaultLabelStyle = new GUIStyle();
    public GUIContent DefaultErrorContent = new GUIContent("Defualt", "router default link is none");
    public GUIStyle DefaultErrorLabelStyle = new GUIStyle();

    public GUIContent AddConditionContent = new GUIContent("", "add a new condition");
    public GUIStyle AddConditionBtStyle = new GUIStyle();
    public Color AddConditionBtColor = Color.green;

    public GUIStyle ConditionBoxStyle = new GUIStyle();

    public GUIContent DelConditionContent = new GUIContent("", "delete this condition");
    public GUIStyle DelConditionStyle = new GUIStyle();
    public GUIStyle ConditionNameText = new GUIStyle();
    public GUIContent ConditionExpandBtContent = new GUIContent("","expand the plane");
    public GUIStyle ConditionExpandBtStyle = new GUIStyle();
    public GUIContent ConditionUnexpandBtContent = new GUIContent("","unexpand the plane");
    public GUIStyle ConditionUnexpandBtStyle = new GUIStyle();


    public GUIStyle ConditionUnExpandLabelStyle = new GUIStyle();
    public GUIStyle ConditionUnExpandErrorLabelStyle = new GUIStyle();

    public GUIStyle RouterScriptErrorStyle = new GUIStyle();
    public GUIContent RouterScriptErrorContent = new GUIContent();

    [Space(10)]
    [Header("Sub Canvas Window")]
    public GUIStyle SubCanvasErrorStyle = new GUIStyle();
    public GUIContent SubCanvasErrorContent = new GUIContent();
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CreateAssetMenu(menuName = "CanvasLayout/Create")]
public class CanvasLayout:ScriptableObject
{    
    public Rect ShareDataRect;
    public Rect CanvasListRect;
    public Rect CurrentWindowRect;
    public Rect NodeCanvasRect;

    public float border = 5;

    public Color lineColor = Color.white;

    public Rect GetCanvasListRect(float mainHeight)
    {
        float h = mainHeight
            - CurrentWindowRect.position.y - CurrentWindowRect.size.y
            - 10
            - 20;
        CanvasListRect.size = new Vector2(CanvasListRect.size.x, h);

        return CanvasListRect;
    }

    public Rect GetNodeCanvasRect(Vector2 mainSize)
    {
        float h = mainSize.y
            - 10
            - 10;
        float w = mainSize.x - 5 - 200 - 10 - 5;

        NodeCanvasRect.size = new Vector2(w, h);

        return NodeCanvasRect;
    }

    static CanvasLayout layout;
    public static CanvasLayout Layout
    {
        get
        {
            if(layout==null)
            {
                layout = AssetDatabase.LoadAssetAtPath<CanvasLayout>
                    ("Assets/ScriptNodeFlow/Editor/CanvasLayout.asset");
            }
            return layout;
        }
    }
}

using UnityEngine;
using UnityEditor;

namespace CodeMind
{
    public class SelectedWinWindow
    {
        GUILayoutOption width = GUILayout.Width(50);
        string CanvasID;
        public SelectedWinWindow(string canvasid)
        {
            CanvasID = canvasid;
        }

        public void draw(BaseWindow current)
        {                 
            GUILayout.BeginArea(CanvasLayout.Layout.selected.rect, CanvasLayout.Layout.common.window);
            
            GUILayout.Label(CanvasLayout.Layout.selected.TitleContent, CanvasLayout.Layout.common.WindowTitleStyle);
            
            if (current != null 
                && Event.current.type!=EventType.Ignore)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label(CanvasLayout.Layout.selected.IDContent);
                GUILayout.Label(current.Id, CanvasLayout.Layout.selected.IDLabelStyle);
                GUILayout.FlexibleSpace();

                if(GUILayout.Button(CanvasLayout.Layout.selected.CopyBtContent, CanvasLayout.Layout.common.CopyBtStyle))
                {
                    EditorGUIUtility.systemCopyBuffer = string.Format(NodeBinding.Format, CanvasID, current.Id);
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label(CanvasLayout.Layout.selected.TypeContent,CanvasLayout.Layout.common.TextTitleStyle);
                GUILayout.Space(5);
                GUILayout.Label(current.windowType.ToString(), CanvasLayout.Layout.selected.TypeLabelStyle, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if(current.windowType != NodeType.Start)
                {
                    EditorGUI.BeginDisabledGroup(Application.isPlaying);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label(CanvasLayout.Layout.selected.NameContent, CanvasLayout.Layout.common.TextTitleStyle, GUILayout.Width(38));
                    current.Name = GUILayout.TextField(current.Name, CanvasLayout.Layout.selected.NameTextStyle, GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();

                    GUILayout.Label(CanvasLayout.Layout.selected.DesContent, CanvasLayout.Layout.common.TextTitleStyle);
                    current.describe = GUILayout.TextArea(current.describe, CanvasLayout.Layout.selected.DesTextStyle, GUILayout.ExpandHeight(true));

                    EditorGUI.EndDisabledGroup();
                }
            }         

            GUILayout.EndArea();
        }
    }
}

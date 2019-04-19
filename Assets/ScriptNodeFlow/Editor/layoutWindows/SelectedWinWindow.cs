using UnityEngine;
using UnityEditor;

namespace ScriptNodeFlow
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
            GUILayout.BeginArea(CanvasLayout.Layout.selected.rect, Styles.window);
            
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
                GUILayout.Label(CanvasLayout.Layout.selected.TypeContent);
                GUILayout.Label(current.windowType.ToString(), CanvasLayout.Layout.selected.TypeLabelStyle, GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if(current.windowType != NodeType.Start)
                {
                    EditorGUI.BeginDisabledGroup(Application.isPlaying);

                    GUILayout.BeginHorizontal();                   
                    GUILayout.Label(CanvasLayout.Layout.selected.NameContent, GUILayout.Width(38));
                    current.Name = GUILayout.TextField(current.Name,GUILayout.ExpandWidth(true));
                    GUILayout.EndHorizontal();


                    GUILayout.Label(CanvasLayout.Layout.selected.DesContent);
                    current.describe = GUILayout.TextArea(current.describe, GUILayout.ExpandHeight(true));

                    EditorGUI.EndDisabledGroup();
                }
            }         

            GUILayout.EndArea();
        }
    }
}

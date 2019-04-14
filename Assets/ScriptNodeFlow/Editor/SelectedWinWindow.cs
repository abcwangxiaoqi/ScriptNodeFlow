using UnityEngine;
using UnityEditor;

namespace ScriptNodeFlow
{
    public class SelectedWinWindow
    {
        GUILayoutOption width = GUILayout.Width(50);
        public SelectedWinWindow()
        {
        }

        public void draw(BaseWindow current)
        {     
            GUILayout.BeginArea(CanvasLayout.Layout.CurrentWindowRect, Styles.window);

            GUILayout.Label("Current", Styles.titleLabel);
            
            if (current != null 
                && Event.current.type!=EventType.Ignore)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Label("ID:");
                GUILayout.Label(current.Id,Styles.subTitleLabel);
                GUILayout.FlexibleSpace();

                if(GUILayout.Button(GUIContents.copyID,Styles.copyButton))
                {
                    EditorGUIUtility.systemCopyBuffer = current.Id;
                }

                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                GUILayout.Label("Type:");
                GUILayout.Label(current.windowType.ToString(),Styles.subTitleLabel,GUILayout.ExpandWidth(true));
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();

                if(current.windowType != NodeType.Start)
                {
                    EditorGUI.BeginDisabledGroup(Application.isPlaying);

                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name:",GUILayout.Width(38));
                    current.Name = GUILayout.TextField(current.Name,GUILayout.ExpandWidth(true));
                    //GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();


                    GUILayout.Label("Describe:");
                    current.describe = GUILayout.TextArea(current.describe, GUILayout.ExpandHeight(true));

                    EditorGUI.EndDisabledGroup();
                }
            }         

            GUILayout.EndArea();
        }
    }
}

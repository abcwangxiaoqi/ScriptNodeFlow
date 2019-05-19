using UnityEngine;
using UnityEditor;
using UnityEditor.AnimatedValues;

namespace CodeMind
{
    public class SelectedWinWindow
    {
        AnimBool fadeAnim;

        GUILayoutOption width = GUILayout.Width(50);
        public SelectedWinWindow()
        {

            fadeAnim = new AnimBool(true);
        }

        public void draw(BaseWindow current)
        {                 
            GUILayout.BeginArea(CanvasLayout.Layout.selected.rect, CanvasLayout.Layout.common.window);
            
            GUILayout.Label(CanvasLayout.Layout.selected.TitleContent, CanvasLayout.Layout.common.WindowTitleStyle);
            
            if (current != null 
                && Event.current.type!=EventType.Ignore)
            {
                current.SelectedDraw();
            }         

            GUILayout.EndArea();
        }
    }
}

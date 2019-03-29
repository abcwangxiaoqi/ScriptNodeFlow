using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class SelectedWinWindow
    {
        private Rect rect;

        public SelectedWinWindow()
        {
            DelegateManager.Instance.RemoveListener(DelegateCommand.REFRESHCURRENTWINDOW, refreshWindow);
            DelegateManager.Instance.AddListener(DelegateCommand.REFRESHCURRENTWINDOW, refreshWindow);
        }
        
        void refreshWindow(object[] objs)
        {
            current = objs[0] as BaseWindow;
        }

        private const float border = 5;
        Vector2 position = new Vector2(border, 700);
        private float height = 500;
        BaseWindow current;

        public void draw(Rect main)
        {
            rect.position = position;
            rect.size = new Vector2(main.width - 2 * border, height);
            
            GUILayout.BeginArea(rect);

            GUILayout.Label("Current Window", Styles.titleLabel);

            if (current != null)
            {
                //GUILayout.BeginHorizontal();
                //GUILayout.Label(current.Id);
                //GUILayout.EndHorizontal();


                GUILayout.BeginHorizontal();
                GUILayout.Label("Type:");
                GUILayout.Label(current.windowType.ToString());
                GUILayout.EndHorizontal();

                if(current.windowType != NodeType.Start)
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Name:");
                    current.Name = GUILayout.TextField(current.Name);
                    GUILayout.EndHorizontal();


                    GUILayout.Label("Describe:");
                    current.describe = GUILayout.TextArea(current.describe, GUILayout.Height(50));
                }
            }         

            GUILayout.EndArea();
        }
    }
}

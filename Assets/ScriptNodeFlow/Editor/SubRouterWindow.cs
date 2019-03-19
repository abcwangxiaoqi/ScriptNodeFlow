using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class SubRouterWindow : RouterWindow
    {
        public SubRouterWindow(string orgin, Vector2 pos, List<BaseWindow> _windowList)
            : base(orgin, pos, _windowList)
        {
        }

        public SubRouterWindow(string orgin, RouterWindowData itemData, List<BaseWindow> _windowList)
            : base(orgin, itemData, _windowList)
        {
        }


        protected override void drawConditions()
        {
            for (int i = 0; i < conditions.Count; i++)
            {
                RouterWindowCondition rc = conditions[i];
                GUILayout.BeginHorizontal();

                //删除
                if (GUILayout.Button("", Styles.miniDelButton))
                {
                    conditions.RemoveAt(i);
                    i--;
                    _size.y -= addHeight;
                }

                string c = rc.className;
                int selectindex = allConditionClass.IndexOf(c);
                selectindex = EditorGUILayout.Popup(selectindex, allConditionClass.ToArray(), popupStyle);
                if (selectindex >= 0)
                {
                    conditions[i].className = allConditionClass[selectindex];
                }

                Rect rect = GUILayoutUtility.GetRect(0, 0);
                if (rect.position != Vector2.zero)
                {
                    rect.position += position + new Vector2(connectPortOffset, connectPortOffset);
                    rect.size = new Vector2(connectPortSize, connectPortSize);
                    rc.connectRect = rect;
                }

                GUILayout.EndHorizontal();
            }
        }

        protected override void drawDefualt()
        {
            GUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            GUILayout.Label("default", defaultLabel);

            Rect rect = GUILayoutUtility.GetRect(0, 0);
            if (rect.position != Vector2.zero)
            {
                rect.position += position + new Vector2(connectPortOffset, connectPortOffset);
                rect.size = new Vector2(connectPortSize, connectPortSize);

                defaultConnectRect = rect;
            }

            GUILayout.EndHorizontal();
        }
    }
}

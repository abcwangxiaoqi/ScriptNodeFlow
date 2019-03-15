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

              
                string c = rc.className;
                int selectindex = allConditionClass.IndexOf(c);
                selectindex = EditorGUILayout.Popup(selectindex, allConditionClass.ToArray(), popupStyle);
                if (selectindex >= 0)
                {
                    conditions[i].className = allConditionClass[selectindex];
                }

                //删除
                if (GUILayout.Button("", Styles.miniDelButton))
                {
                    conditions.RemoveAt(i);
                    i--;
                    _size.y -= addHeight;
                }

                //连接选择
                GUI.color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
                if (MyEditorLayout.Button("L", buttonStyle, out rect))
                {
                    GenericMenu menu = new GenericMenu();

                    menu.AddItem(newNode, false, () =>
                    {
                        var tempWindow = new NodeWindow(Orgin, new Vector2(50, 50) + position, windowList);
                        windowList.Add(tempWindow);
                        rc.nextWindow = tempWindow;
                    });

                    menu.AddSeparator("");

                    List<BaseWindow> selectionList = new List<BaseWindow>();
                    foreach (var item in windowList)
                    {
                        if (item.windowType == NodeType.Node || item.windowType == NodeType.SubCanvas)
                        {
                            selectionList.Add(item);
                        }
                    }

                    foreach (var item in selectionList)
                    {
                        bool select = (rc.nextWindow != null) && rc.nextWindow.Id == item.Id;
                        menu.AddItem(new GUIContent(string.Format("[{0}][{1}] {2}", item.Id, item.windowType, item.Name)), select, () =>
                        {
                            if (select)
                            {
                                rc.nextWindow = null;
                            }
                            else
                            {
                                rc.nextWindow = item;
                            }
                        });
                    }

                    menu.ShowAsContext();
                }


                GUI.color = Color.white;

                if (rc.nextWindow == null)
                {
                    linkStyle.normal.textColor = Color.gray;
                }
                else
                {
                    linkStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
                }

                MyEditorLayout.Label("o", linkStyle, out rect);

                //有的时候 rect会为0，0，1，1
                if (rect.position != Vector2.zero)
                {
                    rc.drawPos.x = rect.position.x + rect.width;
                    rc.drawPos.y = rect.position.y + rect.height / 2;
                }

                GUILayout.EndHorizontal();
            }
        }

        protected override void drawDefualt()
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("default", defaultLabel);

            //连接选择
            GUI.color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
            if (MyEditorLayout.Button("L", defaultLButton, out rect))
            {
                GenericMenu menu = new GenericMenu();
                
                menu.AddItem(newNode, false, () =>
                {
                    var tempWindow = new NodeWindow(Orgin, position, windowList);
                    windowList.Add(tempWindow);
                    defaultNextWindow = tempWindow;
                });

                List<BaseWindow> selectionList = new List<BaseWindow>();
                foreach (var item in windowList)
                {
                    if (item.windowType == NodeType.Node || item.windowType == NodeType.SubCanvas)
                    {
                        selectionList.Add(item);
                    }
                }

                foreach (var item in selectionList)
                {
                    bool select = (defaultNextWindow != null) && defaultNextWindow.Id == item.Id;
                    menu.AddItem(new GUIContent(item.Id + " " + item.Name), select, () =>
                    {
                        if (select)
                        {
                            defaultNextWindow = null;
                        }
                        else
                        {
                            defaultNextWindow = item;
                        }
                    });
                }

                menu.ShowAsContext();
            }

            if (defaultNextWindow == null)
            {
                linkStyle.normal.textColor = Color.gray;
            }
            else
            {
                linkStyle.normal.textColor = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
            }

            MyEditorLayout.Label("o", linkStyle, out rect);

            //有的时候 rect会为0，0，1，1
            if (rect.position != Vector2.zero)
            {
                defaultPos.x = rect.position.x + rect.width;
                defaultPos.y = rect.position.y + rect.height / 2;
            }

            GUI.color = Color.white;

            GUILayout.EndHorizontal();
        }
    }
}

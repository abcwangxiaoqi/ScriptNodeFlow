using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class SubStartWindow : StartWindow
    {
        public SubStartWindow(string orgin, StartWindowData itemData, List<BaseWindow> _windowList)
            : base(orgin, itemData, _windowList)
        {
        }

        public override void rightMouseClick(Vector2 mouseposition)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(nextNewNodeContent, false, () =>
            {
                var tempWindow = new NodeWindow(Orgin, position, windowList);
                windowList.Add(tempWindow);
                next = tempWindow;
            }); ;

            menu.AddSeparator(separator);

            #region select the next one
            List<BaseWindow> selectionList = new List<BaseWindow>();

            foreach (var item in windowList)
            {
                if (item.Id == Id)
                    continue;
                if (item.windowType == NodeType.Router)
                    continue;

                selectionList.Add(item);
            }

            foreach (var item in selectionList)
            {
                bool select = (next != null) && next.Id == item.Id;

                menu.AddItem(new GUIContent(string.Format("Next/[{0}][{1}] {2}", item.Id, item.windowType, item.Name))
                    , select, () =>
                    {
                        if (select)
                        {
                            next = null;
                        }
                        else
                        {
                            next = item;
                        }
                    });
            }
            #endregion

            menu.ShowAsContext();
        }
    }
}

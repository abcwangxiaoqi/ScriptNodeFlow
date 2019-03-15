﻿using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ScriptNodeFlow
{
    public class StartWindow : BaseWindow
    {
        protected static GUIContent nextNewNodeContent = new GUIContent("Next/New Node");
        protected static GUIContent nextNewSubCanvasContent = new GUIContent("Next/New SubCanvas");
        protected static string separator = "Next/";

        //下一节点
        public BaseWindow next { get; protected set; }

        Vector2 _size = new Vector2(150, 60);
        protected override Vector2 size
        {
            get
            {
                return _size;
            }
        }


        public override NodeType windowType
        {
            get
            {
                return NodeType.Start;
            }
        }

        public StartWindow(string orgin, StartWindowData itemData, List<BaseWindow> _windowList)
            : base(orgin,itemData, _windowList)
        {
        }

        public void SetNext(BaseWindow entity)
        {
            next = entity;
        }

        public override WindowDataBase GetData()
        {
            StartWindowData dataEntity = new StartWindowData();
            dataEntity.position = position;

            if (next != null)
            {
                dataEntity.nextWindowId = next.Id;
            }

            return dataEntity;
        }

        public override void draw()
        {
            base.draw();

            //draw line
            if (next != null)
            {
                if (!windowList.Contains(next))
                {
                    next = null;
                    return;
                }

                Color color = Color.white;

                if (Application.isPlaying && windowData.runtimeState == RuntimeState.Finished)
                {
                    color = EditorGUIUtility.isProSkin ? Color.green : Color.grey;
                }

                DrawArrow(Out, next.In, color);
            }
        }

        protected override void gui(int id)
        {
            GUILayout.Label("Start", Styles.titleLabel);

            GUI.DragWindow();
        }
        
        public override void rightMouseClick(Vector2 mouseposition)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(nextNewNodeContent, false, () =>
            {
                var tempWindow = new NodeWindow(Orgin, position, windowList);
                windowList.Add(tempWindow);
                next = tempWindow;
            });

            menu.AddItem(nextNewSubCanvasContent, false, () =>
            {
                var tempWindow = new SubCanvasWindow(Orgin, position, windowList);
                windowList.Add(tempWindow);
                next = tempWindow;
            });

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
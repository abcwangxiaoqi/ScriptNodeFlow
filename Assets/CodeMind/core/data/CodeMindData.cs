﻿using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace CodeMind
{
    public enum PlayMode
    {
        Once,
        Loop
    }

    [Serializable]
    public class CodeMindData : ScriptableObject
    {
        public PlayMode mode = PlayMode.Once;

        public SharedData shareData;

        public StartWindowData start = new StartWindowData();
        public List<NodeWindowProxy> nodelist = new List<NodeWindowProxy>();
        public List<RouterWindowProxy> routerlist = new List<RouterWindowProxy>();
        public List<CodeMindWindowData> subCodeMindlist = new List<CodeMindWindowData>();

        public string desc;

        internal WindowDataBase Get(string id)
        {
            WindowDataBase data = null;

            data = nodelist.Find(windowData => { return windowData.ID == id; });
            if (data != null)
                return data;

            data = routerlist.Find(windowData => { return windowData.ID == id; });
            if (data != null)
                return data;

            data = subCodeMindlist.Find(windowData => { return windowData.ID == id; });

            return data;
        }

        /*public NodeWindowData AddNode(Vector2 position)
        {
            NodeWindowData node = new NodeWindowData();
            node.position = position;
            nodelist.Add(node);
            return node;
        }*/

        public NodeWindowProxy AddNode(Type type, Vector2 postion,NodeUsageAttribute attribute)
        {
            var nodeData = Activator.CreateInstance(type) as NodeWindowProxy;
            nodeData.position = postion;
            nodeData.name = attribute.winName;
            nodeData.desc = attribute.winDes;

            //generate the node
            var node = ScriptableObject.CreateInstance(attribute.targetType) as Node;
            node.name = nodeData.ID;
            UnityEditor.AssetDatabase.AddObjectToAsset(node, this);

            nodeData.node = node;

            nodeData.AssetCreate();

            nodelist.Add(nodeData);

            return nodeData;
        }

        public void RemoveNode(NodeWindowProxy nodeWindow)
        {
            if (nodeWindow == null)
                return;

            nodeWindow.AssetDelete();

            nodelist.Remove(nodeWindow);
        }

        public RouterWindowProxy AddRouter(Vector2 position)
        {
            RouterWindowProxy router = new RouterWindowProxy();
            router.position = position;
            routerlist.Add(router);
            return router;
        }

        public RouterWindowProxy AddRouter(Type type,Vector2 position,RouterUsageAttribute attribute)
        {
            var routerData = Activator.CreateInstance(type) as RouterWindowProxy;
            routerData.position = position;
            routerData.name = attribute.winName;
            routerData.desc = attribute.winDes;

            //add pre conditions

            var preTypes = routerData.preConditionTypes;
            foreach (var tyname in preTypes)
            {
                var ty = Type.GetType(tyname);
                if (ty == null)
                    continue;

                var serialAttrs = ty.GetCustomAttributes(typeof(System.SerializableAttribute), false);
                if (serialAttrs == null || serialAttrs.Length == 0)
                    continue;

                var attrs = ty.GetCustomAttributes(typeof(ConditionUsageAttribute), false);
                if (attrs == null || attrs.Length == 0)
                    continue;

                var attr = attrs[0] as ConditionUsageAttribute;

                var condition = AddCondition(ty, attr);

                routerData.conditions.Add(condition);
            }

            routerlist.Add(routerData);

            routerData.AssetCreate();

            return routerData;
        }

        public void RemoveRouter(RouterWindowProxy router)
        {
            router.AssetDelete();

            routerlist.Remove(router);
        }

        public RouterWindowConditionProxy AddCondition(Type type,ConditionUsageAttribute attribute)
        {
            var conditionData = Activator.CreateInstance(type) as RouterWindowConditionProxy;
            conditionData.name = attribute.conditionName;

            var condition = ScriptableObject.CreateInstance(attribute.conditionType) as RouterCondition;
            condition.name = conditionData.ID;
            UnityEditor.AssetDatabase.AddObjectToAsset(condition, this);

            conditionData.routerCondition = condition;

            conditionData.AssetCreate();

            return conditionData;
        }

        public void RemoveCondition(RouterWindowProxy router,RouterWindowConditionProxy condition)
        {
            condition.AssetDelete();

            router.conditions.Remove(condition);
        }

        public CodeMindWindowData AddSubCanvas(Vector2 position)
        {
            CodeMindWindowData subCanvas = new CodeMindWindowData();
            subCanvas.position = position;
            subCodeMindlist.Add(subCanvas);
            return subCanvas;
        }


        /// <summary>
        /// Instantiate this instance.
        /// </summary>
        /// <returns>The instantiate.</returns>
        public GameObject Instantiate(out CodeMindController controller, Transform root = null)
        {
            GameObject gameObject = new GameObject(this.name, typeof(CodeMindController));
            gameObject.transform.SetParent(root);
            DontDestroyOnLoad(gameObject);
            controller = gameObject.GetComponent<CodeMindController>();
            controller.mindData = this;
            return gameObject;
        }

        internal void OnReset()
        {
            start.OnReset();

            foreach (var item in nodelist)
            {
                item.OnReset();
            }

            foreach (var item in routerlist)
            {
                item.OnReset();
            }

            foreach (var item in subCodeMindlist)
            {
                item.OnReset();
            }
        }

        internal void OnCreate()
        {
            if (shareData != null)
            {
                shareData.OnCreate();
            }

            foreach (var item in nodelist)
            {
                item.OnCreate(shareData);
            }

            foreach (var item in routerlist)
            {
                item.OnCreate(shareData);
            }

            foreach (var item in subCodeMindlist)
            {
                item.OnCreate(shareData);
            }
        }

        internal void OnAssetDestroy()
        {
            if (shareData != null)
            {
                shareData.OnObjectDestroy();
            }

            foreach (var item in nodelist)
            {
                item.OnObjectDestroy(shareData);
            }

            foreach (var item in routerlist)
            {
                item.OnObjectDestroy(shareData);
            }

            foreach (var item in subCodeMindlist)
            {
                item.OnObjectDestroy(shareData);
            }
        }


#if UNITY_EDITOR

        public void Compile()
        {
            var path = UnityEditor.AssetDatabase.GetAssetPath(this);

            foreach (var item in nodelist)
            {
                if (item.node != null)
                    continue;

                Debug.LogErrorFormat("{0} : Node '{1}' script is invalid", path, item.name);
            }


            List<RouterWindowProxy> routers = routerlist;
            foreach (var item in routers)
            {
                foreach (var condItem in item.conditions)
                {
                    if (condItem.routerCondition != null)
                        continue;

                    Debug.LogErrorFormat("{0} : Condition '{1}' of Router '{2}' script is invalid", path, condItem.name, item.name);
                }
            }
        }
#endif
    }
}
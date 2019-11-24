using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMind
{
    public class InstantiatePrefab : Node
    {
        public GameObject prefab;
        public Vector3 position;
        public Vector3 eulerAngles;
        public string gObjectName;

        GameObject instance;
        protected override void OnEnter()
        {
            instance = Instantiate(prefab) as GameObject;
            instance.transform.position = position;
            instance.transform.eulerAngles = eulerAngles;

            if(!string.IsNullOrEmpty(gObjectName))
            {
                instance.name = gObjectName;
            }

            moveNext();
        }

        protected override void OnNodeDestroy()
        {
            Destroy(instance);
        }
    }
}

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

        GameObject instance;
        protected override void OnNodePlay(SharedData sharedData)
        {
            instance = GameObject.Instantiate(prefab) as GameObject;
            instance.transform.position = position;
            instance.transform.eulerAngles = eulerAngles;

            moveNext();
        }

        protected override void OnNodeDestroy(SharedData sharedData)
        {
            GameObject.Destroy(instance);
        }
    }
}

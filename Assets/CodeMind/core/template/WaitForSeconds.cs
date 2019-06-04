using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMind
{
    public class WaitForSeconds : Node
    {
        public float time = 3;

        float passTime = 0f;

        protected override void OnNodePlay(SharedData sharedData)
        {
            passTime = 0;
        }

        protected override void OnNodeUpdate(SharedData sharedData)
        {
            passTime += Time.deltaTime;
            if(passTime>= time)
            {
                moveNext();
            }
        }
    }
}

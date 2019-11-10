using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMind
{
    public class WaitForSeconds : Node
    {

        public float time = 3;

        float passTime = 0f;

        protected override void OnPlay(SharedData sharedData)
        {
            passTime = 0;
        }

        protected override void OnProcessUpdate(SharedData sharedData)
        {
            passTime += Time.deltaTime;
            if(passTime>= time)
            {
                moveNext();
            }
        }
    }
}

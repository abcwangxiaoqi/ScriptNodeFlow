using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CodeMind
{
    public class WaitForSeconds : Node
    {

        public float time = 3;

        float passTime = 0f;

        protected override void OnEnter()
        {
            passTime = 0;
        }

        protected override void OnNodeUpdate()
        {
            passTime += Time.deltaTime;
            if(passTime>= time)
            {
                moveNext();
            }
        }
    }
}

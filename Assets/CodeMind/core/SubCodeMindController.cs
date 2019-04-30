using System;
using System.Collections;
using System.Collections.Generic;
using CodeMind;
using UnityEngine;

public class SubCodeMindController : MonoBehaviour
{
    public event Action<bool> onFinish;

    public SubCanvasData subNodeFlowData;

    [HideInInspector]public SharedData shareData;

	// Use this for initialization
	void Start () {

        current = subNodeFlowData.start;

    }

    public void stateReset()
    {
        current = subNodeFlowData.start;
        current.reset();

        finished = false;
    }

    public bool finished { get; private set; }

    private WindowDataBase current = null;

    // Update is called once per frame
    void Update () {

        if (finished)
            return;

        if (current == null)
            return;

        #region Node Update

        if (current.runtimeState == RuntimeState.Running)
        {
            // current version , just only Node
            current.update();
        }

        #endregion

        if (current.runtimeState != RuntimeState.Idle)
            return;

        if (current.type == NodeType.Start)
        {
            current.play();
        }
        else if (current.type == NodeType.Node ||
            current.type == NodeType.Router)
        {
            current.play(shareData);
        }
    }

    private void LateUpdate()
    {
        if (finished)
            return;

        if (current == null)
            return;

        if (current.runtimeState == RuntimeState.Finished)
        {
            //get next
            if (current.type == NodeType.Router)
            {
                current = subNodeFlowData.Get((current as RouterWindowData).runtimeNextId);
            }
            else
            {
                current = subNodeFlowData.Get(current.nextWindowId);
            }

            if (current == null)
            {
                finished = true;
                if (onFinish != null)
                {
                    onFinish.Invoke(true);
                }
            }
            else if (current.runtimeState == RuntimeState.Finished)
            {
                current.reset();
            }
        }
        else if (current.runtimeState == RuntimeState.Error)
        {
            finished = true;
            if (onFinish != null)
            {
                onFinish.Invoke(false);
            }
        }
    }

    private void OnDestroy()
    {
        if(current!=null)
        {
            current.stop();
        }
    }
}

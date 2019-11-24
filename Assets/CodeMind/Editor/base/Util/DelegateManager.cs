using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class DelegateManager
{
    /// <summary>
    /// 带返回参数
    /// </summary>
    private Dictionary<string, Action<object[]>> objectActions = new Dictionary<string, Action<object[]>>();

    /// <summary>
    /// 注册委托指令，同一个方法只能添加一次
    /// </summary>
    public void AddListener(string command, Action<object[]> callback)
    {
        if (callback == null)
        {
            return;
        }

        if (objectActions.ContainsKey(command))
        {
            Action<object[]> curr = objectActions[command];
            if (curr != null)
            {
                Delegate[] delegates = curr.GetInvocationList();
                for (int i = 0; i < delegates.Length; i++)
                {
                    Action<object[]> action = delegates[i] as Action<object[]>;
                    if (action == callback)//已经注册，不再进行添加
                    {
                        Debug.LogWarning(">> DelegateManager > repeat > " + callback.Method.Name);
                        return;
                    }
                }
            }
            objectActions[command] += callback;
        }
        else
        {
            objectActions.Add(command, callback);
        }
    }

    /// <summary>
    /// 移除委托指令
    /// </summary>
    public void RemoveListener(string command, Action<object[]> callback)
    {
        if (objectActions.ContainsKey(command))
        {
            objectActions[command] -= callback;
        }
    }

    /// <summary>
    /// 派发指令
    /// </summary>
    public void Dispatch(string command, params object[] obj)
    {
        if (objectActions.ContainsKey(command))
        {
            if (objectActions[command] != null)
            {
                objectActions[command].Invoke(obj);
            }
        }
    }

}
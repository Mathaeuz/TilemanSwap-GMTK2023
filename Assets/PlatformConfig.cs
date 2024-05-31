using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class PlatformConfig : MonoBehaviour
{
    [Serializable]
    public struct PlatformAction
    {
        public RuntimePlatform Platform;
        public UnityEvent Action;        
        public UnityEvent NotAction;
    }
    public PlatformAction[] ActionMap;
    // Start is called before the first frame update
    void Start()
    {
        foreach (var item in ActionMap)
        {
            if (item.Platform == Application.platform)
            {
                item.Action.Invoke();
            }else{
                item.NotAction.Invoke();
            }
        }

    }
}

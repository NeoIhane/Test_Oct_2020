﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
                Debug.Log(typeof(T).ToString() + " is NULL");

            return _instance;
        }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            SetInstance();
        }
        else
        {
            Destroy(this.gameObject);
        }
    }
    void SetInstance()
    {
        _instance = this as T;
    }

}


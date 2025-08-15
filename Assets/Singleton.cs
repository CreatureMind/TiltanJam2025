using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;
    private static readonly object objLock = new();

    protected virtual bool dontDestroyOnLoad => true;

    protected virtual void Awake()
    {
        if (instance == null)
            instance = this as T;
        else if (instance != this)
            Destroy(gameObject);

        if (dontDestroyOnLoad)
            DontDestroyOnLoad(gameObject);
    }

    public static T Get()
    {
        lock (objLock)
        {
            if (!instance)
            {
                instance = new GameObject(typeof(T).Name).AddComponent<T>();
            }

            return instance;
        }
    }
}
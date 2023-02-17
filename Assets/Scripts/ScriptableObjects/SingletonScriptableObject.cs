using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingletonScriptableObject<T> : ScriptableObject where T : ScriptableObject
{

    static T instance;
    public static T Instance
    {
        get
        {
            if (instance == null)
            {
                instance = Resources.Load<T>(typeof(T).ToString());
                (instance as SingletonScriptableObject<T>).OnInitialize();
            }
            return instance;
        }
    }

    // Optional overridable method for initializing the instance.
    protected virtual void OnInitialize() { }

}

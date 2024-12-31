using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IOCContainer
{
    private Dictionary<Type,object> instanceDict = new Dictionary<Type, object> ();

    public void Register<T>(T instance) {
        var key = typeof(T);
        if (instanceDict.ContainsKey(key)) {
            instanceDict[key] = instance;
        }
        else { 
        instanceDict.Add(key, instance);
        }
    }

    public T Get<T>() where T : class {

        var key = typeof(T);
        object obj = null;
        if (instanceDict.TryGetValue(key, out obj)) {
            return obj as T;
        }
        else {
            Debug.Log("获取的对象为空");       
        }
        return null;    
        
    }

    public void InitAllModules() {
        foreach (var item in instanceDict) {
            ((INeedInit)item.Value).Init();
        }
    }




}

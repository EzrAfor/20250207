using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IEventSystem
{

    void Register<T>(Action<object> onEvent);
   
    void UnRegister<T>(Action<object> onEvent);
    
    void Send<T>(object obj) where T : new();
}

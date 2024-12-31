using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameEventSystem : IEventSystem
{
    private Dictionary<Type, IEventRegistraion> eventRegistrationsDict = new Dictionary<Type, IEventRegistraion>();

    public void Register<T>(Action<object> onEvent)
    {
        var type = typeof(T);
        IEventRegistraion eventRegistraion;
        if (eventRegistrationsDict.TryGetValue(type, out eventRegistraion))
        {
            (eventRegistraion as EventRegistration).OnEvent += onEvent;
        }
        else
        {
            eventRegistraion = new EventRegistration()
            {
                OnEvent = onEvent
            };
            eventRegistrationsDict.Add(type, eventRegistraion);
        }
    }

    public void Send<T>(object obj) where T : new()
    {
        var type = typeof(T);
        IEventRegistraion eventRegistraion;
        if (eventRegistrationsDict.TryGetValue(type, out eventRegistraion))
        {
            (eventRegistraion as EventRegistration).OnEvent.Invoke(obj);
        }
    }

    public void UnRegister<T>(Action<object> onEvent)
    {
        var type = typeof(T);
        IEventRegistraion eventRegistraion;
        if (eventRegistrationsDict.TryGetValue(type, out eventRegistraion))
        {
            (eventRegistraion as EventRegistration).OnEvent -= onEvent;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class EventExtension
{
    public static void SendEvent<T>(this ICanSendEvent self, object dataObj = null) where T : new()
    {
        StartArchitecture.Instance.GetArchitecture().SendEvent<T>(dataObj);
    }

    public static void RegisterEvent<T>(this ICanRegisterAndUnregisterEvent self, Action<object> onEvent) where T : new()
    {
        StartArchitecture.Instance.GetArchitecture().RegisterEvent<T>(onEvent);
    }

    public static void UnRegisterEvent<T>(this ICanRegisterAndUnregisterEvent self, Action<object> onEvent) where T : new()
    {
        StartArchitecture.Instance.GetArchitecture().RegisterEvent<T>(onEvent);
    }
}

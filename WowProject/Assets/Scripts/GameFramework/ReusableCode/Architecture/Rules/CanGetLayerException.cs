using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CanGetLayerExtension
{
    public static T GetSystem<T>(this ICanGetSystem self) where T : class, ISystem
    {
        return StartArchitecture.Instance.GetArchitecture().GetSystem<T>();
    }
    public static T GetModel<T>(this ICanGetModel self) where T : class, IModel
    {
        return StartArchitecture.Instance.GetArchitecture().GetModel<T>();
    }
    public static T GetUtility<T>(this ICanGetUtility self) where T : class, IUtility
    {
        return StartArchitecture.Instance.GetArchitecture().GetUtility<T>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CanSendCommandExtension 
{
    public static void SendCommand<T>(this ICanSendCommand self ,object dataobj=null)where T:ICommand,new()
    {
        StartArchitecture.Instance.GetArchitecture().SendCommand<T>(dataobj);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：命令接口
//***************************************** 
public interface ICommand:ICanGetModel,ICanGetSystem,ICanGetUtility,ICanSendEvent,ICanSendCommand
{
    public void Execute(object dataObj);
}

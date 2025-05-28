using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：WOW游戏架构
//***************************************** 
public class WowArchitercture : Architecture<WowArchitercture>
{
    protected override void Init()
    {
        this.RegistSystem<INetSystem>(new NetSystem());
        this.RegistSystem<IUISystem>(new UISystem());
    }
}

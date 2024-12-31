using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WowArchitecture : Architecture<WowArchitecture>
{
    protected override void Init()
    {
        this.RegisterSystem<INetSystem>(new NetSystem());
        //this.RegisterSystem<IUISystem>(new UISystem());
        
    }
}

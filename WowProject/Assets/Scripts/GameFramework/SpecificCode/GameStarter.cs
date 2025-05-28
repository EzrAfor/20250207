using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：游戏入口实例
//***************************************** 
public class GameStarter : MonoBehaviour,IController
{
    private StartArchitecture startArchitecture;

    void Awake()
    {
        startArchitecture = StartArchitecture.Instance;
        startArchitecture.SetGameArchitecture(new WowArchitercture());
        startArchitecture.InitAllModulesInArchitecture();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        this.GetSystem<INetSystem>().Update();
    }
}

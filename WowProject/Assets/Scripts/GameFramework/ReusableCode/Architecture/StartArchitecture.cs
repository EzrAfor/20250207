using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：启动框架的启动器（启动入口）
//***************************************** 
public class StartArchitecture : Singleton<StartArchitecture>, ISingleton
{
    private IArchitecture gameArchitecture;

    private StartArchitecture() { Init(); }

    public void Init() { }
    /// <summary>
    /// 实例化并设置不同项目的架构
    /// </summary>
    public void SetGameArchitecture(IArchitecture architecture)
    {
        gameArchitecture = architecture;
    }
    /// <summary>
    /// 获取架构实例
    /// </summary>
    public IArchitecture GetArchitecture()
    {
        return gameArchitecture;
    }
    /// <summary>
    /// 初始化框架中的所有模块
    /// </summary>
    public void InitAllModulesInArchitecture()
    {
        gameArchitecture.InitAllModules();
    }
}

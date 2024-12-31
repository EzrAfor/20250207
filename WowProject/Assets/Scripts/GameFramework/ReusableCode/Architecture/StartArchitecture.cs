using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//�����ˣ� Trigger 
//����˵����������ܵ���������������ڣ�
//***************************************** 
public class StartArchitecture : Singleton<StartArchitecture>, ISingleton
{
    private IArchitecture gameArchitecture;

    private StartArchitecture() { Init(); }

    public void Init() { }
    /// <summary>
    /// ʵ���������ò�ͬ��Ŀ�ļܹ�
    /// </summary>
    public void SetGameArchitecture(IArchitecture architecture)
    {
        gameArchitecture = architecture;
    }
    /// <summary>
    /// ��ȡ�ܹ�ʵ��
    /// </summary>
    public IArchitecture GetArchitecture()
    {
        return gameArchitecture;
    }
    /// <summary>
    /// ��ʼ������е�����ģ��
    /// </summary>
    public void InitAllModulesInArchitecture()
    {
        gameArchitecture.InitAllModules();
    }
}

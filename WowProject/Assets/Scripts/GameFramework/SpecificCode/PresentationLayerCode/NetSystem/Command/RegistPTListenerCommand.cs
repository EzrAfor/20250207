using UnityEngine;
/// <summary>
/// 命令名称：
/// 参数:
/// </summary>
public struct RegistPTListenerCommand : ICommand
{
    public void Execute(object dataObj)
    {        
        PTSrc ptSrc = (PTSrc)dataObj;
        this.GetSystem<INetSystem>().RegistPTListener(ptSrc.ptName, ptSrc.listener);
    }
}

public struct PTSrc
{
    public string ptName;
    public NetSystem.PTListener listener;
}
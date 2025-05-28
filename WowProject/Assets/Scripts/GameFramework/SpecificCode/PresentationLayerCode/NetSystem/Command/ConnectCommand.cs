using UnityEngine;
/// <summary>
/// 创建人：Trigger 
/// 命令名称：
/// 参数:
/// </summary>
public struct ConnectCommand : ICommand
{
    public void Execute(object dataObj)
    {
        ConnectCommandSrc src = (ConnectCommandSrc)dataObj;
        this.GetSystem<INetSystem>().Connect(src.ipAddress,src.port);        
    }
}
/// <summary>
/// 
/// </summary>
public struct ConnectCommandSrc
{
    public string ipAddress;
    public int port;
}

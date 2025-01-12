using UnityEngine;
/// <summary>
/// 命令名称：
/// 参数:
/// </summary>
public struct SetPDValueCommand : ICommand
{
    public void Execute(object dataObj)
    {
        this.GetSystem<INetSystem>().SetPDValue((PlayerData)dataObj);
    }
}

using UnityEngine;
/// <summary>
/// 创建人：Trigger 
/// 命令名称：
/// 参数:
/// </summary>
public struct SendPTCommand : ICommand
{
    public void Execute(object dataObj)
    {
        this.GetSystem<INetSystem>().Send((PTBase)dataObj);
    }
}

using UnityEngine;
/// <summary>
/// 创建人：Trigger 
/// 命令名称：
/// 参数:
/// </summary>
public struct SetPDValueCommand : ICommand
{
    public void Execute(object dataObj)
    {
        PlayerData pd = (PlayerData)dataObj;
        this.GetSystem<INetSystem>().SetPSDValue(pd.PDToPSD());
    }
}

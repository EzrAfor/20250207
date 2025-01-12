using UnityEngine;
/// <summary>
/// 命令名称：
/// 参数:
/// </summary>
public struct SendEnterGamePTCommand : ICommand
{
    public void Execute(object dataObj)
    {
        INetSystem ins = this.GetSystem<INetSystem>();
        PTEnterGameScene ptgs = new PTEnterGameScene();
        ptgs.enterGamePlayerDataJson=JsonUtility.ToJson(ins.GetPDValue());
        ins.Send(ptgs);
    }
}

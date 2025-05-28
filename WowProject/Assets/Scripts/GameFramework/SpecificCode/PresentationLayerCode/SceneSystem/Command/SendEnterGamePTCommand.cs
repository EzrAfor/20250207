using UnityEngine;
/// <summary>
/// 创建人：Trigger 
/// 命令名称：
/// 参数:
/// </summary>
public struct SendEnterGamePTCommand : ICommand
{
    public void Execute(object dataObj)
    {
        INetSystem ins = this.GetSystem<INetSystem>();
        PTEnterGameScene ptgs = new PTEnterGameScene();
        ptgs.choiceID = ins.GetChoiceID();
        ins.Send(ptgs);
    }
}

using UnityEngine;
/// <summary>
/// 创建人：Trigger 
/// 命令名称：
/// 参数:
/// </summary>
public struct UnLoadLoginPanelsAndLoadGamePanelsCommand : ICommand
{
    public void Execute(object dataObj)
    {
        this.GetSystem<IUISystem>().UnLoadLoginPanelsAndLoadGamePanels();
    }
}

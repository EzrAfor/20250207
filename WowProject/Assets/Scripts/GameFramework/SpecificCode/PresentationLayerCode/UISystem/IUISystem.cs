using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：UI系统接口
//***************************************** 
public interface IUISystem : ISystem
{
    /// <summary>
    /// 打开该类型的面板
    /// </summary>
    /// <param name="panelType"></param>
    public void OpenPanel<T>(params object[] objs) where T : BasePanel;
    /// <summary>
    /// 关闭该类型的面板
    /// </summary>
    /// <param name="panelType"></param>
    public void ClosePanel(string panelType);
    /// <summary>
    /// 加载游戏面板
    /// </summary>
    public void UnLoadLoginPanelsAndLoadGamePanels();
    /// <summary>
    /// 当前选择的人物数据
    /// </summary>
    public PlayerData ChoiePlayerData { get ; set; }
    /// <summary>
    /// 通过物品ID获取物品信息
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public ItemData GetItemDataByID(int id);
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUISystem : ISystem
{

    public void UnLoadLoginPanelsAndLoadGamePanels();
    public void OpenPanel<T>(params object[] objs) where T : BasePanel;
    public void ClosePanel(string panelType);
    public void RotateModel(float angle); //旋转玩家角色模型
    public PlayerData ChoicePlayerData { get ; set; }


}

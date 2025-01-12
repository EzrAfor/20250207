using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUISystem : ISystem
{

    public void UnLoadLoginPanelsAndLoadGamePanels();
    public void OpenPanel<T>(params object[] objs) where T : BasePanel;
    public void ClosePanel(string panelType);
    public PlayerData ChoicePlayerData { get ; set; }


}

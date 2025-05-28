using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：
//***************************************** 
public class SceneLoader : MonoBehaviour,IController
{
    void Start()
    {
        this.SendCommand<SendEnterGamePTCommand>();
        this.SendCommand<RegistPTListenerCommand>(new PTSrc() {ptName= "PTEnterGameScene",listener= OnPTEnterGameScene });
        this.SendCommand<UnLoadLoginPanelsAndLoadGamePanelsCommand>();
        this.SendCommand<SendPTCommand>(new PTGetInventoryItemList());
    }

    void Update()
    {

    }
    /// <summary>
    /// 有新玩家进入游戏场景
    /// </summary>
    /// <param name="pt"></param>
    private void OnPTEnterGameScene(PTBase pt)
    {
        PTEnterGameScene ptgs = (PTEnterGameScene)pt;
        this.SendCommand<OnPTEnterGameSceneCommand>(ptgs);       
    }
}

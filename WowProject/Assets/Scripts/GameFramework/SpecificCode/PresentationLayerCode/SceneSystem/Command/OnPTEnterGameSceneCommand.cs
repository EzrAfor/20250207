using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// 命令名称：
/// 参数:
/// </summary>
public struct OnPTEnterGameSceneCommand : ICommand
{
    public void Execute(object dataObj)
    {
        INetSystem ins = this.GetSystem<INetSystem>();
        PTEnterGameScene ptgs = ( PTEnterGameScene )dataObj;
        PlayerData pd = JsonUtility.FromJson<PlayerData>(ptgs.enterGamePlayerDataJson);
        List<PlayerData> playerDatas = JsonUtility.FromJson<PlayerDatasList>(ptgs.playDatasListJson).playerDatas;
        ins.SetPDListValue(playerDatas);
        List<PlayerData> l = ins.GetSyncPDListValue();

        if (pd.id == ins.GetPDValue().id)
        {
            //客户端第一次进入
            //生成自身
            GameObject selfGo = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Character/Player"), new Vector3(pd.x, pd.y, pd.z), Quaternion.Euler(pd.ex, pd.ey, pd.ez));
            selfGo.name = pd.id;
            selfGo.GetComponent<PlayerMovementController>().InitDressState(ins.GetPDValue());
            for (int i = 0; i < l.Count;  i++)
            {
                //生成其他玩家
            GameObject otherGo = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Character/Player"), new Vector3(l[i].x, l[i].y, l[i].z),Quaternion.Euler(l[i].ex, l[i].ey, l[i].ez));
                otherGo.name = l[i].id;
                SyncPMCtrl spmc = otherGo.GetComponent<SyncPMCtrl>();
                spmc.InitDressState(l[i]);
                ins.AddNewPlayerData(l[i], spmc);
            }
            GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Scene/SilvermoonCity"));
        }
        else
        {
            //收到其他新玩家进入游戏场景
            GameObject newGo = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Character/Player"), new Vector3(pd.x, pd.y, pd.z), Quaternion.Euler(pd.ex, pd.ey, pd.ez));
            newGo.name = pd.id;
            SyncPMCtrl spmc = newGo.GetComponent<SyncPMCtrl>();
            //生成新进入玩家的角色
            spmc.InitDressState(pd);
            ins.AddNewPlayerData(pd,spmc);
        }
    }
}

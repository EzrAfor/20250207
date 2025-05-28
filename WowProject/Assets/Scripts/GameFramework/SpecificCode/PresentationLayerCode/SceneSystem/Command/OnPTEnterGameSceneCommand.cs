using UnityEngine;
using System.Collections.Generic;
using System;
/// <summary>
/// 创建人：Trigger 
/// 命令名称：
/// 参数:
/// </summary>
public struct OnPTEnterGameSceneCommand : ICommand
{
    public void Execute(object dataObj)
    {
        INetSystem ins = this.GetSystem<INetSystem>();
        PTEnterGameScene ptgs = (PTEnterGameScene)dataObj;
        PlayerData pd =JsonUtility.FromJson<PlayerData>(ptgs.enterGamePlayerDataJson);
        if (pd.id==ins.GetPSDValue().id)
        {
            //Console.WriteLine(ptgs.playerDatasListJson);
            List<PlayerData> playerDatas = JsonUtility.FromJson<PlayerDatasList>
        (ptgs.playerDatasListJson).playerDatas;
            //for (int i = 0; i < playerDatas.Count; i++)
            //{
            //    Debug.Log(playerDatas[i].id);
            //}
            ins.SetPDListValue(playerDatas);
            List<PlayerData> l = ins.GetSyncPDListValue();
            //Debug.Log(l.Count);
            this.SendEvent<UpdatePlayerInfoEvent>(ins.GetPSDValue());
            //该客户端第一次进入游戏场景
            //生成自身
            GameObject selfGo = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Character/Player"),
                new Vector3(pd.x,pd.y,pd.z),Quaternion.Euler(pd.ex,pd.ey,pd.ez));
            this.GetSystem<INetSystem>().SetPlayerPMC(selfGo.GetComponent<PlayerMovementController>());
            //Debug.Log("长度是："+l.Count);
            selfGo.name = pd.id;
            selfGo.GetComponent<PlayerMovementController>().InitDressState(ins.GetPSDValue().PSDtoPD());
            //生成其他玩家
            for (int i = 0; i < l.Count; i++)
            {
                //Debug.Log("生成其他玩家"+l[i].id);
                GameObject otherGo = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Character/SyncPlayer"),
               new Vector3(l[i].x, l[i].y, l[i].z), Quaternion.Euler(l[i].ex, l[i].ey, l[i].ez));
                otherGo.name = l[i].id;
                SyncPMCtrl spmc = otherGo.GetComponent<SyncPMCtrl>();
                spmc.isAI= l[i].isAI;
                spmc.InitDressState(l[i]);
                ins.AddNewPlayerData(l[i], spmc);
            }
            GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Scene/SilvermoonCity"));
        }
        else
        {
            //收到其他新玩家进入游戏场景
            //生成新进入玩家的角色游戏物体
            GameObject newGo = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Character/SyncPlayer"),
                new Vector3(pd.x, pd.y, pd.z), Quaternion.Euler(pd.ex, pd.ey, pd.ez));
            newGo.name = pd.id;
            SyncPMCtrl spmc = newGo.GetComponent<SyncPMCtrl>();
            spmc.isAI = pd.isAI;
            spmc.InitDressState(pd);
            ins.AddNewPlayerData(pd, spmc);
        }
    }
}

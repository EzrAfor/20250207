using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTEnterGameScene : PTBase 
{ 
   public PTEnterGameScene()
    {
        protoName = "PTEnterGameScene";
    }
    public string playDatasListJson = "";//服务器回复其他玩家的信息
    public string enterGamePlayerDataJson = "";//客户端发送自己的信息
}

//获取玩家所有信息列表
public class PTGetPlayerDatas : PTBase
{
    public PTGetPlayerDatas()
    {
        protoName = "PTGetPlayerDatas";
    }
    //服务器回复
    public string playerDatasJson = "";

}
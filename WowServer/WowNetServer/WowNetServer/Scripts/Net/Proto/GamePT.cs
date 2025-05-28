using System.Collections;
using System.Collections.Generic;
//*****************************************
//创建人： Trigger 
//功能说明：进入游戏场景后相关的协议
//***************************************** 
public class PTEnterGameScene : PTBase
{
    public PTEnterGameScene() { protoName = "PTEnterGameScene"; }
    //客户端发 当前这个客户端玩家想玩的角色所处所有角色所在列表中的ID位置
    public int choiceID;
    //服务器回 其他玩家角色的信息
    public string playerDatasListJson = "";
    //服务器回 自己角色的信息
    public string enterGamePlayerDataJson = "";
}
/// <summary>
/// 获取玩家所有角色信息的列表
/// </summary>
public class PTGetPlayerDatas : PTBase
{
    public PTGetPlayerDatas() { protoName = "PTGetPlayerDatas"; }
    //服务器回
    public string playerDatasJson = "";
}
/// <summary>
/// 获取玩家背包物品信息
/// </summary>
public class PTGetInventoryItemList:PTBase
{
    public PTGetInventoryItemList() { protoName = "PTGetInventoryItemList"; }
    //服务器回
    public string inventoryItemListJson = "";
}

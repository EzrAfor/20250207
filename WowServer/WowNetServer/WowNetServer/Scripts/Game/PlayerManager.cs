using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
/// <summary>
/// 玩家管理
/// </summary>
public class PlayerManager
{
    public static PlayerManager Instance;
    private Dictionary<string, Player> playersDict = new Dictionary<string, Player>();
    private RoleAttributeValueData[] basicRoleValueList;
    private RoleAttributeValueData[] growthRoleValueList;

    public void ReadNPCDatas()
    {
        string bjsonPath = "BasicRoleAttributeValue.json";
        string bfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, bjsonPath);
        string bjsonStr = File.ReadAllText(bfilePath);
        basicRoleValueList = JsonConvert.DeserializeObject<RoleAttributeValueData[]>(bjsonStr);

        string gjsonPath = "GrowthRoleAttributeValue.json";
        string gfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, gjsonPath);
        string gjsonStr = File.ReadAllText(gfilePath);
        growthRoleValueList = JsonConvert.DeserializeObject<RoleAttributeValueData[]>(gjsonStr);

        string mjsonPath = "MapAIInfo.json";
        string mfilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, mjsonPath);
        string mjsonStr = File.ReadAllText(mfilePath);
        CharacterMapDatasInfo cmdi = JsonConvert.DeserializeObject<CharacterMapDatasInfo>(mjsonStr);
        foreach (var item in cmdi.mapGridPointDatas)
        {
            Player npcPlayer = new AIPlayer(null,item.pd.PDToPSD(),item.pathPoints);
            AddPlayer(npcPlayer);
            GameManager.Instance.SetDefaultGrid(npcPlayer);
        }
    }
    /// <summary>
    /// 获取职业基础属性值
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public RoleAttributeValueData GetBasicRoleAttributeValueData(ROLE role)
    {
        return basicRoleValueList[(int)role];
    }
    /// <summary>
    /// 获取职业成长属性值
    /// </summary>
    /// <param name="role"></param>
    /// <returns></returns>
    public RoleAttributeValueData GetGrowthRoleAttributeValueData(ROLE role)
    {
        return growthRoleValueList[(int)role];
    }

    /// <summary>
    /// 判断该玩家是否在线
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public bool PlayerIsOnline(string id)
    {
        return playersDict.ContainsKey(id);
    }
    /// <summary>
    /// 获取玩家对象
    /// </summary>
    /// <param name="id">玩家ID，并不是角色ID</param>
    /// <returns></returns>
    public Player GetPlayer(string id)
    {
        return playersDict[id];
    }
    /// <summary>
    /// 添加玩家
    /// </summary>
    /// <param name="player"></param>
    public void AddPlayer(Player player)
    {
        //Console.WriteLine("添加"+ player.id);
        playersDict.Add(player.id,player);
    }
    /// <summary>
    /// 移除玩家
    /// </summary>
    /// <param name="player"></param>
    public void RemovePlayer(string id)
    {
        //Console.WriteLine("移除" + id);
        playersDict.Remove(id);
    }
    /// <summary>
    /// 获取在线玩家列表
    /// </summary>
    public PlayerDatasList GetAllPlayer()
    {
        //List<PlayerData> playerDatas = new List<PlayerData>();
        //foreach (Player item in playersDict.Values)
        //{
        //    playerDatas.Add(item.pd);
        //}
        PlayerDatasList pdl = new PlayerDatasList();
        //pdl.playerDatas = playerDatas;
        return pdl;
    }
    /// <summary>
    /// 广播协议消息
    /// </summary>
    public void BroadcastPTMessage(PTBase pt)
    {
        foreach (Player player in playersDict.Values)
        {
            player.Send(pt);
        }
    }
    /// <summary>
    /// 获取对应角色ID的玩家引用
    /// </summary>
    public Player GetPlayerFromCharacterID(string id)
    {
        foreach (Player player in playersDict.Values)
        {
            if (player.psd.id==id)
            {
                return player;
            }
        }
        return null;
    }

    //private float AOIRange = 15;

    //private int size = 15;
    //public List<Player>[,] AOIPlayerArray;//长/size 宽/size
    ///// <summary>
    ///// AOI检测
    ///// </summary>
    //public void AOIDetect(Player player)
    //{
    //    ////1.距离法
    //    //Vector3 playerPos = new Vector3(player.pd.x,player.pd.y,player.pd.z);
    //    //foreach (var p in playersDict)
    //    //{
    //    //    if (player == p.Value)
    //    //    {
    //    //        //是自身
    //    //        continue;
    //    //    }
    //    //    //是其他玩家
    //    //    Vector3 targetPos = new Vector3(p.Value.pd.x,p.Value.pd.y,p.Value.pd.z);
    //    //    if (Vector3.Distance(playerPos,targetPos)<=AOIRange)
    //    //    {
    //    //        player.EnterAOI(p.Value);
    //    //    }
    //    //    else
    //    //    {
    //    //        player.LeaveAOI(p.Value);
    //    //    }
    //    //}

    //    //2.格子法
    //    int indexX= (int)player.pd.x / size;
    //    int indexY = (int)player.pd.y / size;
    //    player.JudgeAOIGrid(indexX,indexY);
    //}
    ///// <summary>
    ///// AOI消息广播
    ///// </summary>
    ///// <param name="pt"></param>
    //public void AOIBroadcastPTMessage(PTBase pt,int x,int y)
    //{        
    //    for (int i = 0; i < AOIPlayerArray[x, y].Count; i++)
    //    {
    //        AOIPlayerArray[x, y][i].Send(pt);
    //    }
    //}
}

/// <summary>
/// 地图块索引
/// </summary>
[Serializable]
public struct GridIndex
{
    public int x;
    public int y;
    public int z;
}
/// <summary>
/// 单个人物AI的地图块信息
/// </summary>
[Serializable]
public class CharacterMapData
{
    public GridIndex gi;
    public PlayerData pd;
    public List<GridIndex> pathPoints;//巡逻路点
}
/// <summary>
/// 地图上所有的信息块（用于保存到json文件里）
/// </summary>
[Serializable]
public class CharacterMapDatasInfo
{
    public List<CharacterMapData> mapGridPointDatas;
}

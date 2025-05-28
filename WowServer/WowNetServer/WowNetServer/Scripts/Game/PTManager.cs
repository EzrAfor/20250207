using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
/// <summary>
/// 消息协议管理
/// </summary>
public class PTManager
{
    public static PTManager Instance;
    public delegate void PTListener(MsgPT msg);
    private Dictionary<string, PTListener> ptListenersDict = new Dictionary<string, PTListener>();
    //json解码编码器
    private JavaScriptSerializer jss = new JavaScriptSerializer();
    public void RegistPTListener(string ptName, PTListener listener)
    {
        if (ptListenersDict.ContainsKey(ptName))
        {
            ptListenersDict[ptName] += listener;
        }
        else
        {
            ptListenersDict[ptName] = listener;
        }
    }
    public void UnregistPTListener(string ptName, PTListener listener)
    {
        if (ptListenersDict.ContainsKey(ptName))
        {
            ptListenersDict[ptName] -= listener;
        }
    }
    public void SendPTEvent(string ptName, MsgPT msg)
    {
        if (ptListenersDict.ContainsKey(ptName))
        {
            //Console.WriteLine(ptName);
            ptListenersDict[ptName](msg);
        }
    }

    public void RegistAllMsgPTListener()
    {
        RegistPTListener("PTPing", OnPTPing);
        RegistPTListener("PTRegister", OnPTRegister);
        RegistPTListener("PTLogin", OnPTLogin);
        RegistPTListener("PTGetPlayerDatas", OnPTGetPlayerDatas);
        RegistPTListener("PTCreateNewCharacter", OnPTCreateNewCharacter);
        RegistPTListener("PTEnterGameScene", OnPTEnterGameScene);
        RegistPTListener("PTSyncCharacter", OnPTSyncCharacter);
        RegistPTListener("PTSyncSetChoiceTarget", OnPTSyncSetChoiceTarget);
        RegistPTListener("PTSyncAttack", OnPTSyncAttack);
        RegistPTListener("PTGetInventoryItemList", OnPTGetInventoryItemList);
    }
    /// <summary>
    /// 心跳检测
    /// </summary>
    private void OnPTPing(MsgPT msg)
    {
        msg.clientObject.lastPingTime = NetManager.Instance.GetTimeStamp();
        NetManager.Instance.Send(msg.clientObject,new PTPong());
    }
    /// <summary>
    /// 注册
    /// </summary>
    private void OnPTRegister(MsgPT msg)
    {
        PTRegister pt = (PTRegister)msg.pt;
        if (DbManager.Instance.RegistNewAccount(pt.id,pt.pw))
        {
            DbManager.Instance.CreatePlayerList(pt.id);
            pt.result = 0;
        }
        else
        {
            pt.result = 1;
        }
        NetManager.Instance.Send(msg.clientObject,pt);
    }
    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="msg"></param>
    private void OnPTLogin(MsgPT msg)
    {
        PTLogin pt =(PTLogin)msg.pt;
        //密码校验
        if (!DbManager.Instance.CheckPassword(pt.id,pt.pw))
        {
            pt.result = 1;
            NetManager.Instance.Send(msg.clientObject,pt);
            return;
        }
        //已登录，不能重复登录
        if (msg.clientObject.player!=null)
        {
            pt.result = 1;
            NetManager.Instance.Send(msg.clientObject, pt);
            return;
        }
        //如果已经登录，进入游戏，踢下线
        if (PlayerManager.Instance.PlayerIsOnline(pt.id))
        {
            //发送踢下线协议
        }
        //正常登录
        Player player = new Player(msg.clientObject);
        player.id = pt.id;
        player.psdl = DbManager.Instance.GetPlayerDatasList(pt.id);
        if (player.psdl==null)
        {
            player.psdl = new PlayerSaveDatasList();
            player.psdl.playerSaveDatas = new List<PlayerSaveData>();
        }
        else
        {
            if (player.psdl.playerSaveDatas == null)
            {
                player.psdl.playerSaveDatas = new List<PlayerSaveData>();
            }
            else
            {
                if (player.psdl.playerSaveDatas.Count > 0)
                {
                    player.psd = player.psdl.playerSaveDatas[player.psdl.choiceID];
                }
            }
        }       
        pt.result = 0;
        msg.clientObject.player = player;
        NetManager.Instance.Send(msg.clientObject,pt);
    }
    /// <summary>
    /// 获取玩家角色信息列表
    /// </summary>
    /// <param name="msg"></param>
    private void OnPTGetPlayerDatas(MsgPT msg)
    {
        PTGetPlayerDatas pt = (PTGetPlayerDatas)msg.pt;
        PlayerSaveDatasList psdl = msg.clientObject.player.psdl;
        PlayerDatasList pdl = new PlayerDatasList();
        pdl.playerDatas = new List<PlayerData>();
        pdl.choiceID = psdl.choiceID;
        for (int i = 0; i < psdl.playerSaveDatas.Count; i++)
        {
            pdl.playerDatas.Add(psdl.playerSaveDatas[i].PSDtoPD());
        }     
        pt.playerDatasJson = jss.Serialize(pdl);
        NetManager.Instance.Send(msg.clientObject, pt);
    }
    /// <summary>
    /// 新角色创建
    /// </summary>
    /// <param name="msg"></param>
    private void OnPTCreateNewCharacter(MsgPT msg)
    {
        PTCreateNewCharacter pnc = (PTCreateNewCharacter)msg.pt;
        PlayerSaveData psd = jss.Deserialize<PlayerData>(pnc.playerDataJson).PDToPSD();
        psd.rd = PlayerManager.Instance.GetBasicRoleAttributeValueData(psd.role)+
            PlayerManager.Instance.GetBasicRoleAttributeValueData(psd.role)*psd.level;
        psd.hp = psd.rd.HP;
        psd.mana = psd.rd.mana;
        psd.slotsList = new List<SlotData>();
        for (int i = 0; i < 30; i++)
        {
            SlotData sd = new SlotData();
            sd.id = i;
            Random r = new Random();
            sd.num = r.Next(1,100);
            psd.slotsList.Add(sd);
        }
        msg.clientObject.player.psdl.playerSaveDatas.Add(psd);
        msg.clientObject.player.psdl.choiceID = msg.clientObject.player.psdl.playerSaveDatas.Count - 1;
        if (DbManager.Instance.UpdatePlayerDatasList
            (msg.clientObject.player.id, msg.clientObject.player.psdl))
        {
            pnc.result = 0;
            //创建成功更新json信息，因为设置了职业相关属性值
            pnc.playerDataJson = jss.Serialize(psd.PSDtoPD());
        }
        else
        {
            pnc.result = 1;
        }
        NetManager.Instance.Send(msg.clientObject,pnc);
    }
    /// <summary>
    /// 进入游戏场景
    /// </summary>
    /// <param name="msg"></param>
    private void OnPTEnterGameScene(MsgPT msg)
    {
        PTEnterGameScene ptgs = (PTEnterGameScene)msg.pt;
        //PlayerData pd= jss.Deserialize<PlayerData>(ptgs.enterGamePlayerDataJson);
        msg.clientObject.player.psd = msg.clientObject.player.psdl.playerSaveDatas[ptgs.choiceID];
        ptgs.enterGamePlayerDataJson = jss.Serialize(msg.clientObject.player.psd.PSDtoPD());
        //msg.clientObject.player.id = pd.id;
        Player player = msg.clientObject.player;
        //Console.WriteLine(player.pd.id);
        PlayerManager.Instance.AddPlayer(player);
        GameManager.Instance.SetDefaultGrid(player);
        //Console.WriteLine(player.AOIGridX+":" + player.AOIGridY);
        //ptgs.playerDatasListJson= jss.Serialize(PlayerManager.Instance.GetAllPlayer());
        PlayerDatasList pdl = new PlayerDatasList();
        List<Player> players = GameManager.Instance.PlayersInAOIGrid(player.AOIGridX, player.AOIGridY);
        //Console.WriteLine(players.Count);
        List<PlayerData> playerDatas = new List<PlayerData>();
        for (int i = 0; i < players.Count; i++)
        {
            //Console.WriteLine(players[i].pd.id);
            playerDatas.Add(players[i].psd.PSDtoPD());
        }
        pdl.playerDatas = playerDatas;
        ptgs.playerDatasListJson = JsonConvert.SerializeObject(pdl);
        //Console.WriteLine(ptgs.playerDatasListJson);
        GameManager.Instance.AOIBoardcastPTMessage(ptgs, player.AOIGridX, player.AOIGridY);
    }
    /// <summary>
    /// 同步人物状态
    /// </summary>
    /// <param name="msg"></param>
    private void OnPTSyncCharacter(MsgPT msg)
    {
        PTSyncCharacter ptsc = (PTSyncCharacter)msg.pt;
        //Console.WriteLine(msg.clientObject.socket.RemoteEndPoint);
        Player player = msg.clientObject.player;
        if (player==null)
        {
            Console.WriteLine("Player对象为空");
            return;
        }
        //防作弊
        //位置更新判定
        if (Math.Abs(player.psd.x - ptsc.cd.x) > 5 ||
            Math.Abs(player.psd.y - ptsc.cd.y) > 5 || 
            Math.Abs(player.psd.z - ptsc.cd.z) > 5)
        {
            Console.WriteLine("玩家:"+ player.id+" 疑似作弊");
        }
        //位置判定
        if (GameManager.Instance.JudgePointInMap(ptsc.cd.x,ptsc.cd.y,ptsc.cd.z))
        {
            //位置正确
            player.psd.x = ptsc.cd.x;
            player.psd.y = ptsc.cd.y;
            player.psd.z = ptsc.cd.z;
        }
        else
        {
            Console.WriteLine("玩家:"+player.psd.id+"位置异常,纠正为上一次的位置");
            ptsc.cd.x = player.psd.x;
            ptsc.cd.y = player.psd.y;
            ptsc.cd.z = player.psd.z;
        }      
        player.psd.ex = ptsc.cd.ex;
        player.psd.ey = ptsc.cd.ey;
        player.psd.ez = ptsc.cd.ez;
        player.psd.fx = ptsc.cd.fx;
        player.psd.fy = ptsc.cd.fy;
        player.psd.fz = ptsc.cd.fz;
        player.psd.characterState = ptsc.cd.characterState;
        //攻击判定
        player.JudgeAttackEvent();
        //受击判定
        player.JudgeHitEvent();
        ptsc.cd.id=player.psd.id;
        ptsc.cd.hp = player.psd.hp;
        ptsc.cd.mana = player.psd.mana;
        ptsc.cd.characterState = player.psd.characterState;
        //if (ptsc.characterState!=CHARACTERSTATE.IDLE)
        //{
        //    Console.WriteLine(ptsc.id + ptsc.characterState);
        //}       
        //Console.WriteLine(msg.clientObject.player.id+":"+ ptsc.id);

        //1.距离法
        //PlayerManager.Instance.AOIDetect(player);
        //player.AOIBroadcastPTMessage(ptsc);
        //2.格子法
        //PlayerManager.Instance.AOIDetect(player);
        //PlayerManager.Instance.AOIBroadcastPTMessage(ptsc,player.AOIGridX,player.AOIGridY);
        //3.九宫格子法
        GameManager.Instance.AOIDetect(player,ptsc);
        //GameManager.Instance.AOIBoardcastPTMessage(ptsc,player.AOIGridX,player.AOIGridY);

        //PlayerManager.Instance.BroadcastPTMessage(ptsc);
    }
    /// <summary>
    /// 同步所有玩家选择的目标
    /// </summary>
    /// <param name="pt"></param>
    private void OnPTSyncSetChoiceTarget(MsgPT msg)
    {
        PTSyncSetChoiceTarget p = (PTSyncSetChoiceTarget)msg.pt;
        Player player = msg.clientObject.player;
        //当前客户端发过来的角色名是否正确
        if (player.psd.id!=p.pID)
        {
            //不正确则按服务器上的ID进行修正
            p.pID = player.psd.id;
        }
        if (p.tID!=null)
        {
            Player targetPlayer = PlayerManager.Instance.
            GetPlayerFromCharacterID(p.tID);
            if (targetPlayer != null)
            {
                player.target = targetPlayer;
            }
            else
            {
                p.tID = null;
                player.target = null;
            }
        }
        else
        {
            player.target = null;
        }
        //Console.WriteLine(player.psd.id+player.target);
        GameManager.Instance.AOIBoardcastPTMessage(p, player.AOIGridX, player.AOIGridY);
        //PlayerManager.Instance.BroadcastPTMessage(p);
    }
    /// <summary>
    /// 判定并同步进入战斗状态
    /// </summary>
    /// <param name="msg"></param>
    private void OnPTSyncAttack(MsgPT msg)
    {
        PTSyncAttack p = (PTSyncAttack)msg.pt;
        Player player = msg.clientObject.player;
        //当前客户端发过来的角色名是否正确
        if (player.psd.id != p.pID)
        {
            //不正确则按服务器上的ID进行修正
            p.pID = player.psd.id;
        }
        p.canBeBattle= player.target.psd.characterState == CHARACTERSTATE.DEAD ? false : true;
        if (p.canBeBattle)
        {
            player.StartAttack();
        }
        //PlayerManager.Instance.BroadcastPTMessage(p);
        GameManager.Instance.AOIBoardcastPTMessage(p, player.AOIGridX, player.AOIGridY);
    }
    /// <summary>
    /// 获取玩家背包信息
    /// </summary>
    /// <param name="msg"></param>
    private void OnPTGetInventoryItemList(MsgPT msg)
    {
        PTGetInventoryItemList p = (PTGetInventoryItemList)msg.pt;
        InventoryItemList l = new InventoryItemList();
        l.slotsList = msg.clientObject.player.psd.slotsList;
        p.inventoryItemListJson= jss.Serialize(l);
        NetManager.Instance.Send(msg.clientObject,p);
    }
}
/// <summary>
/// 消息协议
/// </summary>
public struct MsgPT
{
    public ClientObject clientObject;
    public PTBase pt;
}

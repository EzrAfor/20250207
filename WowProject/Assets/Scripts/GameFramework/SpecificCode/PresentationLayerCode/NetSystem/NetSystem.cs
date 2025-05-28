using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Text;
using System;
using System.Linq;
using UnityEngine.Tilemaps;
//*****************************************
//创建人： Trigger 
//功能说明：网络系统
//***************************************** 
public class NetSystem : INetSystem
{
    private Socket socket;
    private ByteObject readBuff;
    private Queue<ByteObject> writeQueue;
    private bool isClosing;
    private bool isConnecting;
    public delegate void PTListener(PTBase pt);
    private Dictionary<string, PTListener> ptListenersDict = new Dictionary<string, PTListener>();
    private List<PTBase> ptList=new List<PTBase>();
    private int ptListCount;
    private const int MAXPTUPDATENUM=10;
    private bool usePingPong;
    private int pingPongInterval = 30;
    private float lastPingTime;
    private float lastPongTime;
    private PlayerSaveData currentPSD;//当前客户端进入游戏的角色信息
    private PlayerData currentTargetPD;//当前客户端玩家选择的目标的角色信息
    private List<PlayerData> syncPdList=new List<PlayerData>();//所有需要同步的其他客户端玩家角色信息的列表
    private Dictionary<string, SyncPMCtrl> syncPMCDict=new Dictionary<string, SyncPMCtrl>();//所有需要同步的其他客户端玩家角色的字典
    private PlayerMovementController playermCtrl;//当前客户端玩家的引用
    private int choiceID;
    
    #region 网络协议消息事件
    public void RegistPTListener(string ptName,PTListener listener)
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
    public void SendPTEvent(string ptName, PTBase pt)
    {
        if (ptListenersDict.ContainsKey(ptName))
        {
            ptListenersDict[ptName](pt);
        }
    }
    #endregion
    /// <summary>
    /// 关闭客户端
    /// </summary>
    public void Close()
    {
        if (writeQueue.Count>0)
        {
            isClosing = true;
        }
        else
        {
            socket.Close();
        }
    }

    private void InitState()
    {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.NoDelay = true;
        readBuff = new ByteObject();
        writeQueue = new Queue<ByteObject>();
        isClosing = false;
        usePingPong = true;
        lastPingTime = lastPongTime = Time.time;
        RegistPTListener("PTPong", OnPTPong);
        //架构事件
        this.RegistEvent<OnConnectSucceedEvent>(OnConnectSucceed);
        this.RegistEvent<OnConnectFailEvent>(OnConnectFail);
        //网络协议消息
        this.SendCommand<RegistPTListenerCommand>(new PTSrc() 
        {ptName= "PTSyncCharacter",listener= OnPTSyncCharacter });
        this.SendCommand<RegistPTListenerCommand>(new PTSrc()
        { ptName = "PTSyncSetChoiceTarget", listener = OnPTSyncSetChoiceTarget });
        this.SendCommand<RegistPTListenerCommand>(new PTSrc()
        { ptName = "PTSyncAttack", listener = OnPTSyncAttack });
        this.SendCommand<RegistPTListenerCommand>(new PTSrc()
        { ptName = "PTSyncEnterOrLeaveAOI", listener = OnPTSyncEnterOrLeaveAOI });
        this.SendCommand<RegistPTListenerCommand>(new PTSrc() 
        { ptName = "PTGetInventoryItemList", listener = OnPTGetInventoryItemList });
    }

    public void Connect(string ip, int port)
    {
        if (socket!=null&&socket.Connected)
        {
            return;
        }
        if (isConnecting)
        {
            return;
        }
        InitState();
        isConnecting = true;
        socket.BeginConnect(ip,port,ConnectCallback,socket);
    }

    private void ConnectCallback(IAsyncResult iar)
    {
        try
        {
            Socket socket = (Socket)iar.AsyncState;
            socket.EndConnect(iar);
            Receive();
            this.SendEvent<OnConnectSucceedEvent>();
            isConnecting = false;
        }
        catch (SocketException se)
        {
            this.SendEvent<OnConnectFailEvent>();
            Debug.Log(se);
        }
    }

    public void Init()
    {
        
    }

    public void Send(PTBase msg)
    {
        if (isClosing)
        {
            return;
        }
        byte[] ptBytes= PT.EncodeName(msg).Concat(PT.EncodeBody(msg)).ToArray();
        Int16 length = (Int16)ptBytes.Length;
        byte[] lengthBytes = BitConverter.GetBytes(length);
        if (!BitConverter.IsLittleEndian)
        {
            lengthBytes.Reverse();
        }
        byte[] sendBytes = lengthBytes.Concat(ptBytes).ToArray();
        ByteObject bo = new ByteObject(sendBytes);
        int count = 0;
        lock (writeQueue)
        {
            writeQueue.Enqueue(bo);
            count = writeQueue.Count;
        }
        if (count == 1)
        {
            socket.BeginSend(bo.bytes, bo.readIndex, bo.dataLength, SocketFlags.None, SendCallback, socket);
        }
    }

    private void SendCallback(IAsyncResult iar)
    {
        try
        {
            Socket socket = (Socket)iar.AsyncState;
            int length = socket.EndSend(iar);
            ByteObject bo;
            lock (writeQueue)
            {
                bo = writeQueue.First();
            }
            bo.readIndex += length;
            if (bo.dataLength == 0)
            {
                lock (writeQueue)
                {
                    writeQueue.Dequeue();
                    bo = writeQueue.First();
                }
            }
            if (bo!=null)
            {
                socket.BeginSend(bo.bytes, bo.readIndex, bo.dataLength, SocketFlags.None, SendCallback, socket);
            }
            else if (isClosing)
            {
                socket.Close();
            }
        }
        catch (SocketException se)
        {
            Debug.Log("发送失败：" + se);
        }
    }

    private void Receive()
    {
        socket.BeginReceive(readBuff.bytes, readBuff.writeIndex, readBuff.remainLength, SocketFlags.None, ReceiveCallback, socket);
    }

    private void ReceiveCallback(IAsyncResult iar)
    {
        try
        {
            Socket socket = (Socket)iar.AsyncState;
            int length = socket.EndReceive(iar);
            readBuff.writeIndex += length;
            //Debug.Log("接收数据长度" + length);
            HandleReceiveData();
            if (readBuff.remainLength<8)
            {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.dataLength*2);
            }
            Receive();
        }
        catch (SocketException se)
        {
            Debug.Log("接收失败：" + se);
        }
    }

    private void HandleReceiveData()
    {
        if (readBuff.dataLength<=2)
        {
            return;
        }
        //00000010(低地址)  00000001  1*2+1*256=258 小端
        //Debug.Log("读取索引为：" + readBuff.readIndex);
        Int16 bodyLength = (Int16)(readBuff.bytes[readBuff.readIndex] | readBuff.bytes[readBuff.readIndex+1] << 8);
        //Int16 bodyLength = BitConverter.ToInt16(readBuff,0);
        //Debug.Log("消息总体长度为：" + bodyLength);
        if (readBuff.dataLength<bodyLength+2)
        {
            return;
        }
        readBuff.readIndex += 2;
        //解析协议名
        int nameCount = 0;
        string protoName = PT.DecodeName(readBuff.bytes,readBuff.readIndex,out nameCount);
        if (protoName=="")
        {
            Debug.Log("协议解析失败");
            return;
        }
        readBuff.readIndex += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        PTBase ptBase= PT.DecodeBody(protoName,readBuff.bytes,readBuff.readIndex, bodyCount);
        readBuff.readIndex += bodyCount;
        readBuff.CheckAndMoveBytes();
        lock (ptList)
        {
            ptList.Add(ptBase);
            ptListCount++;
        }
        HandleReceiveData();
    }

    public void Update()
    {
        UpdatePT();
        UptatePingPong();
    }
    /// <summary>
    /// 更新协议状态并触发相关协议事件
    /// </summary>
    private void UpdatePT()
    {
        if (ptListCount<=0)
        {
            return;
        }
        for (int i = 0; i < MAXPTUPDATENUM; i++)
        {
            PTBase ptBase = null;
            lock (ptList)
            {
                if (ptList.Count>0)
                {
                    ptBase = ptList[0];
                    ptList.RemoveAt(0);
                    ptListCount--;
                }
            }
            if (ptBase!=null)
            {
                SendPTEvent(ptBase.protoName,ptBase);
            }
            else
            {
                break;
            }
        }
    }
    /// <summary>
    /// 心跳机制检测
    /// </summary>
    private void UptatePingPong()
    {
        if (!usePingPong)
        {
            return;
        }
        //客户端向服务器发送ping协议
        if (Time.time-lastPingTime>pingPongInterval)
        {
            Send(new PTPing());
            lastPingTime = Time.time;
        }
        //客户端检测是否接收到服务器发送的pong协议
        if (Time.time-lastPongTime> pingPongInterval*4)
        {
            Close();
        }
    }

    private void OnPTPong(PTBase pt)
    {
        lastPongTime = Time.time;
    }

    /// <summary>
    /// 连接服务器成功
    /// </summary>
    private void OnConnectSucceed(object obj)
    {
        Debug.Log("客户端连接成功");
    }
    /// <summary>
    /// 连接服务器失败
    /// </summary>
    private void OnConnectFail(object obj)
    {
        Debug.Log("客户端连接失败");
    }
    /// <summary>
    /// 设置玩家当前信息值
    /// </summary>
    /// <param name="pd"></param>
    public void SetPSDValue(PlayerSaveData psd)
    {
        currentPSD = psd;
    }
    /// <summary>
    /// 设置当前玩家选择的人物编号
    /// </summary>
    /// <param name="value"></param>
    public void SetChoiceID(int value)
    {
        choiceID = value;
    }
    /// <summary>
    /// 获取当前玩家选择的人物编号
    /// </summary>
    /// <returns></returns>
    public int GetChoiceID()
    {
        return choiceID;
    }

    /// <summary>
    /// 设置当前客户端玩家的pmc引用
    /// </summary>
    /// <param name="pd"></param>
    public void SetPlayerPMC(PlayerMovementController pmc)
    {
        playermCtrl = pmc;
        playermCtrl.SetPlayerDataValue(currentPSD.rd);
    }
    /// <summary>
    /// 获取玩家当前信息值
    /// </summary>
    /// <returns></returns>
    public PlayerSaveData GetPSDValue()
    {
        return currentPSD;
    }

    /// <summary>
    /// 设置选择目标当前信息值
    /// </summary>
    /// <param name="pd"></param>
    public void SetTargetPDValue(string id)
    {
        if (id=="")
        {
            currentTargetPD = null;
        }
        else
        {
            for (int i = 0; i < syncPdList.Count; i++)
            {
                if (syncPdList[i].id == id)
                {
                    currentTargetPD = syncPdList[i];
                }
            }
        }
    }
    /// <summary>
    /// 获取选择目标当前信息值
    /// </summary>
    /// <returns></returns>
    public PlayerData GetTargetPDValue()
    {
        return currentTargetPD;
    }

    /// <summary>
    /// 设置当前游戏中所有玩家列表信息值
    /// </summary>
    /// <param name="pd"></param>
    public void SetPDListValue(List<PlayerData> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].id != currentPSD.id)
            {
                syncPdList.Add(list[i]);
            }
        }
    }
    /// <summary>
    /// 获取当前游戏中其他列表当前信息值
    /// </summary>
    /// <returns></returns>
    public List<PlayerData> GetSyncPDListValue()
    {
        return syncPdList;
    }
    /// <summary>
    /// 新进入游戏玩家的角色信息
    /// </summary>
    public void AddNewPlayerData(PlayerData pd, SyncPMCtrl spmc)
    {
        if (!syncPdList.Contains(pd))
        {
            syncPdList.Add(pd);
        }
        syncPMCDict.Add(pd.id,spmc);
    }
    /// <summary>
    /// 退出游戏玩家的角色信息
    /// </summary>
    public void ExitPlayerData(PlayerData pd)
    {
        syncPdList.Remove(pd);
        syncPMCDict.Remove(pd.id);
    }
    /// <summary>
    /// 同步人物信息
    /// </summary>
    /// <param name="pt"></param>
    private void OnPTSyncCharacter(PTBase pt)
    {
        PTSyncCharacter ptsc = (PTSyncCharacter)pt;
        if (ptsc.cd.id==currentPSD.id)//接收到的信息是当前客户端玩家自己的信息
        {
            currentPSD.hp = ptsc.cd.hp; 
            currentPSD.mana = ptsc.cd.mana;
            playermCtrl.cFSM.ChangeState(ptsc.cd.characterState);
            this.SendEvent<UpdatePlayerInfoEvent>(currentPSD);
            return;
        }
        else//其他玩家的信息
        {
            //当前客户端选择的目标得不为空，发过来的这个信息就是我们当前客户端玩家
            //选中的目标玩家的信息
            if (currentTargetPD!=null&& ptsc.cd.id==currentTargetPD.id)
            {
                currentTargetPD.hp = ptsc.cd.hp;
                currentTargetPD.mana = ptsc.cd.mana;
                this.SendEvent<UpdateTargetInfoEvent>(currentTargetPD);
            }
            //Debug.Log("更新"+ ptsc.cd.id + "人物的信息");
            //Debug.Log(ptsc.cd);
            //Debug.Log(syncPMCDict);
            if (syncPMCDict!=null&&syncPMCDict.ContainsKey(ptsc.cd.id))
            {
                //Debug.Log("更新"+ ptsc.cd.id+"人物的信息");
                syncPMCDict[ptsc.cd.id].SyncPosAndRot(ptsc);
            }
        }
       
    }
    /// <summary>
    /// 同步所有玩家选择的目标
    /// </summary>
    /// <param name="pt"></param>
    private void OnPTSyncSetChoiceTarget(PTBase pt)
    {
        PTSyncSetChoiceTarget p = (PTSyncSetChoiceTarget)pt;
        //当前客户端玩家自己设置目标
        if (p.pID==currentPSD.id)
        {
            SetTargetPDValue(p.tID);
            this.SendEvent<UpdateTargetInfoEvent>(currentTargetPD);
        }
        else
        {
            //其他需要同步的玩家去设置目标
            if (syncPMCDict.ContainsKey(p.pID))//安全校验
            {
                if (syncPMCDict.ContainsKey(p.tID))//校验目标,存在于需要同步的玩家字典里
                {
                    syncPMCDict[p.pID].targetTrans = syncPMCDict[p.tID].transform;
                }
                else//当前需要设置为其他人物目标的人是我们自己（当前客户端玩家）
                {
                    if (p.tID=="")
                    {
                        syncPMCDict[p.pID].targetTrans = null;
                    }
                    else
                    {
                        syncPMCDict[p.pID].targetTrans = playermCtrl.transform;
                    }
                }
            }
        }

    }
    /// <summary>
    /// 判定并同步攻击状态
    /// </summary>
    /// <param name="pt"></param>
    public void OnPTSyncAttack(PTBase pt)
    {
        PTSyncAttack p = (PTSyncAttack)pt;
        if (p.pID==currentPSD.id)//当前客户端进入战斗状态
        {
            //Debug.Log("当前客户端玩家进入"+p.pID+ p.canBeBattle);
            playermCtrl.cFSM.beBattle = p.canBeBattle;
        }
        else
        {
            //Debug.Log("其他客户端玩家进入" + p.pID + p.canBeBattle);
            syncPMCDict[p.pID].cFSM.beBattle = p.canBeBattle;
        }
    }
    /// <summary>
    /// 有人物进入或退出AOI
    /// </summary>
    /// <param name="pt"></param>
    public void OnPTSyncEnterOrLeaveAOI(PTBase pt)
    {
        PTSyncEnterOrLeaveAOI p = (PTSyncEnterOrLeaveAOI)pt;
        if (p.otherPlayerCDList.Count>0)
        {
            for (int i = 0; i < p.otherPlayerCDList.Count; i++)
            {
                HandleAOIUpdateInfo(p, p.otherPlayerCDList[i]);
            }
        }
        else
        {
            HandleAOIUpdateInfo(p,p.pd);
        }
    }
    private void HandleAOIUpdateInfo(PTSyncEnterOrLeaveAOI p,PlayerData pd)
    {
        if (pd.id != currentPSD.id)//只处理其他人物
        {
            if (syncPMCDict.ContainsKey(pd.id))
            {
                syncPMCDict[pd.id].gameObject.SetActive(p.enterAOI);
                //先更新状态信息
                if (p.enterAOI)
                {
                    Vector3 pos = new Vector3(pd.x, pd.y, pd.z);
                    Vector3 rot = new Vector3(pd.ex, pd.ey, pd.ez);
                    syncPMCDict[pd.id].ImmediateUpdateSyncPosAndRot(pos, rot, pd.characterState);
                }
            }
            else
            {
                if (pd.id==""|| !p.enterAOI)
                {
                    return;
                }
                //第一次进入我们当前客户端玩家的视野范围时
                //生成
                //生成新进入玩家的角色游戏物体
                GameObject newGo = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Character/SyncPlayer"),
                    new Vector3(pd.x, pd.y, pd.z), Quaternion.Euler(pd.ex, pd.ey, pd.ez));
                newGo.name = pd.id;
                SyncPMCtrl spmc = newGo.GetComponent<SyncPMCtrl>();
                spmc.isAI = pd.isAI;
                spmc.InitDressState(pd);
                AddNewPlayerData(pd, spmc);
            }
        }
    }
    /// <summary>
    /// 收到服务器回的背包信息
    /// </summary>
    /// <param name="pt"></param>
    private void OnPTGetInventoryItemList(PTBase pt)
    {
        PTGetInventoryItemList p = (PTGetInventoryItemList)pt;
        currentPSD.slotsList = JsonUtility.FromJson<InventoryItemList>(p.inventoryItemListJson).slotsList;
        this.SendEvent<InitInventoryItemSlotsEvent>();
    }
}

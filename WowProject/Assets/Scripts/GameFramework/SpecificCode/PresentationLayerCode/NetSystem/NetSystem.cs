using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class NetSystem : INetSystem
{
    private Socket socket;
    private ByteObject readBuff ;
    private Queue<ByteObject> writeQueue ;
    private bool isClosing;
    private bool isConnecting;
    public delegate void PTListener(PTBase pt);
    private Dictionary<string,PTListener> ptListenerDict=new Dictionary<string,PTListener>();
    private List<PTBase> ptList = new List<PTBase>();
    private int ptListCount;
    private const int MAXPTUPDATENUM = 10;
    private bool usePingPong;
    private int pingPongInterval = 30;
    private float lastPingTime;
    private float lastPongTime;
    private PlayerData currentPD;//当前客户端进入游戏的角色信息
    private List<PlayerData> pdList;//所有玩家角色信息
    private List<PlayerData> syncPdList;//所有需要同步的其他客户端玩家角色信息
    private Dictionary<string, SyncPMCtrl> syncPMCDict;//所有需要同步的其他客户端玩家的字典

    public void RegistPTListener(string ptName,PTListener listener)
    {
        if (ptListenerDict.ContainsKey(ptName))
        {
            ptListenerDict[ptName] += listener;
        }
        else {
            ptListenerDict[ptName] = listener;
        }
    }

    public void UnregistPTListener(string ptName, PTListener listener) {
        if (ptListenerDict.ContainsKey(ptName))
        {
            ptListenerDict[ptName] -= listener;
        }
    }

    public void SendPTEvent(string ptName, PTBase pt) {
        if (ptListenerDict.ContainsKey(ptName))
        {
            ptListenerDict[ptName](pt);
        }

    }



    public void Close()
    {
        if (writeQueue.Count > 0)
        {
            isClosing = true;
        }
        else { 
        socket.Close();
        }
    }

    private void InitState() {
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//新建套接字
        socket.NoDelay = true;//优化性能
        readBuff = new ByteObject();
        writeQueue = new Queue<ByteObject>();
        isClosing = false;
        usePingPong = true;
        lastPingTime = lastPongTime = Time.time;
        RegistPTListener("PTPong",OnPTPong);

        //架构事件
        this.RegisterEvent<OnConnectSucceedEvent>(OnConnectSucceed);
        this.RegisterEvent<OnConnectFailEvent>(OnConnectFail);
        this.SendCommand<RegistPTListenerCommand>(new PTSrc() {ptName= "PTSyncCharacter",listener= OnPTSyncCharacter });

    }

    public void Connect(string ip, int port)
    {
        if (socket!=null&&socket.Connected)
        {
            return;
        }
        if (isConnecting) return;
        InitState();
        isConnecting=true;
        //socket.Connect(ip, port);//本机测试 同步连接
        socket.BeginConnect(ip, port, ConnectCallBack,socket);//异步

    }

    private void ConnectCallBack(IAsyncResult iar)
    {
        try
        {
            Socket socket = (Socket)iar.AsyncState;
            socket.EndConnect(iar);
            Receive();
            this.SendEvent<OnConnectSucceedEvent>();
            isConnecting = false;
        }
        catch (SocketException se) {
            this.SendEvent<OnConnectFailEvent>();
            Debug.Log(se);
        }
    }


    public void Init()
    {

    }

    public void Send(PTBase msg)
    {

        if(isClosing)return;
        byte[] ptBytes = PT.EncodeName(msg).Concat(PT.EncodeBody(msg)).ToArray();// 将长度信息与消息体合并
        Int16 length = (Int16)ptBytes.Length;
        byte[] lengthBytes = BitConverter.GetBytes(length);
        if (!BitConverter.IsLittleEndian) {//大小端处理
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
            socket.BeginSend(bo.bytes, bo.readIndex, bo.dataLength, SocketFlags.None, SendCallBack, socket);
        }
    }

    private void SendCallBack(IAsyncResult iar)
    {
        try
        {
            Socket socket = (Socket)iar.AsyncState;
            int length = socket.EndSend(iar);// 完成异步发送操作，并获取实际发送的字节数
            ByteObject bo;
            lock (writeQueue)
            {
                bo = writeQueue.First();                
            }            
            bo.readIndex += length;
            int count = 0;
            if (bo.dataLength == 0)
            {
                lock (writeQueue)
                {
                    writeQueue.Dequeue();
                    count = writeQueue.Count;
                }

            }
            if (count>0) {
                socket.BeginSend(bo.bytes, bo.readIndex, bo.dataLength, SocketFlags.None, SendCallBack, socket);
            } else if (isClosing) {
                socket.Close();
            }


        }
        catch (SocketException se)
        {
            Debug.Log("发送失败" + se);
        }
    }




    //public void Send(string msg)
    //{
    //    socket.Send(Encoding.Default.GetBytes(msg));//转成数组是因为send方法只能发送数组
    //}


    //public void Receive() {
    //    byte[] readBuff = new byte[1024];
    //    int count = socket.Receive(readBuff);
    //    string readStr = Encoding.UTF8.GetString(readBuff,0,count);
    //    Debug.Log("客户端接受消息"+readStr);
    //}

    private void Receive()
    {        
        socket.BeginReceive(readBuff.bytes, readBuff.writeIndex, readBuff.remainLength, SocketFlags.None, ReceiveCallBack, socket);
    }

    private void ReceiveCallBack(IAsyncResult iar)
    {
        try
        {
            Socket socket = (Socket)iar.AsyncState;
            int length = socket.EndReceive(iar);// 完成异步接收操作，并获取实际接收到的字节数
            readBuff.writeIndex += length;//将数组的索引进行累加
            HandleReceiveData();
            if (readBuff.remainLength < 8) {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.dataLength*2);
            }
            Receive();

        }
        catch (SocketException se)
        {
            Debug.Log("接收失败" + se);
        }
    }


    private void HandleReceiveData() {
        if (readBuff.dataLength <= 2)
        {// 检查是否有足够的数据来读取消息长度（至少需要2个字节） 这里的2代表的是2个字节 也就是读取消息长度
            return;
        }
        //进行大小端处理 进行或运算后可以得到完整的十六位值
        Int16 bodyLength = (Int16)(readBuff.bytes[readBuff.readIndex] | readBuff.bytes[readBuff.readIndex+1] << 8);
        //Int16 bodyLength =  BitConverter.ToInt16(readBuff,0);// 从缓冲区的前两个字节中读取消息长度
        if (readBuff.dataLength < bodyLength+2)
        {// 检查是否有足够的数据来包含整个消息体 加2是因为要包含前两个代表数据大小的字节
            return;
        }
        readBuff.readIndex += 2;
        int nameCount = 0;
        string protoName = PT.DecodeName(readBuff.bytes, readBuff.readIndex,out nameCount);//解析协议
        if (protoName == "") {
            Debug.Log("协议解析失败");
            return;
        }
        readBuff.readIndex += nameCount;
        int bodyCount = bodyLength - nameCount;
        PTBase ptBase = PT.DecodeBody(protoName, readBuff.bytes, readBuff.readIndex, bodyCount);

        readBuff.readIndex += bodyCount;
        readBuff.CheckAndMoveBytes();
        lock (ptList) { 
        ptList.Add(ptBase);
            ptListCount++;
        }
        HandleReceiveData();
    }

    public void Update() {
        UpdatePT();
        UpdatePingPong();
    }

    //心跳机制检测方法
    private void UpdatePingPong() {
        if (!usePingPong) return;
        if (Time.time - lastPingTime > pingPongInterval) {//客户端发送ping
            Send(new PTPing());
            lastPingTime = Time.time;
        }
        //客户端检测是否收到pong
        if (Time.time - lastPongTime > pingPongInterval*4 ) { 
        Close();
        }
    }

    //更新协议状态并触发相关协议事件   
    private void UpdatePT() {
        if (ptListCount <= 0) {
            return;
        }
        for (int i = 0; i < MAXPTUPDATENUM; i++)
        {
            PTBase ptBase = null;
            lock (ptList) {
                if (ptList.Count > 0) { 
                    ptBase = ptList[0];
                    ptList.RemoveAt(0);
                    ptListCount--;
                }
            }
            if (ptBase != null)
            {

                SendPTEvent(ptBase.protoName, ptBase);
            }
            else { 
            break;
            }
        }
    }

    private void OnPTPong(PTBase pt)
    {
        lastPongTime = Time.time;

    }

    private void OnConnectSucceed(object obj)
    {
        Debug.Log("客户端连接成功");
    }


    private void OnConnectFail(object obj)
    {
        Debug.Log("客户端连接失败");
    }

    public void SetPDValue(PlayerData pd) { //设置玩家当前信息值
        currentPD = pd;
    }

    public PlayerData GetPDValue() {//获取玩家当前信息值
        return currentPD;
    }


    public void SetPDListValue(List<PlayerData> list)
    { //设置所有玩家列表
        pdList = list;
        SetSyncPDListValue();
    }

    public List<PlayerData> GetPDListValue()
    {//获取所有玩家列表
        return pdList;
    }

    public void SetSyncPDListValue()
    { //设置其他玩家列表
        syncPdList = new List<PlayerData>();
        syncPMCDict = new Dictionary<string, SyncPMCtrl>();
        for (int i=0;i<syncPdList.Count;i++) {
            if (pdList[i].id != currentPD.id) {
                syncPdList.Add(pdList[i]);
            }
        }
    }

    public List<PlayerData> GetSyncPDListValue()
    {//获取其他玩家列表
        return syncPdList;
    }

    public void AddNewPlayerData(PlayerData pd, SyncPMCtrl spmc)
    {
        if (!syncPdList.Contains(pd))
        {
            syncPdList.Add(pd);
        }
        syncPMCDict.Add(pd.id,spmc);
    }

    public void ExitPlayerData(PlayerData pd)
    {
        syncPdList.Remove(pd);
        syncPMCDict.Remove(pd.id);
    }
    
    private void OnPTSyncCharacter(PTBase pt)
    {
        PTSyncCharacter ptsc = (PTSyncCharacter)pt;
        if (ptsc.id == currentPD.id) return;
        if (syncPMCDict.ContainsKey(ptsc.id))
        {
            syncPMCDict[ptsc.id].SyncPosAndRot(ptsc);
        }
    }

}

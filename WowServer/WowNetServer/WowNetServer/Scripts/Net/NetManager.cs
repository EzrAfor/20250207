using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.Net;
/// <summary>
/// 服务器总管理
/// </summary>
public class NetManager
{
    public static NetManager Instance;
    private Socket listenedSocket;
    //客户端Socket对象以及信息
    public Dictionary<Socket, ClientObject> clientObjectsDict = new Dictionary<Socket, ClientObject>();
    private List<Socket> checkReadSocketsList = new List<Socket>();
    private float pingPongInterval=30;
    /// <summary>
    /// 开始连接并监听消息
    /// </summary>
    /// <param name="listenedPort">端口号</param>
    public void StartServer(int listenedPort)
    {
        listenedSocket = new Socket(AddressFamily.InterNetwork,SocketType.Stream,ProtocolType.Tcp);
        IPAddress iPAddress = IPAddress.Parse("127.0.0.1");
        IPEndPoint iPEndPoint = new IPEndPoint(iPAddress, listenedPort);
        listenedSocket.Bind(iPEndPoint);
        listenedSocket.Listen(0);
        DbManager.Instance = new DbManager();
        DbManager.Instance.Connect("wow", "127.0.0.1", 3306, "root", "");
        Console.WriteLine("Wow服务器启动成功");
        GameManager.Instance = new GameManager();
        GameManager.Instance.ReadMapDatas();
        Console.WriteLine("游戏地图数据读取成功");
        ItemManager.Instance = new ItemManager();
        ItemManager.Instance.ReadItemDatas();
        Console.WriteLine("物品数据读取成功");
        PTManager.Instance = new PTManager();
        PTManager.Instance.RegistAllMsgPTListener();
        PlayerManager.Instance = new PlayerManager();
        PlayerManager.Instance.ReadNPCDatas();
        //异步方法
        //listenedSocket.BeginAccept(AcceptCallback, listenedSocket);
        while (true)
        {
            ResetCheckReadList();
            //多路复用
            Socket.Select(checkReadSocketsList, null,null,1000);
            for (int i = 0; i < checkReadSocketsList.Count; i++)
            {
                Socket socket= checkReadSocketsList[i];
                if (socket==listenedSocket)
                {
                    HandleListenedSocket();
                }
                else
                {
                    HandleClientSocket(socket);
                }
            }
            CheckPingTime();
        }        
    }

    /// <summary>
    /// 更新可读Socket检测列表
    /// </summary>
    private void ResetCheckReadList()
    {
        checkReadSocketsList.Clear();
        checkReadSocketsList.Add(listenedSocket);
        foreach (ClientObject co in clientObjectsDict.Values)
        {
            checkReadSocketsList.Add(co.socket);
        }
    }
    /// <summary>
    /// 处理服务器负责监听的套接字
    /// </summary>
    private void HandleListenedSocket()
    {
        try
        {
            Socket clientSocket = listenedSocket.Accept();
            ClientObject co = new ClientObject() { socket = clientSocket,lastPingTime=GetTimeStamp() };
            clientObjectsDict.Add(clientSocket, co);
        }
        catch (SocketException se)
        {
            Console.WriteLine("客户端连接失败：" + se);
        }
    }
    /// <summary>
    /// 处理客户端套接字
    /// </summary>
    private void HandleClientSocket(Socket socket)
    {
        ClientObject co = clientObjectsDict[socket];
        //缓冲区满了，解析消息
        if (co.bo.remainLength<=0)
        {
            HandleReceiveData(co);
        }
        if (co.bo.remainLength <= 0)
        {
            Console.WriteLine("接收消息失败，协议解析不成功或单条协议超过缓冲区长度");
            CloseClientSocket(co);
            return;
        }
        int length = 0;
        try
        {         
            length = co.socket.Receive(co.bo.bytes,co.bo.writeIndex,co.bo.remainLength,SocketFlags.None);
        }
        catch (SocketException se)
        {
            Console.WriteLine("接收信息失败：" + se);
            CloseClientSocket(co);
            return;
        }
        //客户端正常关闭
        if (length<=0)
        {
            Console.WriteLine("客户端断开连接");
            CloseClientSocket(co);
        }
        co.bo.writeIndex += length;
        HandleReceiveData(co);
        co.bo.CheckAndMoveBytes();
    }
    /// <summary>
    /// 关闭客户端
    /// </summary>
    /// <param name="co"></param>
    private void CloseClientSocket(ClientObject co)
    {
        Console.WriteLine("客户端:" + co.socket.RemoteEndPoint + "关闭");
        PlayerManager.Instance.RemovePlayer(co.player.id);
        co.socket.Close();
        clientObjectsDict.Remove(co.socket);
    }

    public void Send(ClientObject co,PTBase pt)
    {
        byte[] ptBytes = PT.EncodeName(pt).Concat(PT.EncodeBody(pt)).ToArray();
        Int16 length = (Int16)ptBytes.Length;
        byte[] lengthBytes = BitConverter.GetBytes(length);
        if (!BitConverter.IsLittleEndian)
        {
            lengthBytes.Reverse();
        }
        byte[] sendBytes = lengthBytes.Concat(ptBytes).ToArray();
        ByteObject bo = new ByteObject(sendBytes);
        co.socket.Send(bo.bytes,0,bo.bytes.Length,SocketFlags.None);
    }

    private void HandleReceiveData(ClientObject co)
    {
        if (co.bo.dataLength <= 2)
        {
            return;
        }
        Int16 bodyLength = (Int16)(co.bo.bytes[co.bo.readIndex] | co.bo.bytes[co.bo.readIndex + 1] << 8);
        if (co.bo.dataLength < bodyLength + 2)
        {
            return;
        }
        co.bo.readIndex += 2;
        //解析协议名
        int nameCount = 0;
        string protoName = PT.DecodeName(co.bo.bytes, co.bo.readIndex, out nameCount);
        if (protoName == "")
        {
            Console.WriteLine("协议解析失败");
            return;
        }
        co.bo.readIndex += nameCount;
        //解析协议体
        int bodyCount = bodyLength - nameCount;
        PTBase ptBase = PT.DecodeBody(protoName, co.bo.bytes, co.bo.readIndex, bodyCount);
        co.bo.readIndex += bodyCount;
        co.bo.CheckAndMoveBytes();
        PTManager.Instance.SendPTEvent(protoName, new MsgPT() { clientObject = co, pt = ptBase }) ;
        HandleReceiveData(co);
    }
    /// <summary>
    /// 获取时间戳
    /// </summary>
    /// <returns></returns>
    public long GetTimeStamp()
    {
        TimeSpan timeSpan= DateTime.UtcNow - new DateTime(1970,1,1,0,0,0,0);
        return Convert.ToInt64(timeSpan.TotalSeconds);
    }
    /// <summary>
    /// 心跳检测
    /// </summary>
    private void CheckPingTime()
    {
        foreach (ClientObject co in clientObjectsDict.Values)
        {
            if (GetTimeStamp()-co.lastPingTime>pingPongInterval*4)
            {
                Console.WriteLine("心跳检测超时处理");
                CloseClientSocket(co);
                return;
            }
        }
    }

    //#region 异步方法
    //public void AcceptCallback(IAsyncResult iar)
    //{
    //    try
    //    {
    //        Socket clientSocket= listenedSocket.EndAccept(iar);
    //        Console.WriteLine("客户端已连接" + clientSocket.RemoteEndPoint);
    //        ClientObject co = new ClientObject() {socket=clientSocket };
    //        clientObjectsDict.Add(clientSocket,co);
    //        //接收数据
    //        clientSocket.BeginReceive(co.co.bo,0,1024,0, ReceiveCallback,co);
    //        //监听其他客户端连接
    //        listenedSocket.BeginAccept(AcceptCallback,listenedSocket);
    //    }
    //    catch (SocketException se)
    //    {
    //        Console.WriteLine("客户端连接失败："+se);
    //    }
    //}

    //public void ReceiveCallback(IAsyncResult iar)
    //{
    //    try
    //    {
    //        ClientObject co = (ClientObject)iar.AsyncState;
    //        int length = co.socket.EndReceive(iar);
    //        if (length==0)
    //        {
    //            co.socket.Close();
    //            clientObjectsDict.Remove(co.socket);
    //            Console.WriteLine("客户端关闭");
    //        }
    //        else
    //        {
    //            string readStr = Encoding.Default.GetString(co.co.bo, 0, length);
    //            Console.WriteLine("服务器接收信息：" + readStr);
    //            foreach (ClientObject clientObject in clientObjectsDict.Values)
    //            {
    //                Send("服务器发送了消息：" + readStr, clientObject.socket);
    //            }
    //        }

    //    }
    //    catch (SocketException se)
    //    {
    //        Console.WriteLine("接收信息失败：" + se);
    //    }
    //}

    //public void Send(string msg,Socket socket)
    //{
    //    byte[] sendMsg = Encoding.Default.GetBytes(msg);
    //    socket.BeginSend(sendMsg, 0, sendMsg.Length, SocketFlags.None, SendCallback, socket);
    //}

    //private void SendCallback(IAsyncResult iar)
    //{
    //    try
    //    {
    //        Socket socket = (Socket)iar.AsyncState;
    //        int length = socket.EndSend(iar);
    //        Console.WriteLine("发送成功，消息长度为：" + length);
    //    }
    //    catch (SocketException se)
    //    {
    //        Console.WriteLine("发送失败：" + se);
    //    }
    //}

    //#endregion
}

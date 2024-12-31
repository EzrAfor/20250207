using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static NetSystem;

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
        socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);//�½��׽���
        socket.NoDelay = true;//�Ż�����
        readBuff = new ByteObject();
        writeQueue = new Queue<ByteObject>();
        isClosing = false;
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
        //socket.Connect(ip, port);//�������� ͬ������
        socket.BeginConnect(ip, port, ConnectCallBack,socket);//�첽

    }

    private void ConnectCallBack(IAsyncResult iar)
    {
        try
        {
            Socket socket = (Socket)iar.AsyncState;
            socket.EndConnect(iar);
            Receive();
            Debug.Log("���ӳɹ�");
            isConnecting = false;
        }
        catch (SocketException se) { 
        Debug.Log("����ʧ��"+ se);
        }
    }


    public void Init()
    {

    }

    public void Send(PTBase msg)
    {
        if(isClosing)return;

        byte[] ptBytes = PT.EncodeName(msg).Concat(PT.EncodeBody(msg)).ToArray();// ��������Ϣ����Ϣ��ϲ�
        Int16 length = (Int16)ptBytes.Length;
        byte[] lengthBytes = BitConverter.GetBytes(length);
        if (!BitConverter.IsLittleEndian) {//��С�˴���
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
            int length = socket.EndSend(iar);// ����첽���Ͳ���������ȡʵ�ʷ��͵��ֽ���
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
            Debug.Log("����ʧ��" + se);
        }
    }




    //public void Send(string msg)
    //{
    //    socket.Send(Encoding.Default.GetBytes(msg));//ת����������Ϊsend����ֻ�ܷ�������
    //}


    //public void Receive() {
    //    byte[] readBuff = new byte[1024];
    //    int count = socket.Receive(readBuff);
    //    string readStr = Encoding.UTF8.GetString(readBuff,0,count);
    //    Debug.Log("�ͻ��˽�����Ϣ"+readStr);
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
            int length = socket.EndReceive(iar);// ����첽���ղ���������ȡʵ�ʽ��յ����ֽ���
            readBuff.writeIndex += length;//����������������ۼ�
            HandleReceiveData();
            if (readBuff.remainLength < 8) {
                readBuff.MoveBytes();
                readBuff.ReSize(readBuff.dataLength*2);
            }
            Receive();

        }
        catch (SocketException se)
        {
            Debug.Log("����ʧ��" + se);
        }
    }


    private void HandleReceiveData() {
        if (readBuff.dataLength <= 2)
        {// ����Ƿ����㹻����������ȡ��Ϣ���ȣ�������Ҫ2���ֽڣ� �����2�������2���ֽ� Ҳ���Ƕ�ȡ��Ϣ����
            return;
        }
        //���д�С�˴��� ���л��������Եõ�������ʮ��λֵ
        Int16 bodyLength = (Int16)(readBuff.bytes[readBuff.readIndex] | readBuff.bytes[readBuff.readIndex+1] << 8);
        //Int16 bodyLength =  BitConverter.ToInt16(readBuff,0);// �ӻ�������ǰ�����ֽ��ж�ȡ��Ϣ����
        if (readBuff.dataLength < bodyLength+2)
        {// ����Ƿ����㹻������������������Ϣ�� ��2����ΪҪ����ǰ�����������ݴ�С���ֽ�
            return;
        }
        readBuff.readIndex += 2;
        int nameCount = 0;
        string protoName = PT.DecodeName(readBuff.bytes, readBuff.readIndex,out nameCount);//����Э��
        if (protoName == "") {
            Debug.Log("Э�����ʧ��");
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
    }

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

}

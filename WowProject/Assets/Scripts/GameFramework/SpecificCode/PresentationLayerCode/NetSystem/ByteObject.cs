using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
//*****************************************
//创建人： Trigger 
//功能说明：消息数据结构对象
//***************************************** 
public class ByteObject
{
    //默认大小
    private const int DEFAULTSIZE = 1024;
    //起始大小
    private int initSize;
    //缓冲区
    public byte[] bytes;
    //缓冲区容量
    private int capacity;
    //当前缓冲区读写位置
    public int readIndex;
    public int writeIndex;
    //剩余空间
    public int remainLength { get { return capacity - writeIndex; } }
    //数据长度
    public int dataLength { get { return writeIndex - readIndex; } }

    public ByteObject(byte[] defaultBytes)
    {
        bytes = defaultBytes;
        capacity=initSize= writeIndex = defaultBytes.Length;
        readIndex = 0;        
    }
    public ByteObject(int size=DEFAULTSIZE)
    {
        bytes = new byte[size];
        capacity=initSize = size;
        readIndex = writeIndex = 0;
    }
    /// <summary>
    /// 扩容缓冲区大小
    /// </summary>
    /// <param name="size">数据长度</param>
    public void ReSize(int size)
    {
        if (size<dataLength)
        {
            return;
        }
        if (size<initSize)
        {
            return;
        }
        int useCapicity = 1;
        while (useCapicity < size)
        {
            useCapicity *= 2;
        }
        capacity = useCapicity;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes,readIndex,newBytes,0,writeIndex-readIndex);
        bytes = newBytes;
        writeIndex = dataLength;
        readIndex = 0;
    }
    /// <summary>
    /// 检查并移动数据
    /// </summary>
    public void CheckAndMoveBytes()
    {
        if (dataLength<8)
        {
            MoveBytes();
        }
    }
    /// <summary>
    /// 具体的移动数据方法
    /// </summary>
    public void MoveBytes()
    {
        Array.Copy(bytes, readIndex, bytes, 0, dataLength);
        writeIndex = dataLength;
        readIndex = 0;
    }
}

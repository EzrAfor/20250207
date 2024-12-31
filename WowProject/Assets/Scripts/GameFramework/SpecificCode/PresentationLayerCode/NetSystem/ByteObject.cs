//消息数据结构对象
using System;

public class ByteObject 
{
    private const int DEFAULTSIZE = 1024;
    private int initSize;
    private int capacity = 0;//缓冲区容量 
    

    public byte[] bytes ;//缓冲区
    public int readIndex;//缓冲区读写位置
    public int writeIndex;//数据长度的指针
    
    public int remainLength { get { return capacity-writeIndex; } }//剩余缓冲区的空间
    
    public int dataLength { get { return writeIndex - readIndex; } }//剩余读取数据的长度

    public ByteObject(byte[] defaultBytes) {
        bytes = defaultBytes;        
        capacity = initSize = writeIndex = defaultBytes.Length;
        readIndex = 0;
    }

    public ByteObject(int size = DEFAULTSIZE) { 
        bytes = new byte[size];
        initSize = capacity = size;
        readIndex = writeIndex = 0;


    }

    public void ReSize(int size) {
        if (size < dataLength)  return;
        if (size < initSize) return;
        int useCapicity = 1;
        while (useCapicity < size) {
            useCapicity *= 2;
        }
        capacity = useCapicity;
        byte[] newBytes = new byte[capacity];
        Array.Copy(bytes,readIndex,newBytes,0,writeIndex-readIndex);
        bytes = newBytes;
        writeIndex = dataLength;
        readIndex=0;
    }

    public void CheckAndMoveBytes() {//检查并移动数据s
        if (dataLength < 8) { 
        MoveBytes();
        }
    }

    public void MoveBytes() {
        Array.Copy(bytes,readIndex,bytes,0,dataLength);
        writeIndex = dataLength;
        readIndex = 0;
    }

}

//��Ϣ���ݽṹ����
using System;

public class ByteObject 
{
    private const int DEFAULTSIZE = 1024;
    private int initSize;
    private int capacity = 0;//���������� 
    

    public byte[] bytes ;//������
    public int readIndex;//��������дλ��
    public int writeIndex;//���ݳ��ȵ�ָ��
    
    public int remainLength { get { return capacity-writeIndex; } }//ʣ�໺�����Ŀռ�
    
    public int dataLength { get { return writeIndex - readIndex; } }//ʣ���ȡ���ݵĳ���

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

    public void CheckAndMoveBytes() {//��鲢�ƶ�����s
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

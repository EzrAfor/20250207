using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTEnterGameScene : PTBase 
{ 
   public PTEnterGameScene()
    {
        protoName = "PTEnterGameScene";
    }
    public string playDatasListJson = "";//�������ظ�������ҵ���Ϣ
    public string enterGamePlayerDataJson = "";//�ͻ��˷����Լ�����Ϣ
}

//��ȡ���������Ϣ�б�
public class PTGetPlayerDatas : PTBase
{
    public PTGetPlayerDatas()
    {
        protoName = "PTGetPlayerDatas";
    }
    //�������ظ�
    public string playerDatasJson = "";

}
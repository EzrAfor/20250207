using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTRegister : PTBase {

    public PTRegister() {//ע��Э��
        protoName = "PTRegister";
    }

    public string id = "";
    public string pw = "";
    public int result = 0;//�������ظ��Ľ�� 0�ɹ� 1ʧ��
}

public class PTLogin : PTBase//��¼Э��
{
    public PTLogin() { protoName = "PTLogin"; }
    public string id = "";
    public string pw = "";
    public int result = 0;//�������ظ��Ľ�� 0�ɹ� 1ʧ��
}

//�����½�ɫ
public class PTCreateNewCharacter : PTBase
{
    public PTCreateNewCharacter() { protoName = "PTCreateNewCharacter"; }
    public string playDataJson = "";//�ͻ��˷�����Ϣ
    public int result = 0;//�������ظ��Ľ�� 0�ɹ� 1ʧ��

}
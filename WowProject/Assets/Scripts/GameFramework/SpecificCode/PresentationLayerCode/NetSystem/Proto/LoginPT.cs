using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PTRegister : PTBase {

    public PTRegister() {//注册协议
        protoName = "PTRegister";
    }

    public string id = "";
    public string pw = "";
    public int result = 0;//服务器回复的结果 0成功 1失败
}

public class PTLogin : PTBase//登录协议
{
    public PTLogin() { protoName = "PTLogin"; }
    public string id = "";
    public string pw = "";
    public int result = 0;//服务器回复的结果 0成功 1失败
}

//创建新角色
public class PTCreateNewCharacter : PTBase
{
    public PTCreateNewCharacter() { protoName = "PTCreateNewCharacter"; }
    public string playDataJson = "";//客户端发的信息
    public int result = 0;//服务器回复的结果 0成功 1失败

}
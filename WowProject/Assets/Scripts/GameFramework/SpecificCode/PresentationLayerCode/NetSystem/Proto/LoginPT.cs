using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：登录部分相关的协议
//***************************************** 
/// <summary>
/// 注册协议
/// </summary>
public class PTRegister : PTBase
{
    public PTRegister() { protoName = "PTRegister"; }
    //客户端发
    public string id = "";
    public string pw = "";
    //服务器回 (0.成功，1.失败)
    public int result = 0;
}
/// <summary>
/// 登录协议
/// </summary>
public class PTLogin : PTBase
{
    public PTLogin() { protoName = "PTLogin"; }
    //客户端发
    public string id = "";
    public string pw = "";
    //服务器回 (0.成功，1.失败)
    public int result = 0;
}
/// <summary>
/// 创建新角色
/// </summary>
public class PTCreateNewCharacter : PTBase
{
    public PTCreateNewCharacter() { protoName = "PTCreateNewCharacter"; }
    //客户端发
    public string playerDataJson = "";
    //服务器回 (0.成功，1.失败)
    public int result = 0;
}


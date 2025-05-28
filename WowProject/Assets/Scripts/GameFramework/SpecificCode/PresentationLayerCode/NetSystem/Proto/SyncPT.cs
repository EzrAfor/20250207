using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：同步其他玩家人物
//***************************************** 
public class PTSyncCharacter : PTBase
{
    public PTSyncCharacter() { protoName = "PTSyncCharacter"; }
    public CharacterSyncData cd;
}

public class PTSyncSetChoiceTarget : PTBase
{
	public PTSyncSetChoiceTarget() { protoName = "PTSyncSetChoiceTarget"; }
	//客户端发
	//选择目标的玩家 设置者
	public string pID;
	//被选中为目标的玩家 被设置者
	public string tID;
}

public class PTSyncAttack : PTBase
{
	public PTSyncAttack() { protoName = "PTSyncAttack"; }
	//客户端发
	//攻击者
	public string pID;
	//服务器回
	public bool canBeBattle;
}

public class PTSyncEnterOrLeaveAOI : PTBase
{
    //服务器发
    public PTSyncEnterOrLeaveAOI() { protoName = "PTSyncEnterOrLeaveAOI"; }
    public PlayerData pd;
    public List<PlayerData> otherPlayerCDList;
    public bool enterAOI;
}
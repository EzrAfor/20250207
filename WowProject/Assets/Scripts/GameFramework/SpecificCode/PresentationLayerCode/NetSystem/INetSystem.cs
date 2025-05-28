using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：网络系统接口
//***************************************** 
public interface INetSystem : ISystem
{
    /// <summary>
    /// 连接
    /// </summary>
    /// <param name="ip"></param>
    /// <param name="port"></param>
    public void Connect(string ip,int port);
    /// <summary>
    /// 发送数据
    /// </summary>
    /// <param name="msg"></param>
    public void Send(PTBase msg);
    /// <summary>
    /// 关闭连接
    /// </summary>
    public void Close();
    ///// <summary>
    ///// 接收数据
    ///// </summary>
    //public void Receive();
    /// <summary>
    /// 更新系统状态和数据
    /// </summary>
    public void Update();
    public void RegistPTListener(string ptName,NetSystem.PTListener listener);
    public void UnregistPTListener(string ptName, NetSystem.PTListener listener);

    /// <summary>
    /// 设置玩家当前信息值
    /// </summary>
    /// <param name="pd"></param>
    public void SetPSDValue(PlayerSaveData psd);
    /// <summary>
    /// 获取玩家当前信息值
    /// </summary>
    /// <returns></returns>
    public PlayerSaveData GetPSDValue();

    /// <summary>
    /// 设置当前玩家选择的人物编号
    /// </summary>
    /// <param name="value"></param>
    public void SetChoiceID(int value);
    /// <summary>
    /// 获取当前玩家选择的人物编号
    /// </summary>
    /// <returns></returns>
    public int GetChoiceID();

    /// <summary>
    /// 设置当前客户端玩家的pmc引用
    /// </summary>
    /// <param name="pd"></param>
    public void SetPlayerPMC(PlayerMovementController pmc);

    /// <summary>
    /// 设置选择目标当前信息值
    /// </summary>
    /// <param name="pd"></param>
    public void SetTargetPDValue(string id);
    /// <summary>
    /// 获取选择目标当前信息值
    /// </summary>
    /// <returns></returns>
    public PlayerData GetTargetPDValue();

    /// <summary>
    /// 设置当前游戏中所有玩家列表信息值
    /// </summary>
    /// <param name="pd"></param>
    public void SetPDListValue(List<PlayerData> list);
    /// <summary>
    /// 获取当前游戏中其他列表当前信息值
    /// </summary>
    /// <returns></returns>
    public List<PlayerData> GetSyncPDListValue();
    /// <summary>
    /// 新进入游戏玩家的角色信息
    /// </summary>
    public void AddNewPlayerData(PlayerData pd,SyncPMCtrl spmc);
    /// <summary>
    /// 退出游戏玩家的角色信息
    /// </summary>
    public void ExitPlayerData(PlayerData pd);
}

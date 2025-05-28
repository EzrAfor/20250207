using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：状态基类
//***************************************** 
public abstract class BaseState:IController
{
    protected CharacterFSM cfsm;
    protected Animator animator;
    public CHARACTERSTATE stateType;

    public BaseState(CharacterFSM cFSM,Animator animator,CHARACTERSTATE cs)
    {
        cfsm = cFSM;
        this.animator = animator;
        stateType = cs;
    }
    /// <summary>
    /// 初始化状态
    /// </summary>
    public abstract void InitState();
    /// <summary>
    /// 进入状态
    /// </summary>
    public abstract void EnterState();
    /// <summary>
    /// 退出状态
    /// </summary>
    public abstract void ExitState();
    /// <summary>
    /// 更新状态
    /// </summary>
    public abstract void UpdateState();
}

public enum CHARACTERSTATE
{ 
    //NONE,
    IDLE,
    MOVE,
    JUMP,
    BATTLE,
    ATTACK,
    HIT,
    DEAD
}

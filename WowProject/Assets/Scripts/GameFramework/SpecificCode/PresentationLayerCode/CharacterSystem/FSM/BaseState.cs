using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseState :IController
{
    protected characterFSM cfsm;
    
    protected Animator animator;
    public CHARACTERSTATE stateType;





    public BaseState(characterFSM characterFSM, Animator animator,CHARACTERSTATE cs) {
        cfsm = characterFSM;
        this.animator = animator;
        stateType = cs;
    }

    public abstract void InitState();

    public abstract void EnterState();
    public abstract void ExitState();
    public abstract void UpdateState();
    
    

}
public enum CHARACTERSTATE
{
    NONE,
    IDLE,
    MOVE,
    JUMP,
    ATTACK,
    HIT,
    DEAD
}

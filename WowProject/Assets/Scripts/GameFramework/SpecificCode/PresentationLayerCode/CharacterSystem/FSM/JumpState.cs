using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：跳跃状态
//***************************************** 
public class JumpState : BaseState
{
    public JumpState(CharacterFSM cFSM, Animator animator, CHARACTERSTATE cs) : base(cFSM, animator, cs)
    {
    }

    public override void EnterState()
    {
        animator.SetBool("IsGround",false);
        animator.CrossFade("Jump",0);
        cfsm.SetUpperBodyLayerWeight();
        //cfsm.StopLookingFunction(true);
    }

    public override void ExitState()
    {
        animator.SetBool("IsGround", true);
    }

    public override void InitState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：移动状态
//***************************************** 
public class MoveState : BaseState
{
    public MoveState(CharacterFSM cFSM, Animator animator, CHARACTERSTATE cs) : base(cFSM, animator, cs)
    {
    }

    public override void EnterState()
    {
        animator.SetBool("MoveState",true);
        cfsm.SetUpperBodyLayerWeight();
        //if (cfsm.GetLastState()==CHARACTERSTATE.IDLE)
        //{
        //    cfsm.StopLookingFunction(false);
        //}
        //else
        //{
        //    cfsm.StopLookingFunction(false,0.3f);
        //}
    }

    public override void ExitState()
    {
        animator.SetBool("MoveState", false);
    }

    public override void InitState()
    {
        
    }

    public override void UpdateState()
    {
        float h = cfsm.GetFloatInputValue(InputCode.HorizontalMoveValue);
        float v = cfsm.GetFloatInputValue(InputCode.VerticalMoveValue);
        //Debug.Log("h:"+h+"v:"+v);
        animator.SetFloat("MoveX",h);
        animator.SetFloat("MoveY",v);
        //if (v!=0)
        //{
        //    animator.SetFloat("MoveValue",v);
        //}
        //else
        //{
        //    if (h!=0)
        //    {
        //        animator.SetFloat("MoveValue", 1);
        //    }
        //}
    }
}

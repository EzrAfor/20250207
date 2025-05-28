using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：闲置状态
//***************************************** 
public class IdleState : BaseState
{
    public IdleState(CharacterFSM cFSM, Animator animator, CHARACTERSTATE cs) : base(cFSM, animator, cs)
    {
        
    }

    public override void EnterState()
    {
        //animator.SetFloat("MoveValue",0);
        //cfsm.StopLookingFunction(true);
        animator.SetFloat("MotionRandomNum",0);
        animator.SetFloat("MoveX",0);
        animator.SetFloat("MoveY",0);
    }

    public override void ExitState()
    {
        animator.SetFloat("Yaw",0);
    }

    public override void InitState()
    {
        
    }

    public override void UpdateState()
    {       
        float x = cfsm.GetFloatInputValue(InputCode.HorizontalRotateValue);
        if (x>0)
        {
            animator.SetBool("RotateState",true);
            animator.SetFloat("Yaw",1);
        }
        else if (x<0)
        {
            animator.SetBool("RotateState", true);
            animator.SetFloat("Yaw", -1);
        }
        else
        {
            animator.SetBool("RotateState", false);
            animator.SetFloat("Yaw", 0);
        }
    }
}

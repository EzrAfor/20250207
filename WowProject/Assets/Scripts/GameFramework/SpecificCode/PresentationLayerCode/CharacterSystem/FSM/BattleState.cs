using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleState : BaseState
{
    public BattleState(CharacterFSM cFSM, Animator animator, CHARACTERSTATE cs) : base(cFSM, animator, cs)
    {
    }

    public override void EnterState()
    {
        animator.SetFloat("MotionRandomNum",1);
        cfsm.SetBodyLayerWeight();
    }

    public override void ExitState()
    {

    }

    public override void InitState()
    {
        
    }

    public override void UpdateState()
    {
        float x = cfsm.GetFloatInputValue(InputCode.HorizontalRotateValue);
        if (x > 0)
        {
            animator.SetBool("RotateState", true);
            animator.SetFloat("Yaw", 1);
        }
        else if (x < 0)
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

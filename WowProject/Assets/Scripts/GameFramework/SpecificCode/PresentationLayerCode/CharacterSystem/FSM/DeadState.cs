using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadState : BaseState
{
    public DeadState(CharacterFSM cFSM, Animator animator, CHARACTERSTATE cs) : base(cFSM, animator, cs)
    {
    }

    public override void EnterState()
    {
        animator.SetBool("Dead",true);
        cfsm.SetAllLayerWeight();
    }

    public override void ExitState()
    {
        animator.SetBool("Dead", false);
        cfsm.SetBodyLayerWeight();
    }

    public override void InitState()
    {
        
    }

    public override void UpdateState()
    {
        
    }
}

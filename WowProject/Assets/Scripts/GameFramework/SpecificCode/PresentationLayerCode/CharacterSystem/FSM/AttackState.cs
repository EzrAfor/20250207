using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : BaseState
{
    public AttackState(CharacterFSM cFSM, Animator animator, CHARACTERSTATE cs) : base(cFSM, animator, cs)
    {
    }

    public override void EnterState()
    {
        animator.SetTrigger("Attack");
        cfsm.isAttacking = true;
        cfsm.CreateProjectile();
    }

    public override void ExitState()
    {
        
    }

    public override void InitState()
    {

    }

    public override void UpdateState()
    {
        
    }
}

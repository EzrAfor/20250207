using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : BaseState
{
    public HitState(CharacterFSM cFSM, Animator animator, CHARACTERSTATE cs) : base(cFSM, animator, cs)
    {
    }

    public override void EnterState()
    {
        animator.SetTrigger("Hit");
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

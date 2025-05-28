using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：人物有限状态机
//***************************************** 
public class CharacterFSM : MonoBehaviour,IController
{
    private Dictionary<CHARACTERSTATE, BaseState> statesDict;
    private BaseState currentState;
    private BaseState lastState;
    private InputController ic;
    public CHARACTERSTATE c;
    //private PlayerHeadController phc;
    private PlayerMovementController pmc;
    private Animator currentAnimator;
    public bool beBattle;
    public bool isAttacking;
    public bool upperBodyLayerIsSetting;

    private void OnEnable()
    {
        if (currentAnimator!=null)
        {
            SetMoveState();
        }
    }

    /// <summary>
    /// 初始化状态机
    /// </summary>
    public void InitFSM(Animator currentAnimator,InputController inputController, /*PlayerHeadController playerHeadController,*/PlayerMovementController playerMovement)
    {
        statesDict = new Dictionary<CHARACTERSTATE, BaseState>()
        {
            { CHARACTERSTATE.IDLE,new IdleState(this,currentAnimator,CHARACTERSTATE.IDLE)},
            { CHARACTERSTATE.MOVE,new MoveState(this,currentAnimator,CHARACTERSTATE.MOVE)},
            { CHARACTERSTATE.JUMP,new JumpState(this,currentAnimator,CHARACTERSTATE.JUMP)},
            { CHARACTERSTATE.BATTLE,new BattleState(this,currentAnimator,CHARACTERSTATE.BATTLE)},
            { CHARACTERSTATE.ATTACK,new AttackState(this,currentAnimator,CHARACTERSTATE.ATTACK)},
            { CHARACTERSTATE.HIT,new HitState(this,currentAnimator,CHARACTERSTATE.HIT)},
            { CHARACTERSTATE.DEAD,new DeadState(this,currentAnimator,CHARACTERSTATE.DEAD)}

        };
        ic = inputController;
        this.currentAnimator = currentAnimator;
        //phc = playerHeadController;
        pmc = playerMovement;
        SetDefaultState();
    }
    /// <summary>
    /// 设置默认状态并初始化其他状态
    /// </summary>
    private void SetDefaultState()
    {
        foreach (var item in statesDict)
        {
            item.Value.InitState();
        }
        currentState = statesDict[CHARACTERSTATE.IDLE];
        currentState.EnterState();
    }
    /// <summary>
    /// 改变状态
    /// </summary>
    public void ChangeState(CHARACTERSTATE newStateType)
    {
        if (statesDict.ContainsKey(newStateType))
        {
            BaseState changeState = statesDict[newStateType];
            if (changeState!=currentState)
            {
                currentState.ExitState();
                lastState = currentState;
                currentState = changeState;
                currentState.EnterState();
                if (!pmc.isSyncCharacer)
                {
                    pmc.SyncUpdate();
                }
            }
        }
    }
    /// <summary>
    /// 获取当前状态
    /// </summary>
    /// <returns></returns>
    public CHARACTERSTATE GetCurrentState()
    {
        return currentState.stateType;
    }
    /// <summary>
    /// 获取上一个状态
    /// </summary>
    /// <returns></returns>
    public CHARACTERSTATE GetLastState()
    {
        return lastState.stateType;
    }

    void Update()
    {
        if (GetCurrentState() == CHARACTERSTATE.DEAD)
        {
            return;
        }
        if (currentState!=null)
        {
            c = currentState.stateType;
            currentState.UpdateState();
        }
    }

    #region 输入值的设置和获取
    /// <summary>
    /// 设置当前某个输入状态的值(bool)
    /// </summary>
    /// <param name="inputCode"></param>
    /// <param name="inputValue"></param>
    public void SetInputValue(string inputCode, bool inputValue)
    {
        ic.SetInputValue(inputCode,inputValue);
    }
    /// <summary>
    /// 获取当前某个输入的状态(bool)
    /// </summary>
    /// <param name="inputCode"></param>
    /// <returns></returns>
    public bool GetBoolInputValue(string inputCode)
    {
        return ic.GetBoolInputValue(inputCode);
    }
    /// <summary>
    /// 设置当前某个输入状态的值(float)
    /// </summary>
    /// <param name="inputCode"></param>
    /// <param name="inputValue"></param>
    public void SetInputValue(string inputCode, float inputValue)
    {
        ic.SetInputValue(inputCode, inputValue);
    }
    /// <summary>
    /// 获取当前某个输入的状态(float)
    /// </summary>
    /// <param name="inputCode"></param>
    /// <returns></returns>
    public float GetFloatInputValue(string inputCode)
    {
        return ic.GetFloatInputValue(inputCode);
    }
    #endregion

    public void AnimatorSetDefaultValues()
    {
        currentAnimator.SetFloat("MoveX", 0);
        currentAnimator.SetFloat("MoveY", 0);
        currentAnimator.SetBool("IsGround", false);
        currentAnimator.SetBool("MoveState", false);
    }

    public void SetMoveState()
    {
        currentAnimator.SetBool("MoveState", currentState.stateType == CHARACTERSTATE.MOVE);
    }

    private float setValue;
    private Tween setWeightTween;

    /// <summary>
    /// 设置上半身动画层级的权重
    /// </summary>
    public void SetUpperBodyLayerWeight()
    {
        if (isAttacking)
        {
            isAttacking = false;
            upperBodyLayerIsSetting = true;
            currentAnimator.SetLayerWeight(1, 0);
            currentAnimator.SetLayerWeight(2, 1);
            setValue = 1;
            setWeightTween.Kill();
            setWeightTween = DOTween.To(() => setValue, x => setValue = x, 0, 1.5f)
                .OnUpdate(() => { currentAnimator.SetLayerWeight(2, setValue);
                    upperBodyLayerIsSetting = false; });
        }
        else
        {
            if (!upperBodyLayerIsSetting)
            {
                SetAllLayerWeight();
            }
        }
    }
    /// <summary>
    /// 设置全身动画层级的权重
    /// </summary>
    public void SetBodyLayerWeight()
    {
        currentAnimator.SetLayerWeight(1,1);
        //currentAnimator.SetLayerWeight(2,0);
    }

    public void SetAllLayerWeight()
    {
        currentAnimator.SetLayerWeight(1, 0);
        currentAnimator.SetLayerWeight(2, 0);
    }

    public void CreateProjectile()
    {
        if (pmc.targetTrans)
        {
            this.SendCommand<CreateProjectleCommand>(new CreateProjectleCommandParam()
            {
                t = pmc.targetTrans,
                pos=transform.position
            }) ;
        }
    }

    //public void StopLookingFunction(bool stop,float delayTime=0)
    //{
    //    phc.StopLookingFunction(stop,delayTime);
    //}
}

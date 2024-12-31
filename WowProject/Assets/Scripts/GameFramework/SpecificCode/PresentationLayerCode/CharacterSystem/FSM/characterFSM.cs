using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class characterFSM : MonoBehaviour
{

    private Dictionary<CHARACTERSTATE, BaseState> statesDict;

    private BaseState currentState;
    private BaseState lastState;
    private InputController ic;

    public void InitFSM(Animator currentAnimator,InputController inputController) {
        statesDict = new Dictionary<CHARACTERSTATE, BaseState>() {
            {CHARACTERSTATE.IDLE,new IdleState(this,currentAnimator,CHARACTERSTATE.IDLE) },
            {CHARACTERSTATE.MOVE,new MoveState(this,currentAnimator,CHARACTERSTATE.MOVE) },
            {CHARACTERSTATE.JUMP,new JumpState(this,currentAnimator,CHARACTERSTATE.JUMP) },
        };
        ic = inputController;
        SetDefaultState();
    }

    private void SetDefaultState() {
        foreach (var item in statesDict) {
            item.Value.InitState();
        }
        currentState = statesDict[CHARACTERSTATE.IDLE];
        currentState.EnterState();
        
    }


    public void ChangeState(CHARACTERSTATE newStateType) {
        if (statesDict.ContainsKey(newStateType)) {
            BaseState changeState = statesDict[newStateType];
            if (changeState != currentState)
            {
                currentState.ExitState();
                lastState = currentState;
                currentState = changeState;
                currentState.EnterState();

            }
        }
        
    }

    public CHARACTERSTATE GetCurrentState() {
        return currentState.stateType;
    }
    public CHARACTERSTATE GetLastState()
    {
        return lastState.stateType;
    }




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (currentState != null) {
            currentState.UpdateState();
        }
        
    }


    public void SetInputValue(string inputCode, bool inputValue)
    {
        ic.SetInputValue(inputCode, inputValue);
    }

    public bool GetBoolInputValue(string inputCode)
    {

        return ic.GetBoolInputValue(inputCode);
    }


    public void SetInputValue(string inputCode, float inputValue)
    {
        ic.SetInputValue(inputCode, inputValue);
    }

    public float GetFloatInputValue(string inputCode)
    {
        return ic.GetFloatInputValue(inputCode);

    }


}

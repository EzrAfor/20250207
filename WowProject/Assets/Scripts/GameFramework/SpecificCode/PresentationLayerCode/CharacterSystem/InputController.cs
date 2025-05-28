using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：输入控制
//***************************************** 
public class InputController : MonoBehaviour
{
    public Dictionary<string, bool> inputBoolValueDict;
    public Dictionary<string, float> inputFloatValueDict;
    public bool isSyncCharacter;
    void Awake()
    {
        inputBoolValueDict = new Dictionary<string, bool>()
        {
            { InputCode.JumpState,false},
            { InputCode.BattleState,false},
            { InputCode.ChoiceTarget,false},
        };
        inputFloatValueDict = new Dictionary<string, float>()
        {
            { InputCode.HorizontalMoveValue,0},
            { InputCode.VerticalMoveValue,0},
            { InputCode.HorizontalRotateValue,0}
        };
    }

    void Update()
    {
        if (isSyncCharacter)
        {
            return;
        }
        Vector3 mousePosition = Input.mousePosition;
        if (mousePosition.x < 0 || mousePosition.x > Screen.width ||
            mousePosition.y < 0 || mousePosition.y > Screen.height)
        {
            SetDefaultValues();
            //cFSM.AnimatorSetDefaultValues();
            return;
        }
        SetInputValue(InputCode.HorizontalMoveValue, Input.GetAxisRaw("Horizontal"));
        SetInputValue(InputCode.VerticalMoveValue, Input.GetAxisRaw("Vertical"));
        SetInputValue(InputCode.HorizontalRotateValue, Input.GetAxisRaw("Mouse X"));
        SetInputValue(InputCode.JumpState, Input.GetButtonDown("Jump"));
        SetInputValue(InputCode.BattleState, Input.GetMouseButtonDown(1));
        SetInputValue(InputCode.ChoiceTarget, Input.GetMouseButtonDown(0));
    }
    /// <summary>
    /// 设置当前某个输入状态的值(bool)
    /// </summary>
    /// <param name="inputCode"></param>
    /// <param name="inputValue"></param>
    public void SetInputValue(string inputCode,bool inputValue)
    {
        if (inputBoolValueDict.ContainsKey(inputCode))
        {
            inputBoolValueDict[inputCode] = inputValue;
        }
        else
        {
            Debug.Log("设置输入码错误，错误码为："+inputCode);
        }
    }
    /// <summary>
    /// 获取当前某个输入的状态(bool)
    /// </summary>
    /// <param name="inputCode"></param>
    /// <returns></returns>
    public bool GetBoolInputValue(string inputCode)
    {
        if (inputBoolValueDict.ContainsKey(inputCode))
        {
            return inputBoolValueDict[inputCode];
        }
        else
        {
            Debug.Log("设置输入码错误，错误码为：" + inputCode);
            return false;
        }
    }
    /// <summary>
    /// 设置当前某个输入状态的值(float)
    /// </summary>
    /// <param name="inputCode"></param>
    /// <param name="inputValue"></param>
    public void SetInputValue(string inputCode, float inputValue)
    {
        if (inputFloatValueDict.ContainsKey(inputCode))
        {
            inputFloatValueDict[inputCode] = inputValue;
        }
        else
        {
            Debug.Log("设置输入码错误，错误码为：" + inputCode);
        }
    }
    /// <summary>
    /// 获取当前某个输入的状态(float)
    /// </summary>
    /// <param name="inputCode"></param>
    /// <returns></returns>
    public float GetFloatInputValue(string inputCode)
    {
        if (inputFloatValueDict.ContainsKey(inputCode))
        {
            return inputFloatValueDict[inputCode];
        }
        else
        {
            Debug.Log("设置输入码错误，错误码为：" + inputCode);
            return 0;
        }
    }
    /// <summary>
    /// 玩家的所有输入值设置为默认值
    /// </summary>
    public void SetDefaultValues()
    {
        SetInputValue(InputCode.HorizontalMoveValue, 0);
        SetInputValue(InputCode.VerticalMoveValue, 0);
        SetInputValue(InputCode.HorizontalRotateValue, 0);
        SetInputValue(InputCode.JumpState, false);
        SetInputValue(InputCode.BattleState, false);
        SetInputValue(InputCode.ChoiceTarget, false);
    }
}

public static class InputCode
{
    //移动
    public const string HorizontalMoveValue = "HorizontalMoveValue";
    public const string VerticalMoveValue = "VerticalMoveValue";
    public const string HorizontalRotateValue = "HorizontalRotateValue";//左右旋转
    //跳跃
    public const string JumpState = "JumpState";
    //装备
    public const string EquipState = "EquipState";
    //战斗状态
    public const string BattleState = "BattleState";
    //选择目标
    public const string ChoiceTarget = "ChoiceTarget";
    //放技能
    public static string[] skillsState = new string[] {"SkillState0", "SkillState1", "SkillState2", "SkillState3"
    , "SkillState4", "SkillState5", "SkillState6"};
}

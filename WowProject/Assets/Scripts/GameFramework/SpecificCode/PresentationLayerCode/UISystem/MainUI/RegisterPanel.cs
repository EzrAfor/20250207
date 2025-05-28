using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：注册面板
//***************************************** 
public class RegisterPanel : BasePanel
{
	//账号输入框
	private InputField idInput;
	//密码输入框
	private InputField pwInput;
	//重复输入框
	private InputField reInput;
	//注册按钮
	private Button registBtn;
	//关闭按钮
	private Button cancelBtn;

	public override void OnInit()
	{
		idInput = DeepFindTransform.DeepFindChild(transform, "IF_UserID").GetComponent<InputField>();
		pwInput = DeepFindTransform.DeepFindChild(transform, "IF_Password").GetComponent<InputField>();
		reInput = DeepFindTransform.DeepFindChild(transform, "IF_ReInputPassword").GetComponent<InputField>();
		registBtn = DeepFindTransform.DeepFindChild(transform, "Btn_Regist").GetComponent<Button>();
		cancelBtn = DeepFindTransform.DeepFindChild(transform, "Btn_Cancel").GetComponent<Button>();
		registBtn.onClick.AddListener(OnRegistClick);
        cancelBtn.onClick.AddListener(() => { base.OnClose(); });
		this.SendCommand<RegistPTListenerCommand>(new PTSrc() {ptName= "PTRegister", listener= OnPTRegister });
		base.OnInit();
	}
	/// <summary>
	/// 当按下注册按钮
	/// </summary>
	private void OnRegistClick()
	{
		//用户名或密码为空
        if (idInput.text==""||pwInput.text=="")
        {
			uiSystem.OpenPanel<TipPanel>("用户名和密码不能为空");
			return;
        }
        //两次密码输入不同
        if (reInput.text!=pwInput.text)
        {
			uiSystem.OpenPanel<TipPanel>("两次输入密码不同");
			return;
        }
		PTRegister pt = new PTRegister();
		pt.id = idInput.text;
		pt.pw = pwInput.text;
		this.SendCommand<SendPTCommand>(pt);
	}
	/// <summary>
	/// 收到返回注册协议
	/// </summary>
	/// <param name="pt"></param>
	private void OnPTRegister(PTBase pt)
	{
		PTRegister ptr = (PTRegister)pt;
        if (ptr.result==0)
        {
			uiSystem.OpenPanel<TipPanel>("注册成功");
			OnClose();
        }
        else
        {
			uiSystem.OpenPanel<TipPanel>("注册失败");
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：登录面板
//***************************************** 
public class LoginPanel : BasePanel
{
	//账号输入框
	private InputField idInput;
	//密码输入框
	private InputField pwInput;
	//登陆按钮
	private Button loginBtn;
	//注册按钮
	private Button regBtn;
	//退出按钮
	private Button exitBtn;
	//下雪背景
	private GameObject snowBGGO;
	//冰霜巨龙
	//private GameObject frostwurmnorthrendGo;

	public override void OnInit()
	{
		base.OnInit();
		//寻找组件
		idInput = transform.Find("IF_UserID").GetComponent<InputField>();
		pwInput = transform.Find("IF_PassWord").GetComponent<InputField>();
		loginBtn = transform.Find("Btn_Login").GetComponent<Button>();
		regBtn = transform.Find("Btn_Regist").GetComponent<Button>();
		exitBtn= transform.Find("Btn_Exit").GetComponent<Button>();
		//监听
		loginBtn.onClick.AddListener(OnLoginClick);
		regBtn.onClick.AddListener(OnRegistClick);
		exitBtn.onClick.AddListener(OnExitGame);
		//连接服务器
		this.SendCommand<ConnectCommand>(new ConnectCommandSrc() { ipAddress="127.0.0.1",port=8888});
		this.SendCommand<RegistPTListenerCommand>(new PTSrc() { ptName = "PTLogin", listener = OnPTLogin });
	}

    public override void OnShow(params object[] objs)
    {
        base.OnShow(objs);
		LoadOrDestoryLoginPanelGameObjects(true);
		idInput.text = "";
		pwInput.text = "";
	}

    public override void OnClose()
    {
		base.OnClose();
		LoadOrDestoryLoginPanelGameObjects();
	}

    /// <summary>
    /// 当按下登陆按钮
    /// </summary>
    private void OnLoginClick()
	{
        if (idInput.text==""||pwInput.text=="")
        {
			uiSystem.OpenPanel<TipPanel>("用户名和密码不能为空");
			return;
        }
		PTLogin pt = new PTLogin();
		pt.id = idInput.text;
		pt.pw = pwInput.text;
		this.SendCommand<SendPTCommand>(pt);
	}
	/// <summary>
	/// 收到登录协议
	/// </summary>
	/// <param name="pt"></param>
	private void OnPTLogin(PTBase pt)
	{
		PTLogin ptl = (PTLogin)pt;
        if (ptl.result==0)
        {
			uiSystem.OpenPanel<TipPanel>("登录成功",false);
			uiSystem.OpenPanel<MaskPanel>();
        }
        else
        {
			uiSystem.OpenPanel<TipPanel>("登录失败", false);
		}
	}

	/// <summary>
	/// 当按下注册按钮
	/// </summary>
	private void OnRegistClick()
	{
		uiSystem.OpenPanel<RegisterPanel>();
	}
	/// <summary>
	/// 退出游戏
	/// </summary>
	private void OnExitGame()
	{
		Application.Quit();
	}
	/// <summary>
	/// 加载或销毁下雪特效与冰霜巨龙
	/// </summary>
	/// <param name="ifLoad"></param>
	public void LoadOrDestoryLoginPanelGameObjects(bool ifLoad = false)
	{
		if (ifLoad)
		{
			snowBGGO = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Scene/SnowBG"));
			//frostwurmnorthrendGo = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Scene/Frostwurmnorthrend"));
		}
		else
		{
			GameObject.Destroy(snowBGGO);
			//GameObject.Destroy(frostwurmnorthrendGo);
		}
	}

}

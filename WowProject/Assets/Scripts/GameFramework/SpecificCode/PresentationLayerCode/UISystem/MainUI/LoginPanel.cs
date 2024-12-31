using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    private InputField idInput;
    private InputField pwInput;
    private Button loginBtn;
    private Button regBtn;
    private Button exitBtn;
    private GameObject snowBGGO;
    private GameObject frostwurmnorthrendGO;
    public override void OnInit()
    {
        base.OnInit();
        idInput = transform.Find("IF_UserID").GetComponent<InputField>();
        pwInput = transform.Find("IF_PassWord").GetComponent<InputField>();
        loginBtn = transform.Find("Btn_Login").GetComponent<Button>();
        regBtn = transform.Find("Btn_Regist").GetComponent<Button>();
        exitBtn = transform.Find("Btn_Exit").GetComponent<Button>();


        loginBtn.onClick.AddListener(OnLoginClick);
        regBtn.onClick.AddListener(OnRegisterClick);
        exitBtn.onClick.AddListener(OnExitGame);
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


    private void OnLoginClick()
    {
        if (idInput.text == "" || pwInput.text == "")
        {
            uiSystem.OpenPanel<TipPanel>("用户名和密码不能为空");
            return;
        }
        uiSystem.OpenPanel<MaskPanel>();
       
    }

    private void OnRegisterClick()
    {
        uiSystem.OpenPanel<RegisterPanel>();
    }

    private void OnExitGame() {
        Application.Quit();
    }
    public void LoadOrDestoryLoginPanelGameObjects(bool ifLoad = false)
    {
        if (ifLoad == true)
        {
            snowBGGO = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Scene/SnowBG"));
            frostwurmnorthrendGO = GameObject.Instantiate(GameResSystem.GetRes<GameObject>("Prefabs/Scene/Frostwurmnorthrend"));
        }
        else
        {
            GameObject.Destroy(snowBGGO);
            GameObject.Destroy(frostwurmnorthrendGO);
        }
    }

}

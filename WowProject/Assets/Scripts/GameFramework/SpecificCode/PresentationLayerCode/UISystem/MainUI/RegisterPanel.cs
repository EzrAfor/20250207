using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RegisterPanel : BasePanel
{
    private InputField idInput;
    private InputField pwInput;
    private InputField reInput;
    private Button registerBtn;
    private Button cancelBtn;

    public override void OnInit()
    {
        base.OnInit();
        idInput = DeepFindTransform.DeepFindChild(transform, "IF_UserID").GetComponent<InputField>();
        pwInput = DeepFindTransform.DeepFindChild(transform, "IF_Password").GetComponent<InputField>();
        reInput = DeepFindTransform.DeepFindChild(transform, "IF_ReInputPassword").GetComponent<InputField>();
        registerBtn = DeepFindTransform.DeepFindChild(transform, "Btn_Regist").GetComponent<Button>();
        cancelBtn = DeepFindTransform.DeepFindChild(transform, "Btn_Cancel").GetComponent<Button>();
        registerBtn.onClick.AddListener(OnRegisterClick);
        cancelBtn.onClick.AddListener(() => { base.OnClose(); });
        this.SendCommand<RegistPTListenerCommand>(new PTSrc { ptName = "PTRegister",listener=OnPTRegister });

    }

    private void OnRegisterClick() {
        if (idInput.text == "" || pwInput.text == "") {
            uiSystem.OpenPanel<TipPanel>("用户名和密码不能为空");
            return;
        }
        if (reInput.text != pwInput.text)
        {
            uiSystem.OpenPanel<TipPanel>("两次输入密码不同");
            return;
        }
        PTRegister pt = new PTRegister();
        pt.id = idInput.text;
        pt.pw = pwInput.text;
        this.SendCommand<SendPTCommand>(pt);
    }

    private void OnPTRegister(PTBase pt)
    {
        PTRegister ptr = (PTRegister)pt;
        if (ptr.result == 0)
        {
            uiSystem.OpenPanel<TipPanel>("注册成功");
            OnClose();
        }else
        {
            uiSystem.OpenPanel<TipPanel>("注册失败");
        }
    }
}

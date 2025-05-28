using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
//*****************************************
//创建人： Trigger 
//功能说明：遮罩面板
//***************************************** 
public class MaskPanel : BasePanel
{
    private Image maskImg;

    public override void OnInit()
    {
        maskImg = GetComponent<Image>();
        base.OnInit();
    }

    public override void OnShow(params object[] objs)
    {
        base.OnShow(objs);
        uiSystem.OpenPanel<TipPanel>("登录成功",false);
        DOTween.To(()=>maskImg.color,toColor=>maskImg.color=toColor,new Color(0,0,0,1),1)
            .OnComplete(
            () => 
            {   uiSystem.OpenPanel<ChoicePanel>();
                uiSystem.ClosePanel("TipPanel");
                uiSystem.ClosePanel("LoginPanel");
                base.OnClose();
            });
    }
}

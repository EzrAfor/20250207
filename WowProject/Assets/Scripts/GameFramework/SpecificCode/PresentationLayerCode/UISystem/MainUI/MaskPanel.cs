using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;


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
        uiSystem.OpenPanel<TipPanel>("µÇÂ½³É¹¦",false);
        DOTween.To(() => maskImg.color, toColor => maskImg.color = toColor, new Color(0, 0, 0, 1), 1)
            .OnComplete(() => { 
                uiSystem.OpenPanel<ChoicePanel>();
                uiSystem.ClosePanel("TipPanel");
                uiSystem.ClosePanel("LoginPanel");
                base.OnClose();
            });

    }







}


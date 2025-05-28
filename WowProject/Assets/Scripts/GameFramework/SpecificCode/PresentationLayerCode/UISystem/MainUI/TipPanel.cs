using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//*****************************************
//创建人： Trigger 
//功能说明：提示面板
//***************************************** 
public class TipPanel : BasePanel
{   
    //提示文本
    private Text tipText;
    private Text tipText2;
    //确定按钮
    private Button OKBtn;
    public override void OnInit()
    {
        //寻找组件
        tipText = DeepFindTransform.DeepFindChild(transform, "Text_Tip").GetComponent<Text>();
        tipText2 = DeepFindTransform.DeepFindChild(transform, "Text_Tip2").GetComponent<Text>();
        OKBtn = DeepFindTransform.DeepFindChild(transform, "Btn_OK").GetComponent<Button>();
        OKBtn.onClick.AddListener(OnOkBtnClick);
        base.OnInit();
    }
    /// <summary>
    /// 打开提示面板
    /// </summary>
    /// <param name="objs">1.提示内容(string)，2.是否显示btn，默认为true</param>
    public override void OnShow(params object[] objs)
    {
        base.OnShow(objs);
        if (objs.Length==1)
        {
            tipText.gameObject.SetActive(true);
            tipText2.gameObject.SetActive(false);
            tipText.text = (string)objs[0];
        }
        else if (objs.Length>1)
        {
            tipText.gameObject.SetActive(false);
            tipText2.gameObject.SetActive(true);
            tipText2.text = (string)objs[0];
            OKBtn.gameObject.SetActive((bool)objs[1]);
        }
    }

    public override void OnClose()
    {
        base.OnClose();
        OKBtn.gameObject.SetActive(true);
    }

    private void OnOkBtnClick()
    {
        OnClose();
    }
}

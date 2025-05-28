using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：字符串管理
//***************************************** 
public static class StrMgr
{
    public static string staminaStrColor = "<color=#FFD700>";
    public static string strengthStrColor = "<color=#FFD700>";
    public static string intellectStrColor = "<color=#63B8FF>";
    public static string agilityStrColor = "<color=#888800>";
    public static string spiritStrColor = "<color=#EEAEEE>";
    public static string armorStrColor = "<color=#D2B48C>";
    public static string resistanceStrColor = "<color=#B8860B>";
    public static string moveSpeedStrColor = "<color=#9F79EE>";
    public static string attackRangeStrColor = "<color=#66CD00>";
    public static string[] qualityStrColor = new string[] { "<color=#480082>", "<color=#0000AA>",
    "<color=#00CD00>","<color=#F5F5DC>"};
    public static string priceStrColor = "<color=#7FFF00>";
    public static string equipStrColor = "<color=#CDCD00>";
    public static string amorStrColor = "<color=#880000>";


    /// <summary>
    /// 设置当前物品属性值的文本颜色
    /// </summary>
    /// <param name="strColor">设置的属性颜色</param>
    /// <param name="value">属性值</param>
    /// <returns></returns>
    public static string SetStrColor(string strColor,int value)
    {
        return strColor + value + " </color>";
    }
    public static string SetStrColor(string strColor, float value)
    {
        return strColor + value + " </color>";
    }
    public static string SetStrColor(string strColor, string value)
    {
        return strColor + value + " </color>";
    }
}

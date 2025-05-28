using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：选择人物界面的角色展示
//***************************************** 
public class ChoiceCharacterDress : MonoBehaviour,IController
{
    //private SkinnedMeshRenderer[] smrs;
    private GameObject[] gos;
    public GameObject currentCharacterGo;//当前人物控制的游戏物体
    public float initAngle=90;
    public float offsetAngle;
    //private IUISystem uiSystem;

    private void Awake()
    {
        //smrs = new SkinnedMeshRenderer[2];
        gos = new GameObject[2];
        for (int i = 0; i < gos.Length; i++)
        {
            gos[i] = transform.GetChild(i).gameObject;
            //smrs[i] = gos[i].GetComponentInChildren<SkinnedMeshRenderer>();
        }
        this.RegistEvent<SetMaterialEvent>(SetMaterial);
        this.RegistEvent<RotateModelEvent>(RotateModel);
        this.RegistEvent<ResetModelAngleEvent>(ResetModelAngle);
    }

    void Update()
    {

    }

    public void SetMaterial(object obj)
    {
        PlayerData pd = (PlayerData)obj;
        for (int i = 0; i < gos.Length; i++)
        {
            gos[i].SetActive(false);
        }
        currentCharacterGo=gos[(int)pd.gender];
        currentCharacterGo.SetActive(true);
        //Material m = GameResSystem.GetRes<Material>("Materials/"+pd.gender.ToString()+"/"+(int)pd.role);
        //for (int i = 0; i < smrs[(int)pd.gender].sharedMaterials.Length; i++)
        //{
        //    if (pd.gender==GENDER.MALE)
        //    {
        //        if (i!=1&&i!=2&&i!=10)
        //        {
        //            smrs[(int)pd.gender].materials[i].CopyPropertiesFromMaterial(m);
        //        }
        //    }
        //    else
        //    {
        //        if (i != 0 && i != 2 && i != 10)
        //        {
        //            smrs[(int)pd.gender].materials[i].CopyPropertiesFromMaterial(m);
        //        }
        //    }
        //}
    }
    /// <summary>
    /// 旋转人物模型(游戏中)
    /// </summary>
    /// <param name="roateSrc"></param>
    public void RotateCharacterModel(float h,float v)
    {
        if (!currentCharacterGo)
        {
            return;
        }
        if (v != 0)//前后
        {
            if (v > 0)//左上右上
            {
                offsetAngle = 45 * h;
            }
            else//左下右下
            {
                offsetAngle = -45 * h;
            }
        }
        else//左右
        {
            if (h != 0)//正左正右
            {
                offsetAngle = 90 * h;
            }
            else//正前方
            {
                offsetAngle = 0;
            }
        }
        currentCharacterGo.transform.localEulerAngles = new Vector3(0, initAngle + offsetAngle, 0);
    }
    /// <summary>
    /// 旋转选择界面人物的模型
    /// </summary>
    /// <param name="angle"></param>
    private void RotateModel(object angle)
    {
        currentCharacterGo.transform.localEulerAngles += new Vector3(0, (float)angle, 0);
    }
    private void ResetModelAngle(object obj)
    {
        currentCharacterGo.transform.localEulerAngles = new Vector3(0, -90, 0);
    }
}

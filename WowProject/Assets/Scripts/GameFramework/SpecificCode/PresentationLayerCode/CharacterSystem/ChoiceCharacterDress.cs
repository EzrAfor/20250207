using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UIElements;

public class ChoiceCharacterDress : MonoBehaviour,IController
{
    private SkinnedMeshRenderer[] smrs;
    private GameObject[] gos;
    //private IUISystem uiSystem;
    public GameObject currentCharacterGo;
    private float initAngle=90;
    private float offsetAngle;
    private void Awake()
    {
        smrs = new SkinnedMeshRenderer[2];
        gos = new GameObject[2];
        for (int i = 0; i < gos.Length; i++)
        {
            gos[i] = transform.GetChild(i).gameObject;
            smrs[i] = gos[i].GetComponentInChildren<SkinnedMeshRenderer>();

        }
        //uiSystem = this.GetSystem<IUISystem>();
        this.RegisterEvent<RotateCharacterModelEvent>(RotateCharacterModel);
        SetMaterial();
    }

    private void Update()
    {
    }



    public void SetMaterial() {
        // uiSystem.ChoicePlayerData = new PlayerData();
        //PlayerData pd = uiSystem.ChoicePlayerData;
        PlayerData pd = new PlayerData();
        for (int i = 0; i < gos.Length; i++)
        {            
            gos[i].SetActive(false);
        }
        currentCharacterGo = gos[(int)pd.gender];
        currentCharacterGo.SetActive(true);
        Material m = GameResSystem.GetRes<Material>("Materials/"+pd.gender.ToString()+"/"+(int)pd.role);
        for (int i = 0; i < smrs[(int)pd.gender].sharedMaterials.Length; i++) {
            if (pd.gender == GENDER.MALE) {

                if (i != 1 && i != 2 && i != 10)
                {
                    smrs[(int)pd.gender].materials[i].CopyPropertiesFromMaterial(m);
                }

            }
            else {
                if (i != 0 && i != 2 && i != 10)
                {
                    smrs[(int)pd.gender].materials[i].CopyPropertiesFromMaterial(m);
                }

            }
            
        }
    }

    private void RotateCharacterModel(object rotateSrc) {
        if (!currentCharacterGo) return;
        RotateModelSrc rs = (RotateModelSrc)rotateSrc;
        float h = rs.h;
        float v = rs.v;
        if (v != 0) {//ǰ��
            if (v > 0)//��������
            {
                offsetAngle = 45 * h;
            }
            else
            {//��������
                offsetAngle = -45 * h;
            }
        }
        else {//����
            if (h != 0) {//��������
                offsetAngle = 90 * h;
            }
            else//��ǰ��
            {
                offsetAngle = 0;

            }
        }
        currentCharacterGo.transform.localEulerAngles=new Vector3(0,initAngle+offsetAngle,0);
        this.SendEvent<GetLookTargetIndexEvent>(offsetAngle);
    }


}


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：让玩家可以正常左右移动并看向前方
//***************************************** 
public class PlayerHeadController : MonoBehaviour,IController
{
    public Transform currentSpine;
    public Transform targetPoint;
    //看向偏移角度
    private Vector3 spineAngle=new Vector3(-40.7f,1.7f,84.5f);
    private Transform[] points;
    private int currentindex;
    private bool stopLooking;
    private int gender;//

    void Start()
    {
        Transform maleLookTargetTrans = transform.Find("MaleHeadLookTarget");
        Transform femaleLookTargetTrans = transform.Find("FemaleHeadLookTarget");
        points = new Transform[maleLookTargetTrans.childCount+ femaleLookTargetTrans.childCount];
        for (int i = 0; i < maleLookTargetTrans.childCount; i++)
        {
            points[i] = maleLookTargetTrans.GetChild(i);
        }
        for (int i = 0; i < femaleLookTargetTrans.childCount; i++)
        {
            points[i+ maleLookTargetTrans.childCount] = femaleLookTargetTrans.GetChild(i);
        }
        stopLooking = true;
    }

    void LateUpdate()
    {
        if (stopLooking)
        {
            return;
        }
        currentSpine.LookAt(points[currentindex], Vector3.up);
        currentSpine.Rotate(spineAngle);
    }
    /// <summary>
    /// 获取当前性别下作用的游戏物体对象并找到对应骨骼
    /// </summary>
    /// <param name="taregetModelTrans"></param>
    public void InitPlayerHeadCtrl(Transform taregetModelTrans,int sex)
    {
        if (sex==0)
        {
            currentSpine = GameObject.Find("character/bloodelf/male/bloodelfmale_hd_bone_9").transform;
        }
        else
        {
            currentSpine = GameObject.Find("character/bloodelf/female/bloodelffemale_hd_bone_9").transform;
        }
        gender = sex;
    }

    public void GetLookTargetIndex(float offsetAngle)
    {
        switch (offsetAngle)
        {
            case 0:
                currentindex = 0+gender*5;
                break;
            case 45:
                currentindex = 1 + gender * 5;
                break;
            case -45:
                currentindex =2 + gender * 5;
                break;
            case -90:
                currentindex = 3 + gender * 5;
                break;
            case 90:
                currentindex = 4 + gender * 5;
                break;
            default:
                break;
        }
    }

    public void StopLookingFunction(bool stop,float delayTime=0)
    {
        if (delayTime > 0)
        {
            stopLooking = true;
            CancelInvoke();
            Invoke("DelayOpenLookingFunction", delayTime);
        }
        else
        {
            stopLooking = stop;
        }
    }

    private void DelayOpenLookingFunction()
    {
        stopLooking = false;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHeadController : MonoBehaviour,IController
{

    private Transform currentSpine;
    public Transform targetPoint;
    private Vector3 spineAngle=new Vector3(-40.7f,1.7f,84.5f);
    private Transform[] points;
    private int currentindex;
    private bool stopLooking;


    // Start is called before the first frame update
    void Start()
    {
        Transform maleLookTargetTrans = transform.Find("MaleHeadLookTarget");
        Transform femaleLookTargetTrans = transform.Find("FemaleHeadLookTarget");
        points = new Transform[maleLookTargetTrans.childCount + femaleLookTargetTrans.childCount];
        for (int i = 0; i < maleLookTargetTrans.childCount; i++)
        {
            points[i] = maleLookTargetTrans.GetChild(i);
        }
         for (int i = 0; i < femaleLookTargetTrans.childCount; i++)
        {
            points[i+maleLookTargetTrans.childCount] = femaleLookTargetTrans.GetChild(i);
        }
        this.RegisterEvent<GetLookTargetIndexEvent>(GetLookTargetIndex);
        this.RegisterEvent<StopLookingFunctionEvent>(StopLookingFunction);
        stopLooking = true;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (stopLooking) return;
        currentSpine.LookAt(points[currentindex],Vector3.up);
        currentSpine.Rotate(spineAngle);
    }

    public void InitPlayerHeadCtrl(Transform targetModelTrans) {
        currentSpine = GameObject.Find("character/bloodelf/male/bloodelfmale_hd_bone_9").transform;
    }

    private void GetLookTargetIndex(object offsetAngle) {
        float oa = (float)offsetAngle;
        switch (oa) {
            case 0:
                currentindex = 0;
                break;
            case 45:
                currentindex = 1;
                break;
            case -45:
                currentindex = 2;
                break;
            case -90:
                currentindex = 3;
                break;
            case 90:
                currentindex = 4;
                break;
            default:
                break;

        }
       
    }

    private void StopLookingFunction(object obj) {
        StopLookingSrc ss = (StopLookingSrc)obj;
        if (ss.delayTime > 0)
        {
            stopLooking = true;
            CancelInvoke();
            Invoke("DelayOpenLookingFunction",ss.delayTime);
        }
        else {
            stopLooking = ss.stop;
        }

    }

    private void DelayOpenLookingFunction() {
        stopLooking = false;
    }
}

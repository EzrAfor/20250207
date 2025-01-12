using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPMCtrl : PlayerMovementController
{

    private Vector3 lastPos;
    private Vector3 lastRot;
    private Vector3 targetPos;
    private Vector3 targetRot;
    private float receiveTime;
    private CHARACTERSTATE characterState;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SyncPosAndRot(PTSyncCharacter pt)
    {
        Vector3 pos = new Vector3(pt.x, pt.y, pt.z);
        Vector3 rot = new Vector3(pt.ex, pt.ey, pt.ez);
        targetRot= rot;
        targetPos = pos + (pos - lastPos) / 2;
        characterState = pt.characterState;
        lastPos = targetPos;
        lastRot = targetRot;
        receiveTime = Time.time;
        float x = 0, z = 0;
        if (Vector3.Distance(transform.position, targetPos) <= 0.05f)
        {
            x =z= 0;
        }else
        {
            x = Vector3.Cross(transform.forward,targetPos-transform.position).y;
            x = x > 0 ? 1 : -1;
            z = Vector3.Dot(transform.forward,targetPos-transform.position);
            z = z > 0 ? 1 : -1;
            if (ForwardBehindOrLeftRight(targetPos))
            {
                x = 0;
            }else
            {
                x = 1;
                z = 0;
            }
        }
        ic.SetInputValue(InputCode.VerticalMoveValue,z);
        ic.SetInputValue(InputCode.HorizontalMoveValue,x);
    }

    //判断前后左右影响度
    private bool ForwardBehindOrLeftRight(Vector3 targetPos)
    {
        targetPos = transform.worldToLocalMatrix.MultiplyPoint3x4(targetPos);
        if (Mathf.Abs(targetPos.z) - Mathf.Abs(targetPos.x)>=-0.05f)
        {
            return true;
        }else
        {
            return false;
        }
    }

}

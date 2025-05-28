using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：地图编辑工具摄像机视野控制
//***************************************** 
public class MapCamCtrl : MonoBehaviour
{
    public float speed = 10;//移动速度
    public float mouseSensitivity = 5;//镜头旋转角度
    private float angleY;
    private float angleX;

    void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        if (Input.GetMouseButton(1))
        {
            RotateCamera();
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        transform.Translate(new Vector3(h,0,v)*speed*Time.deltaTime);
    }

    private void RotateCamera()
    {
        float turnAngleY= Input.GetAxisRaw("Mouse X")*mouseSensitivity;
        angleY = angleY + turnAngleY;
        transform.eulerAngles = new Vector3(transform.eulerAngles.x,angleY,transform.eulerAngles.z);
        float turnAngleX = Input.GetAxisRaw("Mouse Y")*mouseSensitivity;
        angleX =Mathf.Clamp(angleX - turnAngleX,-90,90);
        transform.eulerAngles = new Vector3(angleX,transform.eulerAngles.y,transform.eulerAngles.z);
    }
}

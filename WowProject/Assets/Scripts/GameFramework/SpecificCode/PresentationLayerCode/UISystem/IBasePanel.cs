using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IBasePanel :IController
{
    public void OnInit();
    public void OnShow(params object[] objs);
    public void OnClose();
   
}

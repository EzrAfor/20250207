using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//*****************************************
//创建人： Trigger 
//功能说明：资源加载系统
//***************************************** 
public class GameResSystem : MonoBehaviour
{
    private static Dictionary<string, Object> resDict = new Dictionary<string, Object>();

    public static T GetRes<T>(string resPath) where T : Object
    {
        if (resDict.ContainsKey(resPath))
        {
            return resDict[resPath] as T;
        }
        else
        {
            Object res= Resources.Load(resPath);
            if (res is Texture2D)
            {
                Texture2D texture2D= res as Texture2D;
                Sprite sprite= Sprite.Create(texture2D, new Rect(0, 0, texture2D.width, texture2D.height), 
                    Vector2.one * 0.5f);
                resDict.Add(resPath,sprite);
                return sprite as T;
            }
            else
            {
                resDict.Add(resPath, res);
                return res as T;
            }
        }
    }
}

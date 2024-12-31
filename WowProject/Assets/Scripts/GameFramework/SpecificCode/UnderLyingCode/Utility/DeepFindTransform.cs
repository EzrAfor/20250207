using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeepFindTransform 
{
   public static Transform DeepFindChild(Transform root,string childName)
    {
        Transform result = null;
        result = root.Find(childName);
        if (!result) {
            foreach (Transform item in root) {
                result = DeepFindChild(item, childName);
                if (result!=null)
                {
                    return result; 
                }
            }
        }
        return result;

    }
    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Transform targetTrans;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position,targetTrans.position+new Vector3(0,1,0))<=0.1f)
        {
            Destroy(gameObject);
        }
        else
        {
            transform.Translate((targetTrans.position + new Vector3(0, 1, 0) - transform.position).normalized*speed*Time.deltaTime);
        }
    }
}

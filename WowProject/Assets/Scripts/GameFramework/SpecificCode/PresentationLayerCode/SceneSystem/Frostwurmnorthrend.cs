using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Frostwurmnorthrend : MonoBehaviour
{
    public ParticleSystem smokePS;
    public ParticleSystem firePS;

    private void Awake()
    {
        smokePS = transform.Find("Smoke").GetComponent<ParticleSystem>();
        firePS = DeepFindTransform.DeepFindChild(transform, "FrostSpray").GetComponent<ParticleSystem>();
        HideAll();
    }

    public void PlayGroundSmoke() {
        smokePS.gameObject.SetActive(true);
        smokePS.Play();
    }

    public void PlayIceFire() {
        firePS.gameObject.SetActive(true);
        firePS.Play();
    }

    public void StopIceFire() {
        firePS.Stop();
    }

    public void HideAll() {
        firePS.Stop();
        smokePS.Stop();
        firePS.gameObject.SetActive(false); 
        smokePS.gameObject.SetActive(false);
    }


}

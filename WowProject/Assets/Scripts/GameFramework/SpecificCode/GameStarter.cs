using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameStarter : MonoBehaviour,IController
{
    private StartArchitecture startArchitecture;

    void Awake()
    {
        startArchitecture = StartArchitecture.Instance;
        startArchitecture.SetGameArchitecture(new WowArchitecture());//∆Ù∂Ø”Œœ∑º‹ππ
        startArchitecture.InitAllModulesInArchitecture();
        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        this.GetSystem<INetSystem>().Update();
    }





}

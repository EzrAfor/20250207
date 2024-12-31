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

    private void Start()
    {
        INetSystem ins = this.GetSystem<INetSystem>();
        ins.Connect("127.0.0.1", 8888);
        ins.RegistPTListener("PTRegister",RegistUser);
        

    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.S)) {
            this.GetSystem<INetSystem>().Send(new PTRegister() { id="ToYJF",pw="123"});
        }
        this.GetSystem<INetSystem>().Update();
    }

    private void RegistUser(PTBase pt) {
        Debug.Log(pt.protoName);
    }

}

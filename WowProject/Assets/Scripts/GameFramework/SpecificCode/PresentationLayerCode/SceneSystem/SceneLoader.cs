using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoader : MonoBehaviour,IController
{
    // Start is called before the first frame update
    void Start()
    {
        this.SendCommand<SendEnterGamePTCommand>();
        this.SendCommand<RegistPTListenerCommand>(new PTSrc() {ptName= "PTEnterGameScene",listener= OnPTEnterGameScene });

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnPTEnterGameScene(PTBase pt)
    {
        PTEnterGameScene ptgs = (PTEnterGameScene)pt;
        this.SendCommand<OnPTEnterGameSceneCommand>(ptgs);

    }

}

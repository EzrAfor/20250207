using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static NetSystem;

public interface INetSystem : ISystem
{
    public void Connect(string ip, int port);
    public void Send(PTBase msg);
    public void Close();

    //public void Receive();
    public void Update() ;

    public void RegistPTListener(string ptName, PTListener listener);

    public void UnregistPTListener(string ptName, PTListener listener);



    public void SendPTEvent(string ptName, PTBase pt);
   
}

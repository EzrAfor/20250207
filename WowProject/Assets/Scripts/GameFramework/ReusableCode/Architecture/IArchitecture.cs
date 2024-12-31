using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IArchitecture
{
   
    void RegisterSystem<U>(U instance) where U : ISystem;
    
    void RegisterModel<U>(U instance) where U : IModel;
   
    void RegisterUtility<U>(U instance) where U : IUtility;
    
    public U GetSystem<U>() where U : class, ISystem;
  
    public U GetModel<U>() where U : class, IModel;
  
    public U GetUtility<U>() where U : class, IUtility;
    
    void RegisterEvent<U>(Action<object> onEvent) where U : new();
   
    void UnRegisterEvent<U>(Action<object> onEvent) where U : new();
   
    void SendEvent<U>(object dataObj) where U : new();
   
    void SendCommand<U>(object dataObj) where U : ICommand, new();
   
    void InitAllModules();
}

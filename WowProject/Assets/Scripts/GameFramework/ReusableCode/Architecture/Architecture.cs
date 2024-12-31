using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class Architecture<T> : IArchitecture where T : new()
{
    private IOCContainer iocContainer = new IOCContainer();
    private GameEventSystem gameEventSystem = new GameEventSystem();

    public Architecture() { Init(); }

    public void InitAllModules()
    {
        iocContainer.InitAllModules();
    }

    public void RegisterEvent<U>(Action<object> onEvent) where U : new()
    {
        gameEventSystem.Register<U>(onEvent);
    }

    public void RegisterModel<U>(U instance) where U : IModel
    {
        iocContainer.Register<U>(instance);
    }

    public void RegisterSystem<U>(U instance) where U : ISystem
    {
        iocContainer.Register<U>(instance);
    }

    public void RegisterUtility<U>(U instance) where U : IUtility
    {
        iocContainer.Register<U>(instance);
    }

    public void SendCommand<U>(object dataObj) where U : ICommand, new()
    {
        var command = new U();
        command.Execute(dataObj);
    }

    public void SendEvent<U>(object dataObj) where U : new()
    {
        gameEventSystem.Send<U>(dataObj);
    }

    public void UnRegisterEvent<U>(Action<object> onEvent) where U : new()
    {
        gameEventSystem.UnRegister<U>(onEvent);
    }

    protected abstract void Init();
    public U GetSystem<U>() where U : class, ISystem
    {
        return iocContainer.Get<U>();
    }
    public U GetModel<U>() where U : class, IModel
    {
        return iocContainer.Get<U>();
    }
    public U GetUtility<U>() where U : class, IUtility
    {
        return iocContainer.Get<U>();
    }
}

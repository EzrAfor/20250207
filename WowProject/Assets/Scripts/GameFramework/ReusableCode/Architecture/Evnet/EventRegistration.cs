using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class EventRegistration : IEventRegistraion
{
    public Action<object> OnEvent = obj => { };
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class State 
{
    protected AssassinFSM _fsm;

    public virtual void OnEnter() {}
    public virtual void OnUpdate() {}
    public virtual void OnExit() {}
}

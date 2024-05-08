using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideState : State
{
    public HideState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("HIDE: run away and hide from player to not let player see you. ");
    }

    public override void OnUpdate()
    {

    }

    public override void OnExit()
    {

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapTriggeredState : State
{
    public TrapTriggeredState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("TRAP_TRIGGERED: a trap has been triggered, go to the trap location to investigate. ");
        // set state text in UI
        _fsm.SetStateText("Trap Triggered State");
        // run towards trap location
        _fsm.Agent.speed = _fsm.RunSpeed;
    }

    public override void OnUpdate()
    {
        // switch to patrol state when reached trap location
        if (!(_fsm.Agent.remainingDistance <= _fsm.Agent.stoppingDistance)) return;
        _fsm.SwitchState(_fsm.Patrol);
    }
}

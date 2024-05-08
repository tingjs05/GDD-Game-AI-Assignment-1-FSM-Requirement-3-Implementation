using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : State
{
    Transform player;

    public FleeState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("FLEE: Run away from player until flee distance. ");
        // reset player transform output
        player = null;
        // set agent move speed to run speed
        _fsm.Agent.speed = _fsm.RunSpeed;
    }

    public override void OnUpdate()
    {
        // check if player is still within flee distance, if player not in flee distance, means sucessfully fled
        if (!_fsm.PlayerNearby(_fsm.FleeDistance, out player))
        {
            _fsm.SwitchState(_fsm.Patrol);
            return;
        }

        // run away from player
        _fsm.Agent.Move((_fsm.transform.position - player.position).normalized);
    }
}

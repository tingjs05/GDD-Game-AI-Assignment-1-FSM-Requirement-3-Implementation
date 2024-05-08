using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeState : State
{
    Transform player;
    Coroutine coroutine;

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
        // start coroutine to wait for state transition, ensure enemy does not stay in flee state for too long
        coroutine = _fsm.StartCoroutine(WaitForState());
    }

    public override void OnUpdate()
    {
        // check if player is still within flee distance, if player not in flee distance, means sucessfully fled
        if (!_fsm.PlayerNearby(_fsm.FleeDistance, out player))
        {
            _fsm.SwitchState(_fsm.Patrol);
            return;
        }

        // run away from player by setting destination as vector away from player
        _fsm.Agent.SetDestination(_fsm.transform.position +  
            ((_fsm.transform.position - player.position).normalized * (1f + _fsm.Agent.stoppingDistance)));
    }

    public override void OnExit()
    {
        // ensure only one coroutine runs at one time
        if (coroutine != null)
        {
            _fsm.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    IEnumerator WaitForState()
    {
        yield return new WaitForSeconds(_fsm.MaxFleeDuration);
        coroutine = null;
        // go into patrol state after max flee duration
        _fsm.SwitchState(_fsm.Patrol);
    }
}

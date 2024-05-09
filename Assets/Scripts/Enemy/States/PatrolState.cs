using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : State
{
    Transform player;
    Vector3 point;
    Coroutine coroutine;

    public PatrolState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("PATROL: randomly wander around the hallway. ");
        // set state text in UI
        _fsm.SetStateText("Patrol State");
        // reset random output point
        point = Vector3.zero;
        // reset coroutine
        coroutine = null;
        // set agent move speed to walk speed
        _fsm.Agent.speed = _fsm.WalkSpeed;
        // start coroutine for periodic push spot check
        coroutine = _fsm.StartCoroutine(PeriodicPushSpotCheck());
    }

    public override void OnUpdate()
    {
        // check for transition to alert state
        // check if player is within alert radius
        if (_fsm.PlayerNearby(_fsm.AlertRadius, out player))
        {
            _fsm.SwitchState(_fsm.Alert);
            return;
        }

        // otherwise, just patrol the area, set a random location to walk towards to patrol
        // ensure agent is not at stopping distance (aka has reached last set position)
        if (!(_fsm.Agent.remainingDistance <= _fsm.Agent.stoppingDistance)) return;
        // get a random point to walk to, ensure it is possible to get a position
        if (!_fsm.RandomPoint(_fsm.transform.position, _fsm.PatrolRadius, out point)) return;
        // set target position to walk towards
        _fsm.Agent.SetDestination(point);
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

    IEnumerator PeriodicPushSpotCheck()
    {
        // randomly wait for seconds between two ranges
        yield return new WaitForSeconds(Random.Range(_fsm.PushCheckCooldown.x, _fsm.PushCheckCooldown.y));
        // randomly has a chance to transition to wait state
        if (Random.Range(0f, 1f) < _fsm.PushTransitionChance)
            _fsm.SwitchState(_fsm.Wait);
        // if failed to switch to wait state, wait for another period
        else
            coroutine = _fsm.StartCoroutine(PeriodicPushSpotCheck());
    }
}

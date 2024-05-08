using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HideState : State
{
    List<Vector3> hidingSpotsOrdered;
    Coroutine coroutine;

    public HideState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("HIDE: run away and hide from player to not let player see you. ");
        // reset coroutine
        coroutine = null;
        // set agent move speed to run speed
        _fsm.Agent.speed = _fsm.RunSpeed;
        // set hiding spots list
        hidingSpotsOrdered = new List<Vector3>(HidingPositionManager.Instance.HidingSpots);
        // do not hide if there are no hiding spots
        if (hidingSpotsOrdered.Count <= 0)
        {
            _fsm.SwitchState(_fsm.Flee);
            return;
        }
        // sort hiding spots by distance from self
        hidingSpotsOrdered = hidingSpotsOrdered
            .OrderBy(x =>  Vector3.Distance(_fsm.transform.position, x))
            .Where(x => Vector3.Distance(_fsm.transform.position, x) >= _fsm.MinHideDistanceThreshold)
            .ToList();

        // go to hiding position
        _fsm.Agent.SetDestination(hidingSpotsOrdered[0]);
    }

    public override void OnUpdate()
    {
        // continue moving if still moving to hiding spot
        if (coroutine == null)
        {
            // check if destination has been reached, if so, start countdown
            if (_fsm.Agent.remainingDistance <= _fsm.Agent.stoppingDistance)
            {
                coroutine = _fsm.StartCoroutine(WaitForState());
            }
            return;
        }

        // when in hiding spot, do stuff
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
        yield return new WaitForSeconds(_fsm.MaxHideDuration);
        coroutine = null;
        // go into flee state after max hide duration
        _fsm.SwitchState(_fsm.Flee);
    }
}

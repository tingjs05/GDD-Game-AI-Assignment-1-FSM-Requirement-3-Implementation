using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayTrapState : State
{
    Coroutine coroutine;

    public LayTrapState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("LAY_TRAP: place down a trap at current position. ");
        // set state text in UI
        _fsm.SetStateText("Lay Trap State");
        // do not let agent move when in this state
        _fsm.Agent.speed = 0f;
        // place down trap
        _fsm.PlaceTrap();
        // start coroutine to count lay trap duration
        coroutine = _fsm.StartCoroutine(WaitForState());
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
        yield return new WaitForSeconds(_fsm.LayTrapDuration);
        coroutine = null;
        // go into patrol state after laying trap duration
        _fsm.SwitchState(_fsm.Patrol);
    }
}

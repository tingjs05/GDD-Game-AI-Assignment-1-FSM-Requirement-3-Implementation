using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunnedState : State
{
    Coroutine coroutine;

    public StunnedState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("STUN: cannot move or perform any actions during stun duration. ");
        // set state text in UI
        _fsm.SetStateText("Stunned State");
        // do not let agent move when in this state
        _fsm.Agent.speed = 0f;
        // start coroutine to count stun duration
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
        yield return new WaitForSeconds(_fsm.StunDuration);
        coroutine = null;
        // go into patrol state after stun duration
        _fsm.SwitchState(_fsm.Patrol);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : State
{
    Transform player;
    Coroutine coroutine;

    public AttackState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("ATTACK: attempt to damage the player. ");
        // reset player transform output
        player = null;
        // attack player
        if (_fsm.PlayerNearby(_fsm.AttackRange, out player))
        {
            // damage player
            Debug.Log("Hit player!");
        }
        // start coroutine to wait for state transition
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
        yield return new WaitForSeconds(_fsm.AttackDuration);
        coroutine = null;
        // go into flee state after attack duration
        _fsm.SwitchState(_fsm.Flee);
    }
}

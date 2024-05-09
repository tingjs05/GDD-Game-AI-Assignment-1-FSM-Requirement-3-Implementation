using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushState : State
{
    Coroutine coroutine;

    public PushState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("PUSH: push obstacle in an attempt to hit the player. ");
        // set state text in UI
        _fsm.SetStateText("Push State");
        // get reference to pushable object
        Collider[] hit = Physics.OverlapSphere(_fsm.transform.position, _fsm.Agent.stoppingDistance, LayerMask.GetMask("Obstacles"));
        // try to get pushable object script
        PushableObject pushableObject = null;
        if (hit.Length > 0) pushableObject = hit[0].transform.parent.GetComponent<PushableObject>();
        // switch back to patrol state if failed to get reference to obstacle or pushable object
        // drop object, ensure drop was successful, if not return to patrol state as well
        if (hit.Length <= 0 || pushableObject == null || !pushableObject.DropObject(out Vector3 pushSpot))
        {
            _fsm.SwitchState(_fsm.Patrol);
            return;
        }
        
        // if drop was successful
        // move agent to push spot
        _fsm.Agent.Move(pushSpot);
        // start coroutine to count exit time
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
        yield return new WaitForSeconds(_fsm.MaxWaitDuration);
        coroutine = null;
        // return to patrol after carrying out action
        _fsm.SwitchState(_fsm.Patrol);
    }
}

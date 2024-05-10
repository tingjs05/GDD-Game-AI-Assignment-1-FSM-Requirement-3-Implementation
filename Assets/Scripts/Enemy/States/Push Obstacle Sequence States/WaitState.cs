using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WaitState : State
{
    Vector3 targetPosition;
    Coroutine coroutine;
    float originalStoppingDistance;

    public WaitState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("WAIT: wait behind an object, preparing to ambush player. ");
        // set state text in UI
        _fsm.SetStateText("Wait State");
        // ensure hiding spot manager is not null
        if (HidingPositionManager.Instance == null) return;
        // reset coroutine
        coroutine = null;
        // save original agent stop distance
        originalStoppingDistance = _fsm.Agent.stoppingDistance;
        // set new agent stopping distance (don't stop too far from waiting spot)
        _fsm.Agent.stoppingDistance = 0.1f;
        // get position to move to
        targetPosition = HidingPositionManager.Instance.PushingSpots
            .OrderBy(x => Vector3.Distance(_fsm.transform.position, x))
            .ToArray()[0];
        // set agent move speed to walk speed
        _fsm.Agent.speed = _fsm.WalkSpeed;
        // move to target position
        _fsm.Agent.SetDestination(targetPosition);
    }

    public override void OnUpdate()
    {
        // check if agent has reached target destination
        if (!(_fsm.Agent.remainingDistance <= _fsm.Agent.stoppingDistance)) return;

        // check for transition to push state
        // get reference to pushable object
        Collider[] hit = Physics.OverlapSphere(_fsm.transform.position, _fsm.Agent.stoppingDistance, LayerMask.GetMask("Obstacles"));
        // check if anything is detected
        if (hit.Length > 0)
        {
            Vector3 frontOfObstacle = hit[0].transform.forward;
            // check for players within range, if there are, transition to push state
            hit = Physics.OverlapSphere(_fsm.transform.position + frontOfObstacle, _fsm.PlayerInObstacleRange);
            // check if the player is hit, if so, switch to push state
            foreach (Collider obj in hit)
            {
                if (obj.CompareTag("Player"))
                {
                    _fsm.SwitchState(_fsm.Push);
                    return;
                }
            }
        }

        // only start a new coroutine if there are currently no coroutines running
        if (coroutine != null) return;
        coroutine = _fsm.StartCoroutine(WaitForState());
    }

    public override void OnExit()
    {
        // reset agent stopping distance on exit
        _fsm.Agent.stoppingDistance = originalStoppingDistance;
        
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
        // return to patrol if push state transition is not met
        _fsm.SwitchState(_fsm.Patrol);
    }
}

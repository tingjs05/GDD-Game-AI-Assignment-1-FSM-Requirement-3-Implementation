using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : State
{
    Transform player;

    public AlertState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("ALERT: slowly move towards player if player is nearby. ");
        // set state text in UI
        _fsm.SetStateText("Alert State");
        // reset player transform output
        player = null;
        // set agent move speed to sneak speed
        _fsm.Agent.speed = _fsm.SneakSpeed;
    }

    public override void OnUpdate()
    {
        // check if player is still within alert radius, if not, continue patrolling
        if (!_fsm.PlayerNearby(_fsm.AlertRadius, out player))
        {
            _fsm.SwitchState(_fsm.Patrol);
            return;
        }

        // check if player is within line of sight, if in line of sight, transition to prowl state
        if (Physics.Raycast(_fsm.transform.position, (player.position - _fsm.transform.position).normalized, out RaycastHit hit,
            Vector3.Distance(player.position, _fsm.transform.position)) && hit.collider.CompareTag("Player"))
        {
            Debug.DrawRay(_fsm.transform.position, player.position - _fsm.transform.position, Color.red);
            _fsm.SwitchState(_fsm.Prowl);
            return;
        }

        // show line of sight ray even when player is not seen
        Debug.DrawRay(_fsm.transform.position, player.position - _fsm.transform.position, Color.yellow);
        // set target position to slowly walk towards player
        _fsm.Agent.SetDestination(player.position);
    }
}

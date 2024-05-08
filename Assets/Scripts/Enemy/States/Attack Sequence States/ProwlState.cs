using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProwlState : State
{
    Transform player;

    public ProwlState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("PROWL: run towards player if player is within line of sight. ");
        // reset player transform output
        player = null;
        // set agent move speed to run speed
        _fsm.Agent.speed = _fsm.RunSpeed;
    }

    public override void OnUpdate()
    {
        // check if player is still within alert radius
        if (!_fsm.PlayerNearby(_fsm.AlertRadius, out player))
        {
            _fsm.SwitchState(_fsm.Alert);
            return;
        }

        // check if player is within line of sight, if in line of sight, transition to prowl state
        RaycastHit hit;
        if (!Physics.Raycast(_fsm.transform.position, (player.position - _fsm.transform.position).normalized, out hit, 
            Vector3.Distance(player.position, _fsm.transform.position)) || !hit.collider.CompareTag("Player"))
        {
            Debug.DrawRay(_fsm.transform.position, player.position - _fsm.transform.position, Color.yellow);
            _fsm.SwitchState(_fsm.Alert);
            return;
        }

        // check if player is within attack range, if so, switch to attack state
        if (Vector3.Distance(_fsm.transform.position, player.position) <= _fsm.AttackRange)
        {
            _fsm.SwitchState(_fsm.Attack);
            return;
        }

        // show line of sight ray if player is seen
        Debug.DrawRay(_fsm.transform.position, player.position - _fsm.transform.position, Color.red);
        // set target position to run towards player
        _fsm.Agent.SetDestination(player.position);
    }
}

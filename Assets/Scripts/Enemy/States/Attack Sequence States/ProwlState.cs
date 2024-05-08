using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProwlState : State
{
    Transform player;
    Coroutine coroutine;

    public ProwlState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("PROWL: run towards player if player is within line of sight. ");
        // reset player transform output and coroutine
        player = null;
        coroutine = null;
        // set agent move speed to run speed
        _fsm.Agent.speed = _fsm.RunSpeed;
    }

    public override void OnUpdate()
    {
        // check if player is still within alert radius
        // check if player is within line of sight, if in line of sight, transition to prowl state
        if (!_fsm.PlayerNearby(_fsm.AlertRadius, out player) ||
            !Physics.Raycast(_fsm.transform.position, (player.position - _fsm.transform.position).normalized, out RaycastHit hit,
                Vector3.Distance(player.position, _fsm.transform.position)) ||
            !hit.collider.CompareTag("Player"))
        {
            _fsm.SwitchState(_fsm.Alert);
            return;
        }

        if (_fsm.PlayerIsMovingTowardsEnemy(player))
        {
            // only assign if coroutine is null
            coroutine ??= _fsm.StartCoroutine(TrueForSetTime());
            return;
        }
        
        // cancel coroutine if player breaks is moving towards enemy condition
        if (coroutine != null) _fsm.StopCoroutine(coroutine);

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

    public override void OnExit()
    {
        // ensure only one coroutine runs at one time
        if (coroutine != null)
        {
            _fsm.StopCoroutine(coroutine);
            coroutine = null;
        }
    }

    IEnumerator TrueForSetTime()
    {
        yield return new WaitForSeconds(_fsm.MinFaceEnemyDuration);
        coroutine = null;
        // switch to hide if player walking towards enemy for minFaceEnemyDuration
        _fsm.SwitchState(_fsm.Hide);
    }
}

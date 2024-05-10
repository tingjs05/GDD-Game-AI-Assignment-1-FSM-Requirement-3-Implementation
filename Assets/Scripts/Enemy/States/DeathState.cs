using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathState : State
{
    public DeathState(AssassinFSM fsm)
    {
        _fsm = fsm;
    }

    public override void OnEnter()
    {
        // log state transition
        Debug.Log("DEATH: destroy itself, game has ended. ");
        // set state text in UI
        _fsm.SetStateText("Death State");
        // log end of game
        Debug.Log("Enemy Died: Mission Sucessful!");
    }
}

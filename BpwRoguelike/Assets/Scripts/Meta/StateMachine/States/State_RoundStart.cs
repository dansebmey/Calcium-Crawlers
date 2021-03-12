using UnityEngine;

public class State_RoundStart : State
{
    public override void OnEnter()
    {
        Gm.hudManager.TransitionToNextRound(Gm.combatManager.CurrentRound);
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnExit()
    {
        
    }
}
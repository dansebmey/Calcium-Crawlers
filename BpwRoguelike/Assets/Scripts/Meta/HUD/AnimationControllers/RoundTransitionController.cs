using UnityEngine;

public class RoundTransitionController : GmAwareObject
{
    public void StartNewRound()
    {
        gm.combatManager.PrepareNewRound();
        gm.combatManager.StartNewRound();
    }
}
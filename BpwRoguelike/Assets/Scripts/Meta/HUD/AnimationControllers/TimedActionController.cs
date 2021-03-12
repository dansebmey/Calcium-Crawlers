using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedActionController : GmAwareObject
{
    public bool isEnemy;
    public enum Performance { Meh, Okay, Good, Perfect }

    public void MarkPerformance(Performance performance)
    {
        EventManager<Performance>.Invoke(isEnemy ? EventType.OnDefencePerformanceMarked : EventType.OnPerformanceMarked, performance);
    }

    public void FinishAction()
    {
        EventManager<Performance>.Invoke(isEnemy ? EventType.OnEnemyAbilityEnd : EventType.OnAbilityEnd, Performance.Meh); // filler argument
    }

    public void EndAttack()
    {
        if (!gm.combatManager.IsAbilityQueueEmpty())
        {
            gm.SwitchState(isEnemy ? typeof(State_EnemyTurn_Performance) : typeof(State_PlayerTurn_Performance));
        }
        else if (gm.CurrentState.GetType() != typeof(State_PlayerTurn_Loot)
                 && gm.CurrentState.GetType() != typeof(State_GameOver_Loss)
                 && gm.CurrentState.GetType() != typeof(State_GameOver_Win))
        {
            gm.combatManager.EndTurn();
        }
    }
}

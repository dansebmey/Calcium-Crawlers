using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/State-switch command")]
public class StateSwitchCommand : Command
{
    public enum StateToSwitchTo { Inventory, Performance, EndTurn }
    public StateToSwitchTo state;

    public bool alwaysQueueable;

    private void Awake()
    {
        cannotQueueReason = "";
    }

    public override bool CanBeQueued(Combatant performer, List<AbilityDetails> queuedAbilities)
    {
        return alwaysQueueable || base.CanBeQueued(performer, queuedAbilities);
    }

    public override bool OnSelected(GameManager gm)
    {
        if (base.OnSelected(gm))
        {
            if (state == StateToSwitchTo.Inventory)
            {
                gm.SwitchState(typeof(State_PlayerTurn_Inventory));   
            }
            else if (state == StateToSwitchTo.Performance)
            {
                if (!gm.combatManager.IsAbilityQueueEmpty())
                {
                    gm.SwitchState(typeof(State_PlayerTurn_Performance));
                }
                else
                {
                    gm.combatManager.EndTurn();
                }
            }
            else if (state == StateToSwitchTo.EndTurn)
            {
                gm.combatManager.EndTurn();
            }

            return true;
        }

        return true;
    }
}
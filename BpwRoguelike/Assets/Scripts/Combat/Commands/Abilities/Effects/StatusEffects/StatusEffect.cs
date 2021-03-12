using System.Collections.Generic;
using UnityEngine;

public abstract class StatusEffect
{
    private readonly Combatant _combatant;

    private int _remainingTurns;

    public StatusEffect(Combatant combatant, int duration)
    {
        _combatant = combatant;
        
        _remainingTurns = duration;
    }

    public bool OnRoundEnd(Combatant combatant)
    {
        _remainingTurns--;
        if (_remainingTurns == 0)
        {
            return true;
        }

        OnRoundEndEffect(combatant);
        return false;
    }

    protected abstract void OnRoundEndEffect(Combatant combatant);
}
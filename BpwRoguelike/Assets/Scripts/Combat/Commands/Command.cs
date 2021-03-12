using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class Command : ScriptableObject
{
    [Header("Command metadata")]
    public string commandName;
    public Sprite sprite;
    [TextArea(3, 10)] public string description;
    
    public List<Tag> tags;
    
    public enum Tag
    {
        Priority, Opening, Ending, Focused
    }

    [HideInInspector] public string cannotQueueReason;
    

    public virtual bool CanBeQueued(Combatant performer, List<AbilityDetails> queuedAbilities)
    {
        cannotQueueReason = "";
        
        foreach (AbilityDetails details in queuedAbilities)
        {
            if (details.command.tags.Contains(Tag.Ending))
            {
                cannotQueueReason = "An [Ending] command has already been queued";
            }
            else if (details.command.tags.Contains(Tag.Focused))
            {
                cannotQueueReason = "A [Focused] command has already been queued";
            }
            else if (tags.Contains(Tag.Focused))
            {
                cannotQueueReason = "[Focused] commands need an empty queue";
            }
            else if (tags.Contains(Tag.Opening))
            {
                cannotQueueReason = "[Opening] commands must be the first in queue";
            }

            if (cannotQueueReason != "")
            {
                return false;
            }
        }

        return true;
    }
    
    public virtual bool OnSelected(GameManager gm)
    {
        return CanBeQueued(gm.combatManager.CurrentActor, gm.combatManager.GetCopyOfCommandQueue());
    }

    public virtual int GetNetEnergyCost(Combatant combatant)
    {
        return 0;
    }

    public virtual bool ActorHasEnoughEnergy(Combatant combatant)
    {
        return true;
    }
}
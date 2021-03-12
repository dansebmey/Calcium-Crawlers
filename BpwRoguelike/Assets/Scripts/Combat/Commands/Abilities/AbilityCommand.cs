using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Command/Ability command")]
public class AbilityCommand : Command
{
    public enum AbilityPool
    {
        OneHandedPiercing, OneHandedSlashing, OneHandedCrushing,
        TwoHandedPiercing, TwoHandedSlashing, TwoHandedCrushing,
        LightBow, HeavyBow, Crossbow, Throwing, Polearm, Rapier, Pommel,
        LightShield, HeavyShield, MeteorHammer, VoidStaff
    }

    public enum Scope
    {
        SingleEnemy, SingleAlly, AOEEnemies, AOEAllies, Self, None, Tile
    }

    [Header("Ability properties")]
    [Range(0, 200)] public int baseEnergyCost = 10;
    public Scope scope;
    [Range(1, 5)] public int minRange = 1;
    [Range(1, 5)] public int maxRange = 1;
    public bool diagonal;

    [Header("Performance multipliers")]
    [Range(0.0f, 2.5f)] public float mehMultiplier = 0.75f;
    [Range(0.0f, 2.5f)] public float okayMultiplier = 1;
    [Range(0.0f, 2.5f)] public float goodMultiplier = 1.2f;
    [Range(0.0f, 2.5f)] public float perfectMultiplier = 1.25f;

    [Header("Damage effects")]
    public bool dealsDamage = true;
    public List<BonusDamageEffect> bonusDamageEffects;
    private readonly BaseDamageEffect _baseDamageEffect = new BaseDamageEffect();

    [Header("Utility effects")]
    public List<UtilityEffect> utilityEffects;
    
    [Header("Compatible weapon types")]
    public List<AbilityPool> compatibleWeaponTypes;

    public bool CanReach(GridPos from, GridPos to, Weapon weapon = null)
    {
        int distance = TileManager.DistanceBetween(from, to, diagonal);

        if (weapon != null)
        {
            return distance >= minRange + weapon.attackRangeModifier && distance <= maxRange + weapon.attackRangeModifier;
        }

        return distance >= minRange && distance <= maxRange;
    }

    public List<Tile> GetTilesWithinReach(Tile[,] tiles, Tile from, Weapon weapon = null)
    {
        if (scope == Scope.AOEEnemies || scope == Scope.AOEAllies)
        {
            return tiles.Cast<Tile>().Where(tile => CanReach(from.point, tile.point, weapon) && tile != from).ToList();   
        }
        if (scope == Scope.SingleEnemy || scope == Scope.SingleAlly)
        {
            return tiles.Cast<Tile>().Where(tile =>
                CanReach(from.point, tile.point, weapon) && tile != from && tile.Occupant != null).ToList();
        }
        // if (scope == Scope.SingleAlly)
        // {
        //     return tiles.Cast<Tile>().Where(tile =>
        //         CanReach(from.point, tile.point, weapon) && tile != from).ToList();
        // }
        
        return tiles.Cast<Tile>().ToList();
    }

    public List<Combatant> GetCombatantsWithinReach(Tile[,] tiles, Tile from)
    {
        List<Combatant> result = new List<Combatant>();
        
        List<Tile> tL = GetTilesWithinReach(tiles, from);
        foreach (Tile tile in tL)
        {
            if (tile.Occupant != null && tile.Occupant is Combatant combatant)
            {
                result.Add(combatant);
            }
        }

        return result;
    }

    public int CalculateNetDamageOutput(AbilityDetails details)
    {
        if (!dealsDamage)
        {
            Debug.LogError("Ability [" + details.command.commandName + "] called Ability.GetNetDamage() even" +
                           "though dealsDamage == false.");
            return -1;
        }
        
        return _baseDamageEffect.NetDamage(details);
    }

    public int GetBonusDamage(float baseDamage, AbilityDetails details, Combatant defender)
    {
        int result = 0;
        if (bonusDamageEffects != null && bonusDamageEffects.Count > 0)
        {
            result += bonusDamageEffects.Sum(dEffect => dEffect.GetBonusDamage(baseDamage, details, defender));
        }

        return result;
    }

    public override int GetNetEnergyCost(Combatant combatant)
    {
        return (int) (baseEnergyCost * combatant.loadout.weapon.EnergyCostMultiplier());
    }

    public override bool ActorHasEnoughEnergy(Combatant combatant)
    {
        return combatant.Energy >= GetNetEnergyCost(combatant);
    }

    public override bool CanBeQueued(Combatant performer, List<AbilityDetails> queuedAbilities)
    {
        cannotQueueReason = "";
        
        if (queuedAbilities.Count == 0 && performer.Energy < GetNetEnergyCost(performer))
        {
            cannotQueueReason = "Not enough energy";
            return false;
        }
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
            else if (queuedAbilities.Count == 5) // hardcoded...
            {
                cannotQueueReason = "Command queue is full";
            }
            else if (performer.Energy < GetNetEnergyCost(performer) + queuedAbilities.Sum(ad => ad.command.GetNetEnergyCost(performer)))
            {
                cannotQueueReason = "Not enough energy";
            }

            if (cannotQueueReason != "")
            {
                return false;
            }
        }

        return true;
    }

    public float GetPerformanceMultiplier(TimedActionController.Performance performance)
    {
        if (performance == TimedActionController.Performance.Meh)
        {
            return mehMultiplier;
        }
        else if (performance == TimedActionController.Performance.Okay)
        {
            return okayMultiplier;
        }
        else if (performance == TimedActionController.Performance.Good)
        {
            return goodMultiplier;
        }
        else if (performance == TimedActionController.Performance.Perfect)
        {
            return perfectMultiplier;
        }

        return mehMultiplier;
    }

    public override bool OnSelected(GameManager gm)
    {
        if (base.OnSelected(gm))
        {
            EventManager<AbilityCommand>.Invoke(EventType.OnAbilityCommandSelected, this);
            if (scope != Scope.None)
            {
                gm.SwitchState(typeof(State_PlayerTurn_PickTarget));   
            }
            else
            {
                gm.combatManager.EnqueueCommand(new AbilityDetails(this, gm.combatManager.CurrentActor, null));
            }

            return true;
        }

        return false;
    }
}
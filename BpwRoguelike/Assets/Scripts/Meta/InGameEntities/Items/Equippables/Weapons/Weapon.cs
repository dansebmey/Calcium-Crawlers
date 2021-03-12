using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Weapon")]
public class Weapon : Item
{
    public List<AbilityCommand.AbilityPool> compatibleAbilityPools;

    [Range(-1, 1)] public int attackRangeModifier;
    
    [Header("Accuracy: higher value = more consistent high damage rolls")]
    [Range(-3, 3)] public int accuracy;
    [Header("Damage: higher value = higher maximum damage")]
    [Range(-3, 3)] public int damage;
    [Header("Lethality: higher value = higher armour penetration (flat)")]
    [Range(-3, 3)] public int lethality;
    [Header("Mobility: higher value = lower energy cost multiplier")]
    [Range(-3, 3)] public int mobility;

    public bool CanPerformAbility(AbilityCommand command)
    {
        foreach (AbilityCommand.AbilityPool compatibleAbilityPool in compatibleAbilityPools)
        {
            if (command.compatibleWeaponTypes.Contains(compatibleAbilityPool))
            {
                return true;
            }
        }

        return false;
    }

    public float EnergyCostMultiplier()
    {
        return 1 + mobility * -0.2f;
    }

    public override void OnUseAsAbility(Combatant user)
    {
        base.OnUseAsAbility(user);

        if (user.loadout.weapon == this)
        {
            user.loadout.SetWeapon(null);
        }
        else
        {
            user.loadout.SetWeapon(this);
            if (IsTwoHanded())
            {
                user.loadout.SetShield(null);
            }   
        }
    }

    public bool IsTwoHanded()
    {
        return compatibleAbilityPools.Contains(AbilityCommand.AbilityPool.TwoHandedPiercing)
               || compatibleAbilityPools.Contains(AbilityCommand.AbilityPool.TwoHandedSlashing)
               || compatibleAbilityPools.Contains(AbilityCommand.AbilityPool.TwoHandedCrushing);
    }
}
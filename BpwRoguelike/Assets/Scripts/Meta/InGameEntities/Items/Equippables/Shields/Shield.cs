using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item/Shield")]
public class Shield : ArmourItem
{
    public List<AbilityCommand.AbilityPool> compatibleAbilityPools;
    
    public override float DmgAbsorbRatio()
    {
        return coverage * 0.01f;
    }

    // 1:1 duplicate of Weapon.CanPerformAbility()... there must be a more clean solution
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

    public override void OnUseAsAbility(Combatant user)
    {
        base.OnUseAsAbility(user);

        if (user.loadout.shield == this)
        {
            user.loadout.SetShield(null);
        }
        else
        {
            user.loadout.SetShield(this);
            if (user.loadout.weapon.IsTwoHanded())
            {
                user.loadout.SetWeapon(null);
            }   
        }
    }
}
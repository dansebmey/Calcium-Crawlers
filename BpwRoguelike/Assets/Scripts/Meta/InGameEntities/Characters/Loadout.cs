using System;
using UnityEngine;

public class Loadout : MonoBehaviour
{
    public Weapon weapon;
    public Armour armour;
    public Shield shield;
    public Trinket trinket;

    private void Start()
    {
        Combatant owner = GetComponent<Combatant>();
        if (weapon != null)
        {
            weapon.AssignTo(owner);
        }
        if (armour != null)
        {
            armour.AssignTo(owner);
        }
        if (shield != null)
        {
            shield.AssignTo(owner);
        }
        if (trinket != null)
        {
            trinket.AssignTo(owner);
        }
    }

    public float DetermineFlatArmourPen(TimedActionController.Performance performance)
    {
        if (performance == TimedActionController.Performance.Perfect)
        {
            return 0.4f + weapon.lethality * 0.1f;
        }
        return 0;
    }

    public float DmgAbsorbRatio()
    {
        if (shield != null)
        {
            return shield.DmgAbsorbRatio();
        }
        if (weapon.compatibleAbilityPools.Contains(AbilityCommand.AbilityPool.Polearm))
        {
            return 0.9f;
        }

        return 0;
    }

    public int DmgReductionBuffer()
    {
        if (shield != null)
        {
            return shield.sturdiness;
        }
        if (weapon.compatibleAbilityPools.Contains(AbilityCommand.AbilityPool.Polearm))
        {
            return 10;
        }

        return 0;
    }

    public void SetWeapon(Weapon newWeapon)
    {
        weapon = newWeapon;
    }

    public void SetShield(Shield newShield)
    {
        shield = newShield;
    }

    public void SetArmour(Armour newArmour)
    {
        armour = newArmour;
    }

    public void SetTrinket(Trinket newTrinket)
    {
        trinket = newTrinket;
    }

    public bool IsItemEquipped(Item item)
    {
        return weapon == item || armour == item || shield == item || trinket == item;
    }
}
using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Bonus Damage/Based on missing HP")]
public class MissingHPDamageEffect : BonusDamageEffect
{
    public override int GetBonusDamage(float baseDamage, AbilityDetails details, Combatant defender)
    {
        int missingHP = defender.maxHP - defender.Hitpoints;
        float netRatio = GetRatio(details.performance);
        
        return (int) Mathf.Clamp(missingHP * netRatio, 0, baseDamage * maxBonusToBaseDamageRatio);
    }
}
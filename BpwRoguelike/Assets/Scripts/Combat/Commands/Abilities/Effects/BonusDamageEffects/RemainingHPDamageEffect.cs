using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Bonus Damage/Based on remaining HP")]
public class RemainingHPDamageEffect : BonusDamageEffect
{
    public override int GetBonusDamage(float baseDamage, AbilityDetails details, Combatant defender)
    {
        return (int) Mathf.Clamp(defender.Hitpoints * GetRatio(details.performance), 0, baseDamage * maxBonusToBaseDamageRatio);
    }
}
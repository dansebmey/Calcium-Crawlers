using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Bonus Damage/Based on target's direction")]
public class DirectionBasedDamageEffect : BonusDamageEffect
{
    [Range(0, 2.5f)] public float damageMultiplier = 1;
    
    public override int GetBonusDamage(float baseDamage, AbilityDetails details, Combatant defender)
    {
        if (defender.Dir == details.performer.Dir)
        {
            return (int) (baseDamage * (damageMultiplier * GetRatio(details.performance) - 1));
        }
        
        return 0;
    }
}
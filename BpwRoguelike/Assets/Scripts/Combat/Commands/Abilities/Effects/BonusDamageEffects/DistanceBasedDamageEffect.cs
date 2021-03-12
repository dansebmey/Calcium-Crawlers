using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Bonus Damage/Based on distance")]
public class DistanceBasedDamageEffect : BonusDamageEffect
{
    public override int GetBonusDamage(float baseDamage, AbilityDetails details, Combatant defender)
    {
        int tilesTravelled = TileManager.DistanceBetween(details.performer.StartingPoint, details.performer.point);

        return (int) (baseDamage * GetRatio(details.performance) * tilesTravelled);
    }
}
using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Utility/Heal")]
public class HealEffect : UtilityEffect
{
    [Header("Heal")]
    [Range(0, 1.0f)] public float missingHPRatio;
    [Range(0, 1.0f)] public float maxHPRatio;

    protected override void OnApplyEffect(Combatant primaryTarget, AbilityDetails details)
    {
        int missingHP = details.performer.maxHP - details.performer.Hitpoints;
        foreach (GridObject target in details.targets)
        {
            if (target is Combatant combatant)
            {
                combatant.Hitpoints += (int) (missingHP * missingHPRatio + details.performer.maxHP * maxHPRatio);   
            }
        }
    }
}
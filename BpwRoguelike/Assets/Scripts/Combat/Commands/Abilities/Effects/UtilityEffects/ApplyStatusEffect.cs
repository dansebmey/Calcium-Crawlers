using UnityEngine;

public abstract class ApplyStatusEffect : UtilityEffect
{
    [Header("ApplyStatusEffect")]
    public int duration = 1;

    protected StatusEffect statusEffect;
    protected abstract StatusEffect NewStatusEffect(Combatant target);
    
    protected override void OnApplyEffect(Combatant primaryTarget, AbilityDetails details)
    {
        primaryTarget.ApplyStatusEffect(NewStatusEffect(primaryTarget));
    }
}
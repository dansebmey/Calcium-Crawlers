using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Utility/Off-balance")]
public class ApplyOffBalance : ApplyStatusEffect
{
    protected override StatusEffect NewStatusEffect(Combatant target)
    {
        return new OffBalance(target, duration);
    }
}
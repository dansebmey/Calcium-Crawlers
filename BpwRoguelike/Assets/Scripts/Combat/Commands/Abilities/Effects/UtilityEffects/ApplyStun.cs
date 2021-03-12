using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Utility/Stun")]
public class ApplyStun : ApplyStatusEffect
{
    protected override StatusEffect NewStatusEffect(Combatant target)
    {
        return new Stun(target, duration);
    }
}
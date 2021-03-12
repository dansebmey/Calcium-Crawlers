using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Utility/Use item")]
public class UseItemEffect : UtilityEffect
{
    [Header("UseItemEffect")]
    public Item item;
    
    protected override void OnApplyEffect(Combatant primaryTarget, AbilityDetails details)
    {
        item.OnUseAsAbility(primaryTarget);
    }
}
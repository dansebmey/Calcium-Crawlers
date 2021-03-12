using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Utility/Discard item")]
public class DiscardItemEffect : UtilityEffect
{
    [Header("UseItemEffect")]
    public Item item;
    
    protected override void OnApplyEffect(Combatant primaryTarget, AbilityDetails details)
    {
        item.OnRemoveAsAbility();
    }
}
using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Utility/Access inventory")]
public class AccessInventory : UtilityEffect
{
    protected override void OnApplyEffect(Combatant primaryTarget, AbilityDetails details)
    {
        details.performer.AccessInventory();
    }
}
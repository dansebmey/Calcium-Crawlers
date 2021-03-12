using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Item/Trinkets/Trinket")]
public class Trinket : Item
{
    [Range(-3, 3)] public int movRangeModifier;
    [Range(-3, 3)] public int atkRangeModifier;

    public override void OnUseAsAbility(Combatant user)
    {
        base.OnUseAsAbility(user);

        user.loadout.SetTrinket(user.loadout.trinket == this ? null : this);
    }
}
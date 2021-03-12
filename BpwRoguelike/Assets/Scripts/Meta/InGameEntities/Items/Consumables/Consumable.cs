using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(menuName = "Item/Consumable")]
public class Consumable : Item
{
    public List<AbilityCommand> containedAbilities;
    
    public override void OnUseAsAbility(Combatant user)
    {
        base.OnUseAsAbility(user);
        
        foreach (AbilityCommand ability in containedAbilities)
        {
            user.PerformAbility(new AbilityDetails(ability, user));
        }

        user.inventory.items.Remove(this);
    }
}
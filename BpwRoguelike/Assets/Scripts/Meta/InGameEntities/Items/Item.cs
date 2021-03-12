using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    [HideInInspector] public Combatant owner;

    public string itemName;
    public Sprite sprite;
    [TextArea(3, 10)] public string description;
    [Range(0.1f, 5)] public float weight;
    [HideInInspector] public bool isQueued;

    public void AssignTo(Combatant newOwner)
    {
        owner = newOwner;
        owner.inventory.AddItem(this);
    }

    public void OnUseFromInventory(GameManager gm)
    {
        AbilityDetails details = new AbilityDetails(gm.combatManager.useItemAbility, gm.combatManager.CurrentActor, new List<GridObject>{gm.combatManager.CurrentActor});
        details.command.sprite = sprite;
        UseItemEffect effect = (UseItemEffect)details.command.utilityEffects.First(e => e is UseItemEffect);
        effect.item = this;

        gm.combatManager.EnqueueCommand(details);
        isQueued = true;
    }

    public virtual void OnUseAsAbility(Combatant user)
    {
        isQueued = false;
    }

    public void OnRemoveFromInventory(GameManager gm)
    {
        AbilityDetails details = new AbilityDetails(gm.combatManager.discardItemAbility, gm.combatManager.CurrentActor, new List<GridObject>{gm.combatManager.CurrentActor});
        details.command.sprite = sprite;
        DiscardItemEffect effect = (DiscardItemEffect)details.command.utilityEffects.First(e => e is DiscardItemEffect);
        effect.item = this;
        
        gm.combatManager.EnqueueCommand(details);
        isQueued = true;
    }
    
    public void OnRemoveAsAbility()
    {
        owner.inventory.items.Remove(this);
        isQueued = false;
    }
}
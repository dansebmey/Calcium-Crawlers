using System.Collections.Generic;
using UnityEngine;

public class InventoryInterface : ItemInterface
{
    private Combatant _owner;

    public void AssignTo(Combatant combatant)
    {
        _owner = combatant;
    }

    protected override Vector2 AnchoredPosition(float bgWidth)
    {
        return new Vector2(bgWidth / 2, 0);
    }

    protected override string DetermineInteractText()
    {
        return SelectedItemSlot.item is Consumable ?
            "Press SPACE to consume (ends turn)" : "Press SPACE to equip (ends turn)";
    }

    protected override List<Item> GetItems()
    {
        return _owner.inventory.items;
    }

    public void UseSelectedItem()
    {
        if (!SelectedItemSlot.item.isQueued)
        {
            if (SelectedItemSlot.item is Consumable)
            {
                SelectedItemSlot.item.OnUseFromInventory(gm);
                gm.SwitchState(typeof(State_PlayerTurn_PickAbility));   
            }
            else
            {
                SelectedItemSlot.item.OnUseAsAbility(_owner);
                Show(true);
            }
        }
    }

    public void DestroySelectedItem()
    {
        if (!SelectedItemSlot.item.isQueued)
        {
            SelectedItemSlot.item.OnRemoveAsAbility();
            Show(true);
        }
    }
}
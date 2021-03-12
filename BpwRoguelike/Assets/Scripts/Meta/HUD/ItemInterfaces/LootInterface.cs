using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LootInterface : ItemInterface
{
    private List<Item> _lootables = new List<Item>();

    public void AddItem(Item lootable)
    {
        _lootables.Add(lootable);
    }

    public void LootSelectedItem()
    {
        SelectedItemSlot.item.AssignTo(gm.combatManager.CurrentActor);
        SelectedItemSlot.AssignTo(null, gm.combatManager.CurrentActor);
        gm.hudManager.inventoryInterface.Show(true);

        int remainingItemsToLoot = itemSlots.Count(slot => slot.item != null);
        if (remainingItemsToLoot == 0)
        {
            gm.combatManager.EndTurn();
        }
        else
        {
            ToNextItem();
        }
    }

    protected override Vector2 AnchoredPosition(float bgWidth)
    {
        return new Vector2(-bgWidth / 2, 0);
    }

    protected override string DetermineInteractText()
    {
        return "Press SPACE to loot";
    }

    protected override List<Item> GetItems()
    {
        return _lootables;
    }

    public void ClearLoot()
    {
        _lootables.Clear();
        foreach (ItemSlot itemSlot in itemSlots)
        {
            itemSlot.AssignTo(null, null);
        }
    }
}
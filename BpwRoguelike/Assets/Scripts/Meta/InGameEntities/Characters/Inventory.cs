using System;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : GmAwareObject
{
    public int itemLimit = 16;
    [HideInInspector] public List<Item> items = new List<Item>();
    private Combatant _owner;

    public List<Item> startingItems;

    protected override void Start()
    {
        base.Start();
        foreach (Item item in startingItems)
        {
            item.AssignTo(_owner);
        }
    }

    public bool ContainsItemWithAbility(AbilityCommand command)
    {
        foreach (Item item in items)
        {
            if (item is Consumable c && c.containedAbilities.Contains(command))
            {
                return true;
            }
        }

        return false;
    }

    public void AddItem(Item item)
    {
        items.Add(item);
        item.isQueued = false;
    }

    public void AddItemAtIndex(Item item, int index)
    {
        items[index] = item;
        item.isQueued = false;
    }

    public int GetIndexOfItem(Item item)
    {
        for (int i = 0; i < items.Count; i++)
        {
            if (items[i] == item)
            {
                return i;
            }
        }

        Debug.LogWarning("Item ["+item.itemName+"] not found in inventory");
        return -1;
    }

    public void AssignTo(Combatant combatant)
    {
        _owner = combatant;
    }
}
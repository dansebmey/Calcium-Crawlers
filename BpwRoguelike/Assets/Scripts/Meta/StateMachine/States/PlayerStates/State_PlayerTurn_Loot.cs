using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class State_PlayerTurn_Loot : State_PlayerTurn
{
    public override void OnEnter()
    {
        base.OnEnter();

        InputController.BindActionToKey(Button.MoveUp, null, Gm.hudManager.lootInterface.ToFirstItem);
        InputController.BindActionToKey(Button.MoveLeft, null, Gm.hudManager.lootInterface.ToPreviousItem);
        InputController.BindActionToKey(Button.MoveDown, null, Gm.hudManager.lootInterface.ToLastItem);
        InputController.BindActionToKey(Button.MoveRight, null, Gm.hudManager.lootInterface.ToNextItem);
        InputController.BindActionToKey(Button.Interact, null, Gm.hudManager.lootInterface.LootSelectedItem);
        InputController.BindActionToKey(Button.Back, null, Gm.combatManager.EndTurn);
        
        Gm.hudManager.inputInfoInterface.ShowCommands(new Dictionary<string, string>
        {
            {"WASD", "Browse loot"},
            {"SPACE", "Loot item"},
            {"E", "End turn"}
        });
        
        Gm.hudManager.inventoryInterface.Show(true);
        Gm.hudManager.lootInterface.Show(true);
        Gm.hudManager.lootInterface.ToFirstItem();
        Gm.hudManager.itemDescriptionPanel.AssignTo(Gm.hudManager.lootInterface.itemSlots[0].item);
        Gm.hudManager.itemDescriptionPanel.gameObject.SetActive(true);
    }

    public override void OnExit()
    {
        base.OnExit();
        Gm.hudManager.inventoryInterface.Show(false);
        Gm.hudManager.lootInterface.Show(false);
        Gm.hudManager.itemDescriptionPanel.gameObject.SetActive(false);
    }
}
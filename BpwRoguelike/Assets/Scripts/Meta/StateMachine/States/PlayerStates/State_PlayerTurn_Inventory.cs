using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class State_PlayerTurn_Inventory : State_PlayerTurn
{
    public override void OnEnter()
    {
        base.OnEnter();

        InputController.BindActionToKey(Button.MoveUp, null, Gm.hudManager.inventoryInterface.ToFirstItem);
        InputController.BindActionToKey(Button.MoveLeft, null, Gm.hudManager.inventoryInterface.ToPreviousItem);
        InputController.BindActionToKey(Button.MoveDown, null, Gm.hudManager.inventoryInterface.ToLastItem);
        InputController.BindActionToKey(Button.MoveRight, null, Gm.hudManager.inventoryInterface.ToNextItem);
        InputController.BindActionToKey(Button.Interact, null, Gm.hudManager.inventoryInterface.UseSelectedItem);
        InputController.BindActionToKey(Button.Delete, null, Gm.hudManager.inventoryInterface.DestroySelectedItem);
        InputController.BindActionToKey(Button.Back, null, ReturnToAbilityState);
        
        Gm.hudManager.inputInfoInterface.ShowCommands(new Dictionary<string, string>
        {
            {"WASD", "Browse items"},
            {"SPACE", "(Un)equip/consume item"},
            {"DEL", "Discard item"},
            {"E", "Return to command panel"}
        });
        
        Gm.hudManager.inventoryInterface.Show(true);
        Gm.hudManager.inventoryInterface.ToFirstItem();
        if (CurrentActor.inventory.items.Count > 0)
        {
            Gm.hudManager.itemDescriptionPanel.AssignTo(Gm.hudManager.inventoryInterface.itemSlots[0].item);
            Gm.hudManager.itemDescriptionPanel.gameObject.SetActive(true);
        }
    }

    private void ReturnToAbilityState()
    {
        Gm.SwitchState(typeof(State_PlayerTurn_PickAbility));
    }

    public override void OnExit()
    {
        base.OnExit();
        Gm.hudManager.inventoryInterface.Show(false);
        Gm.hudManager.itemDescriptionPanel.gameObject.SetActive(false);
    }
}
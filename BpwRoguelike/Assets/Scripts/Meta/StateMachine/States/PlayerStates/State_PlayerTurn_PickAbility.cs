using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State_PlayerTurn_PickAbility : State_PlayerTurn
{
    public override void Init(GameManager gm)
    {
        base.Init(gm);
    }

    public override void OnEnter()
    {
        base.OnEnter();
        
        Gm.tileManager.RemoveFlags(Tile.Flag.HighlightedForMovement);
        Gm.hudManager.commandInterface.ShowCompatibleCommands(Gm.combatManager, CurrentActor);
        
        InputController.BindActionToKey(Button.Interact, null, ExecuteSelectedCommand);
        InputController.BindActionToKey(Button.MoveLeft, null, Gm.hudManager.commandInterface.ToPreviousCommand);
        InputController.BindActionToKey(Button.MoveRight, null, Gm.hudManager.commandInterface.ToNextCommand);
        InputController.BindActionToKey(Button.MoveUp, null, Gm.hudManager.commandInterface.ToFirstCommand);
        InputController.BindActionToKey(Button.MoveDown, null, Gm.hudManager.commandInterface.ToLastCommand);
        InputController.BindActionToKey(Button.Confirm, null, AdvanceToPerformanceState);
        InputController.BindActionToKey(Button.Back, null, RemoveLastAbility);
        
        Gm.hudManager.inputInfoInterface.ShowCommands(new Dictionary<string, string>
        {
            {"WASD", "Browse commands"},
            {"SPACE", "Enqueue command"},
            {"E", "Remove last command"}
        });

        Gm.hudManager.commandInterface.UpdateInterface();
    }

    private void RemoveLastAbility()
    {
        if (!Gm.combatManager.IsAbilityQueueEmpty())
        {
            RemoveAbilityItemLock();
            Gm.combatManager.RemoveLastQueuedAbility();
            
            Gm.hudManager.commandInterface.ShowCompatibleCommands(Gm.combatManager, CurrentActor);

            if (Gm.combatManager.IsAbilityQueueEmpty())
            {
                Gm.hudManager.inputInfoInterface.ShowCommands(new Dictionary<string, string>
                {
                    {"WASD", "Browse commands"},
                    {"SPACE", "Enqueue command"},
                    {"E", "Close panel"}
                });
            }
        }
        else
        {
            EventManager<List<AbilityDetails>>.Invoke(EventType.OnAbilityQueueChanged, null);
            Gm.SwitchState(typeof(State_PlayerTurn_Move));
        }
    }

    private void RemoveAbilityItemLock()
    {
        AbilityCommand ability = Gm.combatManager.CurrentAbilityDetails.command;

        if (ability.utilityEffects.Any(effect => effect is UseItemEffect))
        {
            UseItemEffect useItemEffect = (UseItemEffect) ability.utilityEffects.First(effect => effect is UseItemEffect);
            useItemEffect.item.isQueued = false;   
        }
        else if (ability.utilityEffects.Any(effect => effect is DiscardItemEffect))
        {
            DiscardItemEffect discardItemEffect = (DiscardItemEffect) ability.utilityEffects.First(effect => effect is DiscardItemEffect);
            discardItemEffect.item.isQueued = false;   
        }
    }

    private void SwitchToMoveState()
    {
        // Gm.combatManager.ClearAbilityQueue();
        Gm.SwitchState(typeof(State_PlayerTurn_Move));
    }

    private void ExecuteSelectedCommand()
    {
        Gm.hudManager.inputInfoInterface.ShowCommands(new Dictionary<string, string>
        {
            {"WASD", "Browse commands"},
            {"SPACE", "Enqueue command"},
            {"E", "Remove last command"}
        });
        Gm.hudManager.commandInterface.ExecuteCommand();
    }

    private void AdvanceToPerformanceState()
    {
        if (!Gm.combatManager.IsAbilityQueueEmpty())
        {
            // Gm.combatManager.ConfirmAbilityQueue();
            Gm.SwitchState(typeof(State_PlayerTurn_Performance));
        }
    }

    public override void OnExit()
    {
        base.OnExit();
        
        Gm.hudManager.commandInterface.Hide();
    }
}
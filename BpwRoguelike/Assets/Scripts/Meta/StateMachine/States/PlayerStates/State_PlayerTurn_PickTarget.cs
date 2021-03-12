using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State_PlayerTurn_PickTarget : State_PlayerTurn
{
    private AbilityCommand _currentCommand;
    
    private List<Tile> _potentialTargetTiles;
    private Tile _primaryTargetTile;

    public override void Init(GameManager gm)
    {
        base.Init(gm);
        
        EventManager<AbilityCommand>.AddListener(EventType.OnAbilityCommandSelected, SetCurrentAbility);
    }

    public override void OnEnter()
    {
        base.OnEnter();

        // TODO: Add target selection per ability
        InputController.BindActionToKey(Button.Interact, null, ConfirmTarget);
        InputController.BindActionToKey(Button.MoveUp, null, PreviousTarget);
        InputController.BindActionToKey(Button.MoveLeft, null, PreviousTarget);
        InputController.BindActionToKey(Button.MoveDown, null, NextTarget);
        InputController.BindActionToKey(Button.MoveRight, null, NextTarget);
        InputController.BindActionToKey(Button.Back, null, ReturnToAbilitySelectionState);
        
        Gm.hudManager.inputInfoInterface.ShowCommands(new Dictionary<string, string>
        {
            {"WASD", "Browse targets"},
            {"SPACE", "Confirm target"},
            {"E", "Cancel command"}
        });
    }

    private void ReturnToAbilitySelectionState()
    {
        Gm.SwitchState(typeof(State_PlayerTurn_PickAbility));
    }

    private void SetCurrentAbility(AbilityCommand command)
    {
        _currentCommand = command;
        if (_currentCommand.scope != AbilityCommand.Scope.Self)
        {
            _potentialTargetTiles = _currentCommand.GetTilesWithinReach(Gm.tileManager.dungeonGen.grid, CurrentActor.Tile);
            SetPrimaryTarget(_potentialTargetTiles[0]);   
        }
        else
        {
            _potentialTargetTiles = new List<Tile>{CurrentActor.Tile};
            SetPrimaryTarget(CurrentActor.Tile);
        }
    }

    private void ConfirmTarget()
    {
        Gm.combatManager.EnqueueCommand(new AbilityDetails(_currentCommand, CurrentActor, DetermineTargets()));
        Gm.SwitchState(typeof(State_PlayerTurn_PickAbility));
    }

    private void SetPrimaryTarget(Tile tile)
    {
        Gm.tileManager.RemoveFlags(Tile.Flag.SelectedForAttack);
        
        _primaryTargetTile = tile;
        if (_currentCommand.scope == AbilityCommand.Scope.AOEAllies || _currentCommand.scope == AbilityCommand.Scope.AOEEnemies)
        {
            foreach (Tile t in _potentialTargetTiles)
            {
                t.AddFlag(Tile.Flag.HighlightedForAttack);

                if (t.Occupant != null)
                {
                    t.AddFlag(Tile.Flag.SelectedForAttack);   
                }
            }
        }
        else
        {
            _primaryTargetTile.AddFlag(Tile.Flag.SelectedForAttack);
        }
    }
    
    private void PreviousTarget()
    {
        SelectTarget(-1);
    }
    
    private void NextTarget()
    {
        SelectTarget(+1);
    }

    private void SelectTarget(int indexDelta)
    {
        Gm.tileManager.RemoveFlags(Tile.Flag.HighlightedForAttack, Tile.Flag.SelectedForAttack);
        if (_currentCommand.scope != AbilityCommand.Scope.AOEEnemies
            && _currentCommand.scope != AbilityCommand.Scope.AOEAllies 
            && _potentialTargetTiles.Count > 0)
        {
            _primaryTargetTile =
                _potentialTargetTiles[
                    (_potentialTargetTiles.Count + _potentialTargetTiles.IndexOf(_primaryTargetTile) + indexDelta) %
                    _potentialTargetTiles.Count];
            Gm.tileManager.AddFlag(_primaryTargetTile, Tile.Flag.SelectedForAttack);
        }

        foreach (var tile in _potentialTargetTiles.Where(tile => tile != _primaryTargetTile))
        {
            Gm.tileManager.AddFlag(tile, Tile.Flag.HighlightedForAttack);
        }
    }

    private List<GridObject> DetermineTargets()
    {
        List<GridObject> result = new List<GridObject>();
        if (_currentCommand.scope == AbilityCommand.Scope.SingleEnemy)
        {
            result.Add(_primaryTargetTile.Occupant);
        }
        else if (_currentCommand.scope == AbilityCommand.Scope.AOEEnemies)
        {
            foreach (Tile tile in _potentialTargetTiles.Where(tile => tile.Occupant != null))
            {
                result.Add(tile.Occupant);
            }
        }

        return result;
    }
}
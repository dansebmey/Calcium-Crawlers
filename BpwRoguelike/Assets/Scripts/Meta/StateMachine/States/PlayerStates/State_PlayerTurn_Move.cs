using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class State_PlayerTurn_Move : State_PlayerTurn
{
    public override void OnEnter()
    {
        base.OnEnter();
        
        Gm.tileManager.RemoveFlags();
        foreach (Tile tile in CurrentActor.traversibleTiles)
        {
            Gm.tileManager.AddFlag(tile, Tile.Flag.HighlightedForMovement);
        }
        HighlightTargetsWithinRange();

        InputController.BindActionToKey(Button.MoveLeft, null, MovePlayerLeft);
        InputController.BindActionToKey(Button.MoveRight, null, MovePlayerRight);
        InputController.BindActionToKey(Button.MoveUp, null, MovePlayerUp);
        InputController.BindActionToKey(Button.MoveDown, null, MovePlayerDown);
        InputController.BindActionToKey(Button.Interact, null, SwitchToAbilitySelectionState);
        InputController.BindActionToKey(Button.Back, null, Gm.combatManager.EndTurn);
        // InputController.BindActionToKey(Button.ShowInventory, null, SwitchToInventoryState);
        // InputController.BindActionToKey(Button.Interact, null, InteractWithGridObject);
        
        Gm.hudManager.inputInfoInterface.ShowCommands(new Dictionary<string, string>
        {
            {"WASD", "Move around"},
            {"SPACE", "Open command panel"},
            {"E", "End turn early"}
        });
    }

    private void SwitchToInventoryState()
    {
        Gm.SwitchState(typeof(State_PlayerTurn_Inventory));
    }

    private void SwitchToAbilitySelectionState()
    {
        Gm.SwitchState(typeof(State_PlayerTurn_PickAbility));
    }

    private bool PlayerCanMoveTo(Tile tile)
    {
        return CurrentActor.traversibleTiles.Contains(tile);
        return TileManager.DistanceBetween(CurrentActor.StartingPoint, tile.point) <= CurrentActor.MovRange
            && tile.IsTrespassable();
    }
    
    private void MovePlayerLeft()
    {
        if (CurrentActor.Tile.point.x == 0) return;
        
        Tile targetTile = Gm.tileManager.dungeonGen.grid[CurrentActor.Tile.point.x - 1, CurrentActor.Tile.point.y];
        
        CurrentActor.LookAt(targetTile.point);
        if (PlayerCanMoveTo(targetTile))
        {
            CurrentActor.MoveTo(targetTile);
            HighlightTargetsWithinRange();
        }
    }

    private void MovePlayerRight()
    {
        if (CurrentActor.Tile.point.x == Gm.tileManager.dungeonGen.grid.GetLength(0)) return;
        
        Tile targetTile = Gm.tileManager.dungeonGen.grid[CurrentActor.Tile.point.x + 1, CurrentActor.Tile.point.y];
        
        CurrentActor.LookAt(targetTile.point);
        if (PlayerCanMoveTo(targetTile))
        {
            CurrentActor.MoveTo(targetTile);
            HighlightTargetsWithinRange();
        }
    }

    private void MovePlayerUp()
    {
        if (CurrentActor.Tile.point.y == 0) return;
        
        Tile targetTile = Gm.tileManager.dungeonGen.grid[CurrentActor.Tile.point.x, CurrentActor.Tile.point.y + 1];
        
        CurrentActor.LookAt(targetTile.point);
        if (PlayerCanMoveTo(targetTile))
        {
            CurrentActor.MoveTo(targetTile);
            HighlightTargetsWithinRange();
        }
    }

    private void MovePlayerDown()
    {
        if (CurrentActor.Tile.point.y == Gm.tileManager.dungeonGen.grid.GetLength(1)-1) return;
        
        Tile targetTile = Gm.tileManager.dungeonGen.grid[CurrentActor.Tile.point.x, CurrentActor.Tile.point.y - 1];
        
        CurrentActor.LookAt(targetTile.point);
        if (PlayerCanMoveTo(targetTile))
        {
            CurrentActor.MoveTo(targetTile);
            HighlightTargetsWithinRange();
        }
    }

    private void HighlightTargetsWithinRange()
    {
        Gm.tileManager.RemoveFlags(Tile.Flag.HighlightedForAttack, Tile.Flag.SelectedForAttack);
        foreach (GridObject gridObject in GetTargetsWithinRange(CurrentActor.Tile))
        {
            gridObject.Tile.AddFlag(Tile.Flag.HighlightedForAttack);
        }
    }

    private void InteractWithGridObject()
    {
        
    }

    private List<GridObject> GetTargetsWithinRange(Tile originTile)
    {
        List<GridObject> result = new List<GridObject>();
        foreach (GridObject gridObject in Gm.combatManager.gridObjects)
        {
            if (gridObject != CurrentActor && CurrentActor.HasAbilityThatCanReach(originTile.point, gridObject.point))
            {
                result.Add(gridObject);
            }
        }

        return result;
    }
}
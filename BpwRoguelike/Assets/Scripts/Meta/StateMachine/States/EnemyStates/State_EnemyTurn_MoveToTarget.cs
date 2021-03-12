using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

public class State_EnemyTurn_MoveToTarget : State_EnemyTurn
{
    private bool _hasAttacked;

    public override void OnEnter()
    {
        base.OnEnter();
        
        Gm.inputController.BindActionToKey(Button.EndTurn, null, Gm.combatManager.EndTurn);

        if (CurrentActor.HasStatusEffect(typeof(Stun)))
        {
            Gm.combatManager.EndTurn();
            return;
        }
        
        // DetermineAction();
        _hasAttacked = false;
        MoveTowardsTarget(PickNewTarget());
        Gm.delayBetweenNPCStateSwitches = 0.6f;
    }

    private void MoveTowardsTarget(Combatant target)
    {
        List<Tile> finalPath = CurrentActor.pathfinder.FindPath(CurrentActor.point, CurrentActor.pathfinder.TileClosestToTarget(CurrentActor.Tile, target.Tile));

        if (finalPath == null)
        {
            Combatant newTarget = PickNewTarget();
            if (newTarget != target)
            {
                MoveTowardsTarget(target);
            } // this check should go once PickNewTarget() has been implemented properly

            TryToAttackTarget(target);
        }
        else
        {
            int stepsToTake = Mathf.Min(finalPath.Count, CurrentActor.MovRange);
            // CurrentActor.Tile.AddFlag(Tile.Flag.PathFindingTest2);

            if (stepsToTake == 0)
            {
                if (TryToAttackTarget(target)) return;
            }
            else
            {
                List<Tile> route = finalPath.GetRange(0, stepsToTake);
                foreach (Tile tile in route)
                {
                    MoveCombatantTo(tile);
                    if (TryToAttackTarget(target)) return;
                }
            }
        }

        if (!_hasAttacked)
        {
            Gm.combatManager.EndTurn();
        }
    }

    private bool TryToAttackTarget(Combatant target)
    {
        if (!_hasAttacked && CurrentActor.HasAbilityThatCanReach(CurrentActor.point, target.point))
        {
            _hasAttacked = true;
            Gm.combatManager.EnqueueCommand(new AbilityDetails(CurrentActor.GetCurrentlyPerformableAbilities()[0], CurrentActor, new List<GridObject>{target}));
            Gm.SwitchState(typeof(State_EnemyTurn_Performance), 0.5f);
            
            return true;
        }

        return false;
    }

    private Combatant PickNewTarget()
    {
        List<Combatant> potentialTargets = Gm.combatManager.combatants.Where(cbt => cbt is PCombatant).ToList();
        Combatant closestCombatant = null;
        int lowestDistance = 999;
        foreach (Combatant cbt in potentialTargets)
        {
            int distance = TileManager.DistanceBetween(cbt.Point, CurrentActor.Point);
            if (distance < lowestDistance)
            {
                closestCombatant = cbt;
                lowestDistance = distance;
            }
        }

        return closestCombatant;
        
        return Gm.combatManager.combatants.First(combatant => combatant is Combatant c && c is PCombatant);
    }

    // private void DetermineAction()
    // {
    //     Dictionary<Combatant, Tile> potentialTargets = new Dictionary<Combatant, Tile>();
    //     Tile targetTile = null;
    //
    //     List<Tile> tilesWithinRange = Gm.tileManager.GetAllTilesWithinRangeOf(CurrentActor.point, CurrentActor.MovRange);
    //     foreach (Tile tile in tilesWithinRange)
    //     {
    //         List<Combatant> targetsWithinRange = Gm.tileManager.GetAllTargetsWithinAttackRangeOf(CurrentActor, tile.point)
    //             .Where(target => target != CurrentActor && target is PCombatant).ToList();
    //         if (targetsWithinRange.Count > 0)
    //         {
    //             foreach (Combatant target in targetsWithinRange)
    //             {
    //                 if (!targetsWithinRange.Contains(target))
    //                 {
    //                     potentialTargets.Add(target, tile);   
    //                 }
    //             }
    //         }
    //     }
    //     
    //     Combatant lowestHealthCombatant = null;
    //     if (potentialTargets.Count > 0)
    //     {
    //         foreach (Combatant potentialTarget in potentialTargets.Keys)
    //         {
    //             if (lowestHealthCombatant == null || potentialTarget.Hitpoints < lowestHealthCombatant.Hitpoints)
    //             {
    //                 lowestHealthCombatant = potentialTarget;
    //             }
    //         }
    //
    //         Debug.Assert(lowestHealthCombatant != null, nameof(lowestHealthCombatant) + " != null");
    //         targetTile = potentialTargets[lowestHealthCombatant];
    //         MoveCombatantTo(targetTile);
    //         // AttackTarget(lowestHealthCombatant);
    //     }
    //     else
    //     {
    //         foreach (GridObject gridObject in Gm.combatManager.gridObjects.Where(gridObject => gridObject is Combatant && gridObject != CurrentActor))
    //         {
    //             var combatant = (Combatant) gridObject;
    //             if (lowestHealthCombatant == null || combatant.Hitpoints < lowestHealthCombatant.Hitpoints)
    //             {
    //                 targetTile = combatant.Tile;
    //                 lowestHealthCombatant = combatant;
    //             }
    //         }
    //
    //         targetTile = TileClosestTo(lowestHealthCombatant, tilesWithinRange);
    //         MoveCombatantTo(targetTile);
    //     }
    //     
    //     Gm.combatManager.EndTurn();
    // }

    private void MoveCombatantTo(Tile targetTile)
    {
        CurrentActor.LookAt(targetTile.point);
        CurrentActor.MoveTo(targetTile);
    }

    private Tile TileClosestTo(Combatant targetCombatant, List<Tile> tilesWithinRange)
    {
        Tile closestTile = null;
        int lowestDistance = 99;
        
        foreach (Tile tile in tilesWithinRange)
        {
            if (closestTile == null || TileManager.DistanceBetween(tile.point, targetCombatant.point) < lowestDistance)
            {
                closestTile = tile;
                lowestDistance = TileManager.DistanceBetween(tile.point, targetCombatant.point);
            }
        }

        return closestTile;
    }
}
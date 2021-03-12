using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

[RequireComponent(typeof(DungeonGenerator))]
public class TileManager : Manager
{
    [HideInInspector] public DungeonGenerator dungeonGen;

    protected override void Awake()
    {
        base.Awake();
        dungeonGen = GetComponent<DungeonGenerator>();
        
        EventManager<GridObject>.AddListener(EventType.RegisterGridObject, RegisterGridObject);
    }

    public void RegisterGridObject(GridObject gridObject)
    {
        Tile[,] grid = dungeonGen.grid;
        Vector2Int assignedPos;
        do
        {
            assignedPos = new Vector2Int(Random.Range(1, dungeonGen.dungeonWidth),
                Random.Range(1, dungeonGen.dungeonHeight));
        } while (!grid[assignedPos.x, assignedPos.y].IsTrespassable());
        // TODO: See if this can do without a while-loop
        
        Tile tile = grid[assignedPos.x, assignedPos.y];
        tile.Occupant = gridObject;
        gridObject.Point = tile.point;
    }

    public static int DistanceBetween(GridPos gridPosA, GridPos gridPosB, bool diagonal = false)
    {
        if (diagonal)
        {
            return Mathf.Max(Mathf.Abs(gridPosA.x - gridPosB.x), Mathf.Abs(gridPosA.y - gridPosB.y));
        }
            
        return Mathf.Abs(gridPosA.x - gridPosB.x) + Mathf.Abs(gridPosA.y - gridPosB.y);
    }

    public void TransferOccupant(GridObject gridObject, Tile targetTile)
    {
        gridObject.Tile.Occupant = null;
        targetTile.Occupant = gridObject;
    }

    public List<Tile> GetTilesWithinMovementRange(Combatant combatant)
    {
        List<Tile> tilesToScanWithPathfinder = new List<Tile>();
        foreach (Tile tile in dungeonGen.grid)
        {
            if (DistanceBetween(combatant.Point, tile.point) <= combatant.MovRange
                && (tile.IsTrespassable() || tile.Occupant == combatant))
            {
                tilesToScanWithPathfinder.Add(tile);
            }
        }
        
        List<Tile> result = new List<Tile>();
        if (tilesToScanWithPathfinder.Count > 0)
        {
            foreach (Tile tile in tilesToScanWithPathfinder)
            {
                if (combatant == null || combatant.pathfinder == null || tile == null)
                {
                    Debug.Log("stop here");
                }
                
                List<Tile> pathToTile = combatant.pathfinder.FindPath(combatant.point, tile.point);
                if (pathToTile != null && pathToTile.Count <= combatant.MovRange)
                {
                    result.Add(tile);
                }
            }
        }

        return result;
    }

    public void HighlightAttackRange(AbilityCommand command)
    {
        gm.tileManager.RemoveFlags(Tile.Flag.HighlightedForAttack);
        foreach (Tile tile in command.GetTilesWithinReach(dungeonGen.grid, gm.combatManager.CurrentActor.Tile))
        {
            tile.AddFlag(Tile.Flag.HighlightedForAttack);
        }
    }

    public List<Tile> GetAllTilesWithinRangeOf(GridPos origin, int range)
    {
        List<Tile> result = new List<Tile>();
        foreach (Tile tile in dungeonGen.grid)
        {
            if (DistanceBetween(origin, tile.point) <= range)
            {
                result.Add(tile);
            }
        }

        return result;
    }

    public List<GridObject> GetAllTargetsWithinAttackRangeOf(Combatant attacker, GridPos point)
    {
        List<GridObject> result = new List<GridObject>();
        foreach (AbilityCommand ability in attacker.GetCurrentlyPerformableAbilities())
        {
            foreach (GridObject target in gm.combatManager.gridObjects)
            {
                if (ability.CanReach(point, target.point, attacker.loadout.weapon))
                {
                    if (attacker is PCombatant || attacker is NPCombatant npc && npc.WantsToUseAbility(ability))
                    {
                        result.Add(target);
                    }
                }
            }
        }

        return result;
    }

    public void AddFlag(Tile tile, Tile.Flag flag, bool firstRemoveExistingFlags = false)
    {
        if (firstRemoveExistingFlags)
        {
            foreach (Tile t in dungeonGen.grid)
            {
                t.RemoveFlag(flag);
            }
        }
        
        tile.AddFlag(flag);
    }

    public void RemoveFlags(params Tile.Flag[] flags)
    {
        foreach (Tile t in dungeonGen.grid)
        {
            t.RemoveFlags(flags);
        }
    }

    public Tile GetRandomUnoccupiedTile()
    {
        List<Tile> candidates = new List<Tile>();
        foreach (Tile tile in dungeonGen.grid)
        {
            if (tile.IsTrespassable())
            {
                candidates.Add(tile);
            }
        }

        return candidates[Random.Range(0, candidates.Count - 1)];
    }
}
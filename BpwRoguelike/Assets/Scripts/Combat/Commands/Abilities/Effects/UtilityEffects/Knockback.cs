using System;
using UnityEngine;

[CreateAssetMenu(menuName = "Ability Effects/Utility/Knockback")]
public class Knockback : UtilityEffect
{
    [Header("Knockback")]
    public int distance;

    protected override void OnApplyEffect(Combatant primaryTarget, AbilityDetails details)
    {
        if (details.command.diagonal)
        {
            Debug.LogWarning("Knockback cannot be applied on abilities with diagonal reach");
            return;
        }
        
        GridPos performerPos = details.performer.Point;

        foreach (GridObject target in details.targets)
        {
            int xDelta = 0;
            int yDelta = 0;
            
            GridPos defPos = target.Point;
            if (defPos.x < performerPos.x)
            {
                xDelta = -distance;
            }
            else if (defPos.x > performerPos.x)
            {
                xDelta = +distance;
            }
            else if (defPos.y < performerPos.y)
            {
                yDelta = -distance;
            }
            else if (defPos.y > performerPos.y)
            {
                yDelta = +distance;
            }

            KnockBack(target, defPos, xDelta, yDelta);
        }
    }

    private void KnockBack(GridObject def, GridPos defPos, int xDelta, int yDelta)
    {
        Tile targetTile = FindObjectOfType<DungeonGenerator>().grid[defPos.x + xDelta, defPos.y + yDelta]; // VERY dirty fix. Ew!!
        if (targetTile.Occupant != null)
        {
            KnockBack(targetTile.Occupant, targetTile.Occupant.Point, xDelta, yDelta);
        }
        if (targetTile.IsTrespassable() || targetTile.Occupant != null)
        {
            def.MoveTo(targetTile);
        }
        // TODO: Combatants can currently stack on top of each other when they are pushed against a wall
    }
}
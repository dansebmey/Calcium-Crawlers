using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class Tile : GmAwareObject
{
    [SerializeField] private bool isTrespassable;
    
    public GridObject Occupant { get; set; }
    public GridPos point;
    
    private SpriteRenderer _highlightSprite;
    
    public enum Flag { Dormant, HighlightedForMovement, HighlightedForAttack, SelectedForAttack,
        PathFindingTest1, PathFindingTest2
    }
    private List<Flag> _flags = new List<Flag>();
    private Dictionary<Flag, Color> _flagColours;
    
    protected override void Awake()
    {
        base.Awake();
        
        _highlightSprite = GetComponentsInChildren<SpriteRenderer>(true)[1];
    }

    protected override void Start()
    {
        base.Start();

        _flagColours = new Dictionary<Flag, Color>
        {
            [Flag.Dormant] = new Color(1, 1, 1, 0),
            [Flag.HighlightedForMovement] = new Color(0.4f, 1, 0.4f),
            [Flag.HighlightedForAttack] = new Color(1, 0.4f, 0.4f),
            [Flag.SelectedForAttack] = new Color(1, 0, 0, 1),
            [Flag.PathFindingTest1] = new Color(0, 1, 1, 0.4f),
            [Flag.PathFindingTest2] = new Color(0.75f, 0, 1, 1)
        };
    }

    public void AssignCoords(int x, int y)
    {
        point = new GridPos(x, y);
    }

    public virtual bool IsTrespassable()
    {
        return isTrespassable && (Occupant == null || Occupant is Combatant c && c.state == Combatant.State.Fallen);
    }

    public void AddFlag(Flag flag)
    {
        _flags.Add(flag);
        UpdateHighlight();
    }
    
    public void RemoveFlag(Flag flag)
    {
        _flags.Remove(flag);
        UpdateHighlight();
    }

    public void RemoveFlags(params Flag[] flags)
    {
        if (flags.Length > 0)
        {
            foreach (Flag flag in flags)
            {
                _flags.Remove(flag);
            }
        }
        else
        {
            _flags.Clear();
        }
        UpdateHighlight();
    }

    private void UpdateHighlight()
    {
        Flag highestPriorityFlag = Flag.Dormant;
        foreach (Flag f in _flags)
        {
            if ((int) f > (int) highestPriorityFlag)
            {
                highestPriorityFlag = f;
            }
        }

        _highlightSprite.color = _flagColours[highestPriorityFlag];
    }
}
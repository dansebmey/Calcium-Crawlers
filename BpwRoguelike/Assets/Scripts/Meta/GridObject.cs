using System;
using System.Linq;
using UnityEngine;

public abstract class GridObject : GmAwareObject, IDamageable
{
    public GridPos point;
    [HideInInspector] public OverheadHUD overheadHud;
    private Vector3 _targetPos;
    
    private SpriteRenderer _minimapMask;

    public GridPos Point
    {
        get => point;
        set
        {
            point = value;
            _targetPos = GameManager.Instance.tileManager.dungeonGen.grid[value.x, value.y].transform.position;  // point.TxPos;
        }
    }

    public Tile Tile => gm.tileManager.dungeonGen.grid[point.x, point.y];

    protected override void Awake()
    {
        base.Awake();
        overheadHud = GetComponentInChildren<OverheadHUD>();
        _minimapMask = GetComponentsInChildren<SpriteRenderer>().Last();
    }

    protected override void Start()
    {
        base.Start();
        EventManager<GridObject>.Invoke(EventType.RegisterGridObject, this);
    }

    private void Update()
    {
        if (transform.position != _targetPos);
        {
            transform.position = Vector3.Lerp(transform.position, _targetPos, 0.25f);
        }
    }

    // public abstract void OnClick();
    // public abstract void OnHover();

    public bool IsWithinRangeOf(GridObject target, int ixRange)
    {
        return TileManager.DistanceBetween(point, target.point) <= ixRange;
    }

    public void ShowAction(bool show)
    {
        overheadHud.ShowText(show);
    }

    public abstract void TakeDamageFromAttack(AbilityDetails abilityDetails);

    public void MoveTo(Tile targetTile)
    {
        MoveTo(targetTile.point);
    }

    private void MoveTo(GridPos targetPointOnGrid)
    {
        gm.tileManager.TransferOccupant(this, gm.tileManager.dungeonGen.grid[targetPointOnGrid.x, targetPointOnGrid.y]);
        Point = targetPointOnGrid;
    }

    protected virtual void OnDisable()
    {
        gm.combatManager.RemoveGridObject(this);
        _minimapMask.gameObject.SetActive(false);
    }
}
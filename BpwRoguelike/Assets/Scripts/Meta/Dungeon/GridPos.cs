using UnityEngine;

public class GridPos
{
    public int x, y;

    public GridPos(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public Vector2 TxPos => new Vector2(-8.5f + x, 4.5f - y);
}
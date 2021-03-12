using System;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Vector2Int pos;
    public Vector2Int size;
    
    private int _maxAmtOfEnemies;

    private Dictionary<int, Vector2Int> doorDictionary;

    public const int DOOR_UPPER = 0;
    public const int DOOR_LEFT = 1;
    public const int DOOR_RIGHT = 2;
    public const int DOOR_LOWER = 3;

    public Room()
    {
        _maxAmtOfEnemies = (int)Mathf.Sqrt(size.x * size.y);
        doorDictionary = new Dictionary<int, Vector2Int>();
    }

    public Vector2Int ClosestDoorPos(Vector2Int fromPos)
    {
        int closestDoorIndex = DOOR_UPPER;
        foreach (KeyValuePair<int, Vector2Int> kv in doorDictionary)
        {
            if (TileManager.DistanceBetween(new GridPos(fromPos.x, fromPos.y), new GridPos(kv.Value.x, kv.Value.y)) <
                closestDoorIndex)
            {
                closestDoorIndex = kv.Key;
            }
        }

        return doorDictionary[closestDoorIndex];
    }

    public void RegisterDoor(int doorIndex, int x, int y)
    {
        doorDictionary.Add(doorIndex, new Vector2Int(x, y));
    }
}
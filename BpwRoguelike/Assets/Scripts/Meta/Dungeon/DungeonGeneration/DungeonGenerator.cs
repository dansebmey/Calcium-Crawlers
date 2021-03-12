using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class DungeonGenerator : GmAwareObject
{
    public Tile[,] grid;
    
    [Header("Prefabs")]
    public GroundTile groundTilePrefab;
    public WallTile wallTilePrefab;
    
    [Header("Generation data")]
    public int amtOfRooms = 3;
    public int dungeonWidth = 48;
    public int dungeonHeight = 48;
    public int minRoomSize = 7;
    public int maxRoomSize = 15;
    
    private readonly Dictionary<Vector2Int, Tile> _dungeonDictionary = new Dictionary<Vector2Int, Tile>();
    private List<Room> _rooms =  new List<Room>();

    [Header("Spawnables")]
    public List<GridObjectSpawnData> spawnablePrefabs;

    [Serializable]
    public class GridObjectSpawnData
    {
        public GridObject gridObject;
        [Range(1,100)] public int spawnWeight;
        [Range(1,25)] public int pointCost;
    }

    protected override void Awake()
    {
        base.Awake();
        
        GenerateDungeon();
    }

    private void GenerateDungeon()
    {
        AllocateRooms();
        AllocateCorridors();
        FillEmptySpace();

        BuildDungeon();

        SpawnObjects();
    }

    private void SpawnObjects()
    {
        int amountOfGroundTiles = grid.OfType<GroundTile>().Count();
        int remainingPointBudget = Mathf.RoundToInt(Mathf.Sqrt(amountOfGroundTiles));
        Debug.Log("Total budget: "+remainingPointBudget);

        for (int i = remainingPointBudget; i > 0;)
        {
            GridObject objectToSpawn = DetermineWhichObjectToSpawn(remainingPointBudget, out remainingPointBudget);
            if (objectToSpawn != null)
            {
                Instantiate(objectToSpawn, Vector3.zero, Quaternion.identity);
            }
            i = remainingPointBudget;
        }
    }

    private GridObject DetermineWhichObjectToSpawn(int remainingPoints, out int remainingPointsOut)
    {
        List<GridObjectSpawnData> eligibleObjects = spawnablePrefabs.Where(spawnData => spawnData.pointCost <= remainingPoints).ToList();
        // if (eligibleObjects.Count == 0)
        // {
        //     return;
        // }

        int collectiveSpawnWeight = eligibleObjects.Sum(eData => eData.spawnWeight);
        int randomNo = Random.Range(0, collectiveSpawnWeight);
        eligibleObjects.Sort((d1, d2) => d1.spawnWeight.CompareTo(d2.spawnWeight));

        int cumulativeSpawnWeight = 0;
        foreach (GridObjectSpawnData spawnData in eligibleObjects)
        {
            cumulativeSpawnWeight += spawnData.spawnWeight;
            if (cumulativeSpawnWeight >= randomNo)
            {
                remainingPointsOut = remainingPoints - spawnData.pointCost;
                return spawnData.gridObject;
            }
        }
        
        remainingPointsOut = 0;
        return null;
    }

    private void AllocateRooms()
    {
        int failsafeCounter = 0;
        for (int i = 0; i < amtOfRooms; i++)
        {
            Room room = new Room()
            {
                pos = new Vector2Int(Random.Range(1, dungeonWidth-1), Random.Range(1, dungeonHeight-1)),
                size = new Vector2Int(Random.Range(minRoomSize, maxRoomSize), Random.Range(minRoomSize, maxRoomSize))
            };
            
            if (CheckIfRoomFitsInDungeon(room))
            {
                AddRoomToDungeon(room);
            }
            else if (failsafeCounter < 10)
            {
                failsafeCounter++;
                i--;
            }
        }
    }

    private bool CheckIfRoomFitsInDungeon(Room room)
    {
        for (int iX = room.pos.x; iX < room.pos.x + room.size.x; iX++)
        {
            for (int iY = room.pos.y; iY < room.pos.y + room.size.y; iY++)
            {
                if (iX >= 48 || iY >= 48)
                {
                    Debug.Log("check");
                }
                Vector2Int pos = new Vector2Int(iX, iY);
                if (room.pos.x + room.size.x >= dungeonWidth
                    || room.pos.y + room.size.y >= dungeonHeight
                    || _dungeonDictionary.ContainsKey(pos))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private void AddRoomToDungeon(Room room)
    {
        int doorIndex = 0;
        for (int iX = room.pos.x; iX < room.pos.x + room.size.x; iX++)
        {
            for (int iY = room.pos.y; iY < room.pos.y + room.size.y; iY++)
            {
                Vector2Int pos = new Vector2Int(iX, iY);

                Tile prefab = DetermineTilePrefab(room, iX, iY, out bool isDoorPos);
                _dungeonDictionary.Add(pos, prefab);

                if (isDoorPos)
                {
                    room.RegisterDoor(doorIndex, iX, iY);
                    doorIndex++;
                }
            }
        }
        
        _rooms.Add(room);
    }

    private Tile DetermineTilePrefab(Room room, int x, int y, out bool isDoorPos)
    {
        isDoorPos = false;
        if (x == room.pos.x || x == room.pos.x + room.size.x - 1 || y == room.pos.y || y == room.pos.y + room.size.y - 1)
        {
            if (((x == room.pos.x || x == room.pos.x + room.size.x - 1) && y == room.pos.y + Mathf.FloorToInt(room.size.y / 2.0f)) 
                || ((y == room.pos.y || y == room.pos.y + room.size.y - 1) && x == room.pos.x + Mathf.FloorToInt(room.size.x / 2.0f)))
            {
                isDoorPos = true;
                // return doorTilePrefab;
                return groundTilePrefab;
            }
            
            // return wallTilePrefab;
            return groundTilePrefab;
        }

        return groundTilePrefab;
    }

    private void AllocateCorridors()
    {
        for (int i = 0; i < _rooms.Count; i++)
        {
            Room startRoom = _rooms[i];
            Room otherRoom = _rooms[(i + Random.Range(1, _rooms.Count - 1)) % _rooms.Count];

            Vector2Int targetPos = otherRoom.ClosestDoorPos(startRoom.pos);
            int dirX = Mathf.RoundToInt(Mathf.Sign(targetPos.x - startRoom.pos.x));
            int dirY = Mathf.RoundToInt(Mathf.Sign(targetPos.y - startRoom.pos.y));

            int repetitions = 0;
            for(int x = startRoom.pos.x; x != targetPos.x; x += dirX)
            {
                Vector2Int pos = new Vector2Int(x, startRoom.pos.y);
                if (!_dungeonDictionary.ContainsKey(pos))
                {
                    _dungeonDictionary.Add(pos, groundTilePrefab);
                }

                repetitions++;
                if (repetitions == 1000)
                {
                    break;
                }
            }

            repetitions = 0;
            for (int y = startRoom.pos.y; y != targetPos.y; y += dirY)
            {
                Vector2Int pos = new Vector2Int(otherRoom.pos.x, y);
                if (!_dungeonDictionary.ContainsKey(pos))
                {
                    _dungeonDictionary.Add(pos, groundTilePrefab);
                }
                
                repetitions++;
                if (repetitions == 1000)
                {
                    break;
                }
            }

        }
    }

    private void FillEmptySpace()
    {
        for (int iX = 0; iX < dungeonWidth; iX++)
        {
            for (int iY = 0; iY < dungeonHeight; iY++)
            {
                Vector2Int pos = new Vector2Int(iX, iY);
                if (!_dungeonDictionary.ContainsKey(pos))
                {
                    _dungeonDictionary.Add(pos, wallTilePrefab);
                }  
            }
        }
    }
    
    private void BuildDungeon()
    {
        grid = new Tile[dungeonWidth,dungeonHeight];
        foreach(KeyValuePair<Vector2Int, Tile> kv in _dungeonDictionary)
        {
            Tile tile = Instantiate(kv.Value, new Vector3Int(kv.Key.x, kv.Key.y, 0), Quaternion.identity);
            if (kv.Key.x > grid.GetLength(0) || kv.Key.y > grid.GetLength(1))
            {
                Debug.Log("check");
            }
            grid[kv.Key.x, kv.Key.y] = tile;
            tile.point = new GridPos(kv.Key.x, kv.Key.y);
            
            tile.transform.SetParent(transform);
            // SpawnWallsForTile(kv.Key);
        }
    }
}
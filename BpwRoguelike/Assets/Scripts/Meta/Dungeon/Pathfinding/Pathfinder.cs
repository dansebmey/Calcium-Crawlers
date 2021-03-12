using System;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinder : MonoBehaviour
{
    private TileManager _tileManager;
    public Node[,] nodes;

    private void Awake()
    {
        _tileManager = FindObjectOfType<TileManager>();
        
        nodes = new Node[_tileManager.dungeonGen.dungeonWidth, _tileManager.dungeonGen.dungeonHeight];
        foreach (Tile tile in _tileManager.dungeonGen.grid)
        {
            nodes[tile.point.x, tile.point.y] = new Node(tile);
        }
    }

    public List<Tile> FindPath(GridPos startPos, GridPos targetPos)
    {
        // _tileManager.RemoveFlags(Tile.Flag.PathFindingTest1, Tile.Flag.PathFindingTest2);
        
        Node startNode = nodes[startPos.x, startPos.y];
        Node targetNode = nodes[targetPos.x, targetPos.y];
        
        List<Node> openList = new List<Node>();
        HashSet<Node> closedList = new HashSet<Node>();
        
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].fCost < currentNode.fCost || openList[i].fCost == currentNode.fCost
                    && openList[i].hCost < currentNode.hCost)
                {
                    currentNode = openList[i];
                }
                // openList[i].tile.AddFlag(Tile.Flag.PathFindingTest1);
            }

            openList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == targetNode)
            {
                return GetFinalPath(startNode, targetNode);
            }

            foreach (Node neighbourNode in GetNeighbouringNodes(currentNode))
            {
                if (
                    !neighbourNode.tile.IsTrespassable() ||
                    closedList.Contains(neighbourNode))
                {
                    continue;
                }

                int moveCost = currentNode.gCost +
                               TileManager.DistanceBetween(currentNode.tile.point, neighbourNode.tile.point);

                if (!openList.Contains(neighbourNode) || moveCost < neighbourNode.gCost)
                {
                    neighbourNode.gCost = moveCost;
                    neighbourNode.hCost = TileManager.DistanceBetween(neighbourNode.tile.point, targetNode.tile.point);
                    neighbourNode.parent = currentNode;

                    if (!openList.Contains(neighbourNode))
                    {
                        openList.Add(neighbourNode);
                    }
                }
            }
        }
        
        return null;
    }

    private List<Node> GetNeighbouringNodes(Node currentNode)
    {
        List<Node> result = new List<Node>();
        int xCheck;
        int yCheck;

        // right
        xCheck = currentNode.tile.point.x + 1;
        yCheck = currentNode.tile.point.y;
        if (xCheck >= 0 && xCheck < _tileManager.dungeonGen.dungeonWidth
                        && yCheck >= 0 && yCheck < _tileManager.dungeonGen.dungeonHeight)
        {
            result.Add(nodes[xCheck, yCheck]);
        }
        
        // left
        xCheck = currentNode.tile.point.x - 1;
        yCheck = currentNode.tile.point.y;
        if (xCheck >= 0 && xCheck < _tileManager.dungeonGen.dungeonWidth
                        && yCheck >= 0 && yCheck < _tileManager.dungeonGen.dungeonHeight)
        {
            result.Add(nodes[xCheck, yCheck]);
        }
        
        // top
        xCheck = currentNode.tile.point.x;
        yCheck = currentNode.tile.point.y + 1;
        if (xCheck >= 0 && xCheck < _tileManager.dungeonGen.dungeonWidth
                        && yCheck >= 0 && yCheck < _tileManager.dungeonGen.dungeonHeight)
        {
            result.Add(nodes[xCheck, yCheck]);
        }
        
        // bottom
        xCheck = currentNode.tile.point.x;
        yCheck = currentNode.tile.point.y - 1;
        if (xCheck >= 0 && xCheck < _tileManager.dungeonGen.dungeonWidth
                        && yCheck >= 0 && yCheck < _tileManager.dungeonGen.dungeonHeight)
        {
            result.Add(nodes[xCheck, yCheck]);
        }

        return result;
    }

    private List<Tile> GetFinalPath(Node startNode, Node targetNode)
    {
        List<Tile> finalPath = new List<Tile>();
        Node currentNode = targetNode;

        while (currentNode != startNode)
        {
            finalPath.Add(currentNode.tile);
            currentNode = currentNode.parent;
        }
        
        finalPath.Reverse();
        return finalPath;
    }

    public class Node
    {
        public Tile tile;
        public int hCost;
        public int gCost;
        public int fCost => hCost + gCost;

        public Node parent;

        public Node(Tile tile)
        {
            this.tile = tile;
        }
    }

    public GridPos TileClosestToTarget(Tile startTile, Tile targetTile)
    {
        List<Tile> adjacentTiles = new List<Tile>
        {
            _tileManager.dungeonGen.grid[targetTile.point.x, targetTile.point.y - 1],
            _tileManager.dungeonGen.grid[targetTile.point.x - 1, targetTile.point.y],
            _tileManager.dungeonGen.grid[targetTile.point.x + 1, targetTile.point.y],
            _tileManager.dungeonGen.grid[targetTile.point.x, targetTile.point.y + 1]
        }; // This approach might be prone to ArgumentOutOfRangeExceptions down the road 

        GridPos closestTilePos = adjacentTiles[0].point;
        int shortestDistance = TileManager.DistanceBetween(startTile.point, closestTilePos);
        for (int i = 1; i < adjacentTiles.Count; i++)
        {
            int distance = TileManager.DistanceBetween(startTile.point, adjacentTiles[i].point);
            if (distance < shortestDistance && adjacentTiles[i].IsTrespassable())
            {
                closestTilePos = adjacentTiles[i].point;
                shortestDistance = distance;
            }
        }

        return closestTilePos;
    }
}
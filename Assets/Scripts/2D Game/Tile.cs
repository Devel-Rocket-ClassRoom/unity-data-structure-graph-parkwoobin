using UnityEngine;

// 1010
public enum Sides
{
    None = -1,
    Top, // 3
    Left,  // 2
    Right,   // 1
    Bottom     // 0
}
public class Tile
{
    public int id;
    public Tile[] adjacents = new Tile[4];

    public int autoTileId;
    public int fowTileId;
    public bool isVisited = false;

    public bool CanMove => autoTileId != (int)TileTypes.Empty;  // 이동 가능한 타일은 Empty가 아닌 타일

    public int Weight
    {
        get
        {
            if (autoTileId == (int)TileTypes.Empty)
            {
                return int.MaxValue;
            }
            if (autoTileId == (int)TileTypes.Grass)
            {
                return 1;
            }
            if (autoTileId == (int)TileTypes.Tree)
            {
                return 2;
            }
            if (autoTileId == (int)TileTypes.Hills)
            {
                return 4;
            }
            if (autoTileId == (int)TileTypes.Mountains)
            {
                return int.MaxValue;
            }
            if (autoTileId == (int)TileTypes.Towns || autoTileId == (int)TileTypes.Castle || autoTileId == (int)TileTypes.Monsters)
            {
                return 1;
            }
            return 1;
        }
    }

    public void UpdateAutoTileId()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null && adjacents[i].autoTileId != (int)TileTypes.Empty)
            {
                autoTileId |= (1 << i);
            }
        }
    }
    public void UpdateFowTileId()
    {
        fowTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null || !adjacents[i].isVisited)
            {
                fowTileId |= (1 << i);
            }

        }
    }

    public void RemoveAdjacents(Tile tile)
    {
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
            {
                continue;
            }
            if (adjacents[i].id == tile.id)
            {
                adjacents[i] = null;
                UpdateAutoTileId();
                // UpdateFowTileId();
                break;
            }
        }
    }

    public void ClearAdjacents()
    {
        // autoTileId = (int)TileTypes.Empty;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] == null)
            {
                continue;
            }
            adjacents[i].RemoveAdjacents(this);
            adjacents[i] = null;

            // adjacents[i].UpdateFowTileId();
        }
    }
}

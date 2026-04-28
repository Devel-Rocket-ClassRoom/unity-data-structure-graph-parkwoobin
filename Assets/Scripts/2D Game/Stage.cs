using UnityEngine;

public class Stage : MonoBehaviour
{
    public GameObject tilePrefabs;
    private GameObject[] tileObjs;

    public int mapWidth = 20;
    public int mapHeight = 20;

    [Range(0f, 0.9f)]
    public float erodePercent = 0.5f;
    [Range(0f, 0.9f)]
    public float lakePercent = 0.1f;
    [Range(0f, 0.9f)]
    public float treePercent = 0.1f;
    [Range(0f, 0.9f)]
    public float hillPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float mountainPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float townPercent = 0.1f;
    [Range(0f, 0.9f)]
    public float monsterPercent = 0.1f;

    public int erodeIterations = 2;

    public Vector2 tileSize = new Vector2(16, 16);

    public Sprite[] islandSprites;

    private Map map;

    public Map Map => map;

    private int prevTileId = -1;
    public int PrevTileId => prevTileId;

    public PlayerMovement playerPrefab;
    private PlayerMovement player;

    public int FowRadius = 3;   // 시야 반경
    public Sprite[] fowSprites; // 시야에 따른 타일 스프라이트 배열


    public Vector3 FirstTilePos
    {
        get
        {
            var pos = transform.position;
            pos.x -= (mapWidth * tileSize.x * 0.5f);
            pos.y += (mapHeight * tileSize.y * 0.5f);
            pos.x += tileSize.x * 0.5f;
            pos.y -= tileSize.y * 0.5f;
            return pos;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ResetStage();
        }

        if (tileObjs != null)
        {
            int currentTileId = ScreenPosToTileId(Input.mousePosition);
            if (prevTileId != currentTileId)
            {
                tileObjs[currentTileId].GetComponent<SpriteRenderer>().color = Color.green;
                if (prevTileId >= 0 && prevTileId < tileObjs.Length)
                {
                    tileObjs[prevTileId].GetComponent<SpriteRenderer>().color = Color.white;
                }
                prevTileId = currentTileId;
            }
        }
    }

    private void ResetStage()
    {
        map = new Map();
        map.Init(mapHeight, mapWidth);
        map.CreateIsland(erodePercent, erodeIterations, lakePercent, treePercent, hillPercent, mountainPercent, townPercent, monsterPercent);
        CreateGrid();
        CreatePlayer();
    }

    private void CreatePlayer()
    {
        if (player != null)
        {
            Destroy(player.gameObject);
        }
        player = Instantiate(playerPrefab);
        player.MoveTo(map.startTile.id);
        UpdateVisibility(map.startTile.id);
    }

    private void CreateGrid()
    {
        if (tileObjs != null)
        {
            foreach (var tile in tileObjs)
            {
                Destroy(tile.gameObject);
            }
        }
        for (int i = 0; i < map.tiles.Length; i++)  // 모든 타일의 isVisited를 false로 초기화
        {
            map.tiles[i].isVisited = false;
        }

        tileObjs = new GameObject[mapWidth * mapHeight];

        var position = FirstTilePos;

        for (int i = 0; i < mapHeight; i++)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                int tileId = i * mapWidth + j;
                var newGo = Instantiate(tilePrefabs, transform);
                newGo.transform.position = position;
                position.x += tileSize.x;

                tileObjs[tileId] = newGo;
                DecorateTile(tileId);
            }
            position.x = FirstTilePos.x;
            position.y -= tileSize.y;
        }
    }

    public void DecorateTile(int tileId)
    {
        var tile = map.tiles[tileId];
        var tileGo = tileObjs[tileId];
        var ren = tileGo.GetComponent<SpriteRenderer>();
        if (tile.autoTileId != (int)TileTypes.Empty)
        {
            if (tile.isVisited)
            {
                ren.sprite = islandSprites[tile.autoTileId];

            }
            else
            {
                tile.UpdateFowTileId();
                if (fowSprites != null && tile.fowTileId >= 0 && tile.fowTileId < fowSprites.Length)
                {
                    ren.sprite = fowSprites[tile.fowTileId];
                }
            }
        }
        else
        {
            if (fowSprites != null && fowSprites.Length > 0)
            {
                ren.sprite = fowSprites[15];
            }
            else
            {
                ren.sprite = null;
            }
        }
    }

    public int ScreenPosToTileId(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);

        var worldPos = Camera.main.ScreenToWorldPoint(screenPos);
        return WorldPosToTileId(worldPos);
    }

    public int WorldPosToTileId(Vector3 worldPos)
    {
        var first = FirstTilePos;

        int x = Mathf.FloorToInt((worldPos.x - first.x) / tileSize.x + 0.5f);
        int y = Mathf.FloorToInt((first.y - worldPos.y) / tileSize.y + 0.5f);

        x = Mathf.Clamp(x, 0, mapWidth - 1);
        y = Mathf.Clamp(y, 0, mapHeight - 1);

        return y * mapWidth + x;
    }

    public Vector3 GetTilePos(int y, int x)
    {
        return FirstTilePos + new Vector3(x * tileSize.x, -y * tileSize.y, 0);
    }


    public Vector3 GetTilePos(int tileId)
    {
        int y = tileId / mapWidth;
        int x = tileId % mapWidth;

        return GetTilePos(y, x);
    }

    public void UpdateVisibility(int centertileId)
    {
        if (map == null || map.tiles == null || centertileId < 0 || centertileId >= map.tiles.Length)
        {
            return;
        }

        int centerY = centertileId / mapWidth;
        int centerX = centertileId % mapWidth;
        int visibleRadius = Mathf.Max(0, FowRadius / 2);
        int refreshRadius = visibleRadius + 1;



        for (int y = Mathf.Max(0, centerY - visibleRadius); y <= Mathf.Min(mapHeight - 1, centerY + visibleRadius); y++)
        {
            for (int x = Mathf.Max(0, centerX - visibleRadius); x <= Mathf.Min(mapWidth - 1, centerX + visibleRadius); x++)
            {
                int tileId = y * mapWidth + x;
                map.tiles[tileId].isVisited = true;
            }
        }

        for (int y = Mathf.Max(0, centerY - refreshRadius); y <= Mathf.Min(mapHeight - 1, centerY + refreshRadius); y++)
        {
            for (int x = Mathf.Max(0, centerX - refreshRadius); x <= Mathf.Min(mapWidth - 1, centerX + refreshRadius); x++)
            {
                int tileId = y * mapWidth + x;
                DecorateTile(tileId);
            }
        }
    }
}
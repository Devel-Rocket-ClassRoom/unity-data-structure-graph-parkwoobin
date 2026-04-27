using UnityEngine;


// 0000
// 0001
// 0010
// 0011

// 1010
public enum Sides
{
    Bottom, // 3
    Right,  // 2
    Left,   // 1
    Top     // 0
}
public class Title
{
    public int id;
    public Title[] adjacents = new Title[4];

    public int autoTileId;
    public bool isVisited = false;

    public void UpdateAutoTileId()
    {
        autoTileId = 0;
        for (int i = 0; i < adjacents.Length; i++)
        {
            if (adjacents[i] != null)
            {
                autoTileId |= (1 << adjacents.Length - 1 - i);
            }
        }
    }

}

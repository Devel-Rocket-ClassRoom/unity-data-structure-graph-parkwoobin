using UnityEngine;

public enum TitleTypes
{
    Empty = -1,
    Grass = 15,
    Wall,
    Hills,
    Mountains,
    Castle,
    Monsters,
}
public class Map
{
    public int rows = 0;
    public int cols = 0;

    public Title[] titles;

    public void Init(int rows, int cols)
    {
        this.rows = rows;
        this.cols = cols;

        titles = new Title[rows * cols];
        for (int i = 0; i < titles.Length; i++)
        {
            titles[i] = new Title();
            titles[i].id = i;
        }

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                int index = r * cols + c;
                var adjacents = titles[index].adjacents;
                if ((r - 1) >= 0)
                {
                    adjacents[(int)Sides.Top] = titles[index - cols];  // up
                }
                if ((c + 1) < cols)
                {
                    adjacents[(int)Sides.Right] = titles[index + 1];    // right
                }
                if ((c - 1) >= 0)
                {
                    adjacents[(int)Sides.Left] = titles[index - 1];    // left
                }
                if ((r + 1) < rows)
                {
                    adjacents[(int)Sides.Bottom] = titles[index + cols];  // bottom
                }
            }
        }

        for (int i = 0; i < titles.Length; i++)
        {
            titles[i].UpdateAutoTileId();
        }
    }
}
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator animator;

    private int currentTileId;

    void Awake()
    {
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            animator.speed = 0f;
        }
        var findGo = GameObject.FindWithTag("Map");
        if (findGo != null)
        {
            stage = findGo.GetComponent<Stage>();
        }
    }

    private void Update()
    {
        var direction = Sides.None;
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            direction = Sides.Top;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            direction = Sides.Bottom;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            direction = Sides.Right;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            direction = Sides.Left;
        }
        if (direction != Sides.None)
        {
            var targetTile = stage.Map.tiles[currentTileId].adjacents[(int)direction];
            if (targetTile != null && targetTile.CanMove)
            {
                MoveTo(targetTile.id);
            }
        }

    }
    public void MoveTo(int tileId)
    {
        if (stage == null)
        {
            return;
        }

        currentTileId = tileId;
        transform.position = stage.GetTilePos(currentTileId);
        stage.UpdateVisibility(currentTileId);
    }
}

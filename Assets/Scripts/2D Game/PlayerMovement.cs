using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator animator;
    public int currentTileId = -1;
    private int targetTileId = -1;
    private Coroutine coMove;

    // 이동 상태

    public float moveSpeed = 50f;
    public bool isMoving = false;

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
        if (Input.GetKeyDown(KeyCode.W))
        {
            direction = Sides.Top;
        }
        if (Input.GetKeyDown(KeyCode.S))
        {
            direction = Sides.Bottom;
        }
        if (Input.GetKeyDown(KeyCode.D))
        {
            direction = Sides.Right;
        }
        if (Input.GetKeyDown(KeyCode.A))
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

    public void WarpTo(int tileId)
    {
        if (coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }
        isMoving = false;
        targetTileId = -1;

        currentTileId = tileId;
        transform.position = stage.GetTilePos(currentTileId);
        stage.OnTileVisited(currentTileId);
    }

    public void MoveTo(int tileId)
    {
        if (isMoving)
        {
            return;
        }
        targetTileId = tileId;
        if (coMove != null)
        {
            StopCoroutine(coMove);
            coMove = null;
        }
        coMove = StartCoroutine(CoMove());
    }

    private IEnumerator CoMove()
    {
        isMoving = true;

        var startPos = transform.position;
        var endPos = stage.GetTilePos(targetTileId);
        var duration = Vector3.Distance(startPos, endPos) / moveSpeed;

        var t = 0f;
        animator.speed = 1f;
        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            transform.position = Vector3.Lerp(startPos, endPos, t);
            yield return null;
        }

        transform.position = endPos;
        currentTileId = targetTileId;
        stage.OnTileVisited(currentTileId);
        isMoving = false;
        coMove = null;

        animator.speed = 0f;
    }
}

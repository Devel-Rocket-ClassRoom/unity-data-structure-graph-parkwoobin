using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Stage stage;
    private Animator animator;
    private int currentTileId;

    [SerializeField] private float moveDuration = 0.18f;

    // 이동 상태
    private bool isMoving;
    private int targetTileId = -1; // 현재 이동 대상
    private int queuedTileId = -1;  // 버퍼된 다음 이동
    private Vector3 moveStart;
    private Vector3 moveEnd;
    private float moveTimer;

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
        if (stage == null)
            return;
        if (isMoving)
        {
            var d = GetDirectionDown();
            if (d != Sides.None)
            {
                var tiles = stage.Map?.tiles;
                int baseId = (targetTileId != -1) ? targetTileId : currentTileId;
                if (tiles != null && baseId >= 0 && baseId < tiles.Length)
                {
                    var t = tiles[baseId].adjacents[(int)d];
                    if (t != null && t.CanMove) queuedTileId = t.id;
                }
            }

            moveTimer += Time.deltaTime;
            float p = Mathf.Clamp01(moveTimer / (moveDuration > 0f ? moveDuration : 0.0001f));
            transform.position = Vector3.Lerp(moveStart, moveEnd, Mathf.SmoothStep(0f, 1f, p));
            if (p >= 1f) FinishMove();
            return;
        }

        var dir = GetDirectionHold();
        if (dir == Sides.None) return;

        var tiles2 = stage.Map?.tiles;
        if (tiles2 != null && currentTileId >= 0 && currentTileId < tiles2.Length)
        {
            var target = tiles2[currentTileId].adjacents[(int)dir];
            if (target != null && target.CanMove) StartMove(target.id);
        }
    }

    private Sides GetDirectionDown()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) return Sides.Top;
        if (Input.GetKeyDown(KeyCode.DownArrow)) return Sides.Bottom;
        if (Input.GetKeyDown(KeyCode.RightArrow)) return Sides.Right;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) return Sides.Left;
        return Sides.None;
    }

    private Sides GetDirectionHold()
    {
        if (Input.GetKey(KeyCode.UpArrow)) return Sides.Top;
        if (Input.GetKey(KeyCode.DownArrow)) return Sides.Bottom;
        if (Input.GetKey(KeyCode.RightArrow)) return Sides.Right;
        if (Input.GetKey(KeyCode.LeftArrow)) return Sides.Left;
        return Sides.None;
    }

    public void MoveTo(int tileId) => StartMove(tileId);

    private void StartMove(int tileId)
    {
        if (stage == null) return;
        if (isMoving)
        {
            queuedTileId = tileId;
            return;
        }

        targetTileId = tileId;
        moveStart = transform.position;
        moveEnd = stage.GetTilePos(tileId);
        moveTimer = 0f;
        isMoving = true;
        if (animator != null) animator.speed = 1f;
    }

    private void FinishMove()
    {
        isMoving = false;
        if (targetTileId != -1) currentTileId = targetTileId;
        targetTileId = -1;
        stage.UpdateVisibility(currentTileId);
        if (animator != null) animator.speed = 0f;
        if (queuedTileId != -1)
        {
            int next = queuedTileId;
            queuedTileId = -1;
            StartMove(next);
        }
    }
}

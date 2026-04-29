using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TileSearch : MonoBehaviour
{
    private Stage stage;
    private PlayerMovement player;
    public List<Tile> path = new List<Tile>();
    private Coroutine pathFollowCoroutine;

    private void Awake()
    {
        var findGo = GameObject.FindWithTag("Map");
        stage = findGo.GetComponent<Stage>();
    }

    private void Start()
    {
        player = FindFirstObjectByType<PlayerMovement>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            HandleMouseClick();
        }
    }

    private void HandleMouseClick()
    {
        if (stage == null || player == null)
        {
            return;
        }

        if (player.isMoving)    // 이동 중 재클릭으로 경로 시작 타일이 꼬이는 문제를 방지
        {
            return;
        }

        int targetTileId = stage.ScreenPosToTileId(Input.mousePosition);    // 클릭한 위치의 타일 ID 가져오기
        Tile targetTile = stage.Map.tiles[targetTileId];    // 클릭한 위치의 타일 가져오기

        if (!targetTile.CanMove) // 클릭한 타일이 이동할 수 없는 경우
        {
            Debug.Log("이동할 수 없는 타일입니다.");
            return;
        }

        Tile startTile = stage.Map.tiles[player.currentTileId];   // 플레이어의 현재 타일 가져오기
        path = AStar(startTile, targetTile);    // A* 알고리즘으로 경로 찾기

        if (path.Count > 0) // 경로가 존재하는 경우
        {
            if (pathFollowCoroutine != null)
            {
                StopCoroutine(pathFollowCoroutine); // 이전에 실행 중인 경로 따라가기 코루틴이 있다면 중지
            }
            pathFollowCoroutine = StartCoroutine(FollowPath()); // 경로 따라가기 시작
        }
        else    // 경로가 존재하지 않는 경우 (예: 산으로 이동하려는 경우)
        {
            Debug.Log("산으로 이동할 수 없습니다");
        }
    }


    // -- A* 알고리즘 구현 -- //
    public List<Tile> AStar(Tile start, Tile end)
    {
        path.Clear();
        var visited = new HashSet<Tile>();
        var priorityQueue = new PriorityQueue<Tile, int>();
        var distances = new Dictionary<int, int>();
        var previous = new Dictionary<int, Tile>();

        foreach (var tile in stage.Map.tiles)
        {
            distances[tile.id] = int.MaxValue; // 초기 거리는 무한대로 설정
            previous[tile.id] = null;
        }
        distances[start.id] = 0;

        priorityQueue.Enqueue(start, distances[start.id]);

        while (priorityQueue.Count > 0)
        {
            var currentTile = priorityQueue.Dequeue();

            if (currentTile.id == end.id)   // 목표 타일에 도달한 경우
            {
                Tile step = end;
                while (step != null)
                {
                    path.Add(step);
                    step = previous[step.id];
                }
                path.Reverse();
                return path;
            }
            visited.Add(currentTile);

            foreach (var adjacent in currentTile.adjacents)
            {
                // 이동할 수 없는 타일이거나 이미 방문한 타일인 경우 무시
                if (adjacent == null || !adjacent.CanMove || visited.Contains(adjacent))
                {
                    continue;
                }

                // 지나갈 수 없는 타일인 경우 무시
                if (adjacent.Weight == int.MaxValue)
                {
                    continue;
                }
                // 현재 타일에서 인접 타일까지의 거리 계산
                var newDistance = distances[currentTile.id] + adjacent.Weight;
                if (newDistance < distances[adjacent.id])
                {
                    distances[adjacent.id] = newDistance;
                    previous[adjacent.id] = currentTile;

                    // 우선순위 큐에 인접 타일과 새로운 거리로 업데이트
                    priorityQueue.Enqueue(adjacent, newDistance + Heuristic(adjacent, end));
                }
            }
        }
        return path; // 경로를 찾지 못한 경우 빈 리스트 반환
    }

    // -- 휴리스틱 함수 -- //
    private int Heuristic(Tile a, Tile b)
    {
        int cols = stage.Map.cols;
        int ax = a.id % cols;
        int ay = a.id / cols;
        int bx = b.id % cols;
        int by = b.id / cols;

        return Mathf.Abs(ax - bx) + Mathf.Abs(ay - by);
    }

    // -- 경로 따라가기 coroutine -- //
    private IEnumerator FollowPath()
    {
        // 첫 번째 타일은 현재 위치이므로 두 번째부터 시작
        for (int i = 1; i < path.Count; i++)
        {
            player.MoveTo(path[i].id);

            while (player.isMoving)
            {
                yield return null;
            }
        }
        Debug.Log(path.Count + "개 타일 이동 완료");
    }
}



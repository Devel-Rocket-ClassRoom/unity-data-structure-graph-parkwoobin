using System;
using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;

public class GraphSearch
{
    private Graph graph;
    public List<GraphNode> path = new List<GraphNode>();


    public void Init(Graph graph)
    {
        this.graph = graph;
    }

    public void DFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>();
        var stack = new Stack<GraphNode>();

        stack.Push(node);
        visited.Add(node);

        while (stack.Count > 0)
        {
            var currentNode = stack.Pop();
            path.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }
                visited.Add(adjacent);
                stack.Push(adjacent);
            }
        }
    }

    public void BFS(GraphNode node)
    {
        path.Clear();

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(node);
        visited.Add(node);

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            path.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }
                visited.Add(adjacent);
                queue.Enqueue(adjacent);
            }
        }
    }

    public void DFSRecursive(GraphNode node)
    {
        path.Clear();

        DFSRecursive(node, new HashSet<GraphNode>());
    }

    protected void DFSRecursive(GraphNode node, HashSet<GraphNode> visited)
    {
        path.Add(node);
        visited.Add(node);

        foreach (var adjacent in node.adjacents)
        {
            if (!adjacent.CanVisit || visited.Contains(adjacent))
            {
                continue;
            }
            DFSRecursive(adjacent, visited);
        }
    }

    public bool PathFindingBFS(GraphNode start, GraphNode end)
    {
        path.Clear();
        graph.ResetNodePrevious();  // 노드들의 previous 초기화

        var visited = new HashSet<GraphNode>();
        var queue = new Queue<GraphNode>();

        queue.Enqueue(start);
        visited.Add(start);

        bool success = false;

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();

            if (currentNode == end)
            {
                success = true;
                break;
            }

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                // 방문 처리와 동시에 이전 노드 설정
                visited.Add(adjacent);
                adjacent.previous = currentNode;
                queue.Enqueue(adjacent);
            }
        }
        if (!success)
        {
            return false;
        }
        GraphNode step = end;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;
    }

    public bool Djikstra(GraphNode start, GraphNode end)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        var priorityQueue = new PriorityQueue<GraphNode, int>();
        var distances = new List<int>(new int[graph.nodes.Length]);

        graph.ResetNodePrevious();  // 노드들의 previous 초기화

        foreach (var node in graph.nodes)
        {
            distances[node.id] = int.MaxValue;
        }
        distances[start.id] = 0;

        priorityQueue.Enqueue(start, distances[start.id]);

        bool success = false;

        while (priorityQueue.Count > 0)
        {
            var currentNode = priorityQueue.Dequeue();

            if (currentNode == end)
            {
                success = true;
                break;
            }
            visited.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                // 방문 처리와 동시에 이전 노드 설정
                var newDistance = distances[currentNode.id] + adjacent.weight;
                if (newDistance < distances[adjacent.id])
                {
                    distances[adjacent.id] = newDistance;
                    adjacent.previous = currentNode;
                    priorityQueue.Enqueue(adjacent, newDistance);
                }
            }
        }
        if (!success)
        {
            return false;
        }
        GraphNode step = end;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;

    }
    public bool AStar(GraphNode start, GraphNode end)
    {
        path.Clear();
        var visited = new HashSet<GraphNode>();
        var priorityQueue = new PriorityQueue<GraphNode, int>();
        var distances = new List<int>(new int[graph.nodes.Length]);

        graph.ResetNodePrevious();  // 노드들의 previous 초기화

        foreach (var node in graph.nodes)
        {
            distances[node.id] = int.MaxValue;
        }
        distances[start.id] = 0;

        priorityQueue.Enqueue(start, distances[start.id]);

        bool success = false;

        while (priorityQueue.Count > 0)
        {
            var currentNode = priorityQueue.Dequeue();

            if (currentNode == end)
            {
                success = true;
                break;
            }
            visited.Add(currentNode);

            foreach (var adjacent in currentNode.adjacents)
            {
                if (!adjacent.CanVisit || visited.Contains(adjacent))
                {
                    continue;
                }

                // 방문 처리와 동시에 이전 노드 설정
                var newDistance = distances[currentNode.id] + adjacent.weight;
                if (newDistance < distances[adjacent.id])
                {
                    distances[adjacent.id] = newDistance;
                    adjacent.previous = currentNode;
                    priorityQueue.Enqueue(adjacent, newDistance + Heuristic(adjacent, end));  // A*에서는 휴리스틱을 더해줌
                }
            }
        }
        if (!success)
        {
            return false;
        }
        GraphNode step = end;
        while (step != null)
        {
            path.Add(step);
            step = step.previous;
        }
        path.Reverse();
        return true;

    }

    private int Heuristic(GraphNode a, GraphNode b)
    {
        // 간단한 휴리스틱 함수 (예: 맨해튼 거리)
        int ax = a.id % graph.cols;
        int ay = a.id / graph.cols;
        int bx = b.id % graph.cols;
        int by = b.id / graph.cols;

        return Math.Abs(ax - bx) + Math.Abs(ay - by);
    }
}

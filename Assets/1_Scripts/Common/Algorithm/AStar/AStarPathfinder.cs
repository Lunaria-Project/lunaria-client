using System.Collections.Generic;
using UnityEngine;

public static class AStarPathfinder
{
    private static readonly Vector2Int[] Directions =
    {
        new(0, 1), new(1, 0), new(0, -1), new(-1, 0),
        new(1, 1), new(1, -1), new(-1, 1), new(-1, -1)
    };

    private static readonly float[] DirectionCosts =
    {
        1f, 1f, 1f, 1f,
        1.414f, 1.414f, 1.414f, 1.414f
    };

    private class Node
    {
        public Vector2Int Pos;
        public float G;
        public float H;
        public float F => G + H;
        public Node Parent;
    }

    public static List<Vector2> FindPath(PathGrid grid, Vector2 startWorld, Vector2 endWorld)
    {
        var startGrid = grid.WorldToGrid(startWorld);
        var endGrid = grid.WorldToGrid(endWorld);

        if (!grid.IsWalkable(startGrid.x, startGrid.y))
        {
            startGrid = grid.FindNearestWalkable(startGrid);
        }
        if (!grid.IsWalkable(endGrid.x, endGrid.y))
        {
            endGrid = grid.FindNearestWalkable(endGrid);
        }

        if (startGrid == endGrid)
        {
            return new List<Vector2> { endWorld };
        }

        var openSet = new SortedList<float, List<Node>>();
        var closedSet = new HashSet<long>();
        var gScores = new Dictionary<long, float>();

        var startNode = new Node
        {
            Pos = startGrid,
            G = 0f,
            H = Heuristic(startGrid, endGrid)
        };

        AddToOpenSet(openSet, startNode);
        gScores[PackKey(startGrid)] = 0f;

        while (openSet.Count > 0)
        {
            var current = PopBest(openSet);
            var currentKey = PackKey(current.Pos);

            if (current.Pos == endGrid)
            {
                return BuildAndSmoothPath(grid, current, startWorld, endWorld);
            }

            closedSet.Add(currentKey);

            for (var i = 0; i < Directions.Length; i++)
            {
                var neighborPos = current.Pos + Directions[i];

                if (!grid.IsWalkable(neighborPos.x, neighborPos.y)) continue;

                var neighborKey = PackKey(neighborPos);
                if (closedSet.Contains(neighborKey)) continue;

                // 대각선 이동 시 양쪽 직교 셀도 통과 가능해야 함
                if (i >= 4)
                {
                    var dx = Directions[i].x;
                    var dy = Directions[i].y;
                    if (!grid.IsWalkable(current.Pos.x + dx, current.Pos.y) ||
                        !grid.IsWalkable(current.Pos.x, current.Pos.y + dy))
                    {
                        continue;
                    }
                }

                var tentativeG = current.G + DirectionCosts[i];

                if (gScores.TryGetValue(neighborKey, out var existingG) && tentativeG >= existingG) continue;

                gScores[neighborKey] = tentativeG;
                var neighbor = new Node
                {
                    Pos = neighborPos,
                    G = tentativeG,
                    H = Heuristic(neighborPos, endGrid),
                    Parent = current
                };
                AddToOpenSet(openSet, neighbor);
            }
        }

        // 경로를 찾지 못한 경우 직선 이동 폴백
        return null;
    }

    private static float Heuristic(Vector2Int a, Vector2Int b)
    {
        var dx = Mathf.Abs(a.x - b.x);
        var dy = Mathf.Abs(a.y - b.y);
        return Mathf.Sqrt(dx * dx + dy * dy);
    }

    private static long PackKey(Vector2Int pos)
    {
        return ((long)pos.x << 32) | (uint)pos.y;
    }

    private static void AddToOpenSet(SortedList<float, List<Node>> openSet, Node node)
    {
        if (!openSet.TryGetValue(node.F, out var list))
        {
            list = new List<Node>();
            openSet.Add(node.F, list);
        }
        list.Add(node);
    }

    private static Node PopBest(SortedList<float, List<Node>> openSet)
    {
        var list = openSet.Values[0];
        var node = list[list.Count - 1];
        list.RemoveAt(list.Count - 1);
        if (list.Count == 0)
        {
            openSet.RemoveAt(0);
        }
        return node;
    }

    private static List<Vector2> BuildAndSmoothPath(PathGrid grid, Node endNode, Vector2 startWorld, Vector2 endWorld)
    {
        var rawPath = new List<Vector2>();
        var current = endNode;
        while (current != null)
        {
            rawPath.Add(grid.GridToWorld(current.Pos.x, current.Pos.y));
            current = current.Parent;
        }
        rawPath.Reverse();

        // 첫 점을 실제 시작 위치로, 마지막 점을 실제 목표 위치로 교체
        rawPath[0] = startWorld;
        rawPath[rawPath.Count - 1] = endWorld;

        return rawPath;
    }

    private static bool IsDirectPathClear(Vector2 from, Vector2 to, float radius, int layerMask)
    {
        var direction = to - from;
        var distance = direction.magnitude;
        if (distance < 0.01f) return true;

        var hit = Physics2D.CircleCast(from, radius, direction.normalized, distance, layerMask);
        return hit.collider == null;
    }
}

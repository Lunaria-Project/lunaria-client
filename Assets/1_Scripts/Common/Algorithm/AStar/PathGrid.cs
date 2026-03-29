using UnityEngine;

public class PathGrid
{
    public readonly float CellSize;
    public readonly int Width;
    public readonly int Height;
    public readonly Vector2 Origin;
    public readonly int ObstacleLayerMask;
    private readonly bool[,] _walkable;

    public PathGrid(Bounds mapBounds, float cellSize, float checkRadius, int layerMask)
    {
        CellSize = cellSize;
        ObstacleLayerMask = layerMask;
        Origin = new Vector2(mapBounds.min.x, mapBounds.min.y);
        Width = Mathf.CeilToInt(mapBounds.size.x / cellSize);
        Height = Mathf.CeilToInt(mapBounds.size.y / cellSize);
        _walkable = new bool[Width, Height];

        for (var x = 0; x < Width; x++)
        {
            for (var y = 0; y < Height; y++)
            {
                var worldPos = GridToWorld(x, y);
                var hit = Physics2D.OverlapCircle(worldPos, checkRadius, layerMask);
                _walkable[x, y] = hit == null;
            }
        }
    }

    public bool IsWalkable(int x, int y)
    {
        if (x < 0 || x >= Width || y < 0 || y >= Height) return false;
        return _walkable[x, y];
    }

    public Vector2Int WorldToGrid(Vector2 worldPos)
    {
        var x = Mathf.FloorToInt((worldPos.x - Origin.x) / CellSize);
        var y = Mathf.FloorToInt((worldPos.y - Origin.y) / CellSize);
        x = Mathf.Clamp(x, 0, Width - 1);
        y = Mathf.Clamp(y, 0, Height - 1);
        return new Vector2Int(x, y);
    }

    public Vector2 GridToWorld(int x, int y)
    {
        return new Vector2(
            Origin.x + x * CellSize + CellSize * 0.5f,
            Origin.y + y * CellSize + CellSize * 0.5f
        );
    }

    public Vector2Int FindNearestWalkable(Vector2Int pos)
    {
        if (IsWalkable(pos.x, pos.y)) return pos;

        var maxRadius = Mathf.Max(Width, Height);
        for (var r = 1; r <= maxRadius; r++)
        {
            for (var dx = -r; dx <= r; dx++)
            {
                for (var dy = -r; dy <= r; dy++)
                {
                    if (Mathf.Abs(dx) != r && Mathf.Abs(dy) != r) continue;
                    var nx = pos.x + dx;
                    var ny = pos.y + dy;
                    if (IsWalkable(nx, ny)) return new Vector2Int(nx, ny);
                }
            }
        }

        return pos;
    }
}

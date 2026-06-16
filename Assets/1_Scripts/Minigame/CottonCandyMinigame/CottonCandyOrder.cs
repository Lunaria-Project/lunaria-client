public enum CottonCandyColor
{
    None = 0,
    White = 1,
    Purple = 2,
    Yellow = 3,
    Blue = 4,
    Pink = 5,
}

public enum CottonCandyShape
{
    None = 0,
    Circle = 1,
    Cat = 2,
    Star = 3,
}

public enum CottonCandyRotationDirection
{
    Clockwise = 0,
    CounterClockwise = 1,
}

public struct CottonCandyLayerOrder
{
    public CottonCandyColor Color;
    public CottonCandyShape Shape;
    public CottonCandyRotationDirection Direction;
    public int RotationCount;
}

public class CottonCandyOrder
{
    public int Quantity;
    public int ScoreReward;
    public CottonCandyLayerOrder[] Layers;
}

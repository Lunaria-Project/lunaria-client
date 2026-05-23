public enum CottonCandyColor
{
    None = 0,
    Pink = 1,
    Yellow = 2,
    Blue = 3,
    Purple = 4,
    White = 5,
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

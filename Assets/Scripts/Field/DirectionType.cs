using UnityEngine;

public enum DirectionType
{
    Right = 0,
    Left = 1,
    Up,
    Down
}

public static class DirectionExtensions{
    public static Vector2Int ToVector(this DirectionType direction)
    {
        return direction switch
        {
            DirectionType.Right => Vector2Int.right,
            DirectionType.Left => Vector2Int.left,
            DirectionType.Up => Vector2Int.up,
            DirectionType.Down => Vector2Int.down
        };
    }
}
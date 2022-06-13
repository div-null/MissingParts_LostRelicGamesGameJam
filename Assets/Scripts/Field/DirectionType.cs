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
    
    public static Vector2Int ToVector(this Vector3 direction)
    {
        return new Vector2Int((int)direction.x, (int)direction.y);
    }

    public static DirectionType ToDirection(this Vector2Int direction)
    {
        if (direction == Vector2Int.left)
            return DirectionType.Left;
        else if (direction == Vector2Int.right)
            return DirectionType.Right;
        else if (direction == Vector2Int.up)
            return DirectionType.Up;
        else if (direction == Vector2Int.down)
            return DirectionType.Down;
        else
        {
            Debug.LogError("Wrong direction");
            return DirectionType.Down;
        }
    }
    
    public static DirectionType ToDirection(this int degrees)
    {
        return degrees switch
        {
            0 => DirectionType.Up,
            90=> DirectionType.Left,
            180 => DirectionType.Down,
            270 => DirectionType.Right
        };
    }
    
    public static DirectionType ToDirection(this Vector2 direction)
    {
        if (direction == Vector2.left)
            return DirectionType.Left;
        else if (direction == Vector2.right)
            return DirectionType.Right;
        else if (direction == Vector2.up)
            return DirectionType.Up;
        else if (direction == Vector2.down)
            return DirectionType.Down;
        else
        {
            Debug.LogError("Wrong direction");
            return DirectionType.Down;
        }
    }
    
    public static DirectionType ToDirection(this Vector3 direction)
    {
        if (direction == Vector3.left)
            return DirectionType.Left;
        else if (direction == Vector3.right)
            return DirectionType.Right;
        else if (direction == Vector3.up)
            return DirectionType.Up;
        else if (direction == Vector3.down)
            return DirectionType.Down;
        else
        {
            Debug.LogError("Wrong direction");
            return DirectionType.Down;
        }
    }
}
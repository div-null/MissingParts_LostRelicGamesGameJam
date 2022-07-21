using UnityEngine;

public static class UnityExtensions
{
    public static Vector3 ToVector3(this Vector2Int vec) => 
        new(vec.x, vec.y, 0);
    
    public static Vector2Int ToVector2Int(this Vector3 direction) => 
        new Vector2Int((int) direction.x, (int) direction.y);
}
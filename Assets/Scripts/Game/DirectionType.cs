using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    [Flags]
    public enum DirectionType
    {
        None = 0,
        Right = 1,
        Left = 2,
        Up = 4,
        Down = 8,
        All = 15
    }

    public static class DirectionExtensions
    {
        public static DirectionType[] directions = new[] {DirectionType.Right, DirectionType.Left, DirectionType.Up, DirectionType.Down};

        public static IEnumerable<TEnum> GetFlags<TEnum>(this TEnum input) where TEnum : Enum
        {
            foreach (Enum value in Enum.GetValues(input.GetType()))
                if (input.HasFlag(value))
                    yield return (TEnum) value;
        }

        public static bool HasFlagEq<TEnum>(this TEnum source, TEnum flags) where TEnum : Enum =>
            source.Equals(flags) || source.HasFlag(flags);

        public static DirectionType RotateRight(this DirectionType direction)
        {
            return direction switch
            {
                DirectionType.Up => DirectionType.Right,
                DirectionType.Right => DirectionType.Down,
                DirectionType.Down => DirectionType.Left,
                DirectionType.Left => DirectionType.Up,
                DirectionType.Up | DirectionType.Right => DirectionType.Right | DirectionType.Down,
                DirectionType.Up | DirectionType.Left => DirectionType.Up | DirectionType.Right,
                DirectionType.Down | DirectionType.Left => DirectionType.Up | DirectionType.Left,
                DirectionType.Down | DirectionType.Right => DirectionType.Down | DirectionType.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(direction), "Not allowed")
            };
        }

        public static DirectionType Invert(this DirectionType direction) =>
            direction.RotateRight().RotateRight();

        public static DirectionType NegateSingle(this DirectionType direction)
        {
            var directionTypes = directions.Except(direction.GetFlags()).Intersect(directions).ToList();
            return directionTypes.Single();
        }

        public static DirectionType Negate(this DirectionType direction)
        {
            var directionTypes = directions.Except(direction.GetFlags()).Intersect(directions).ToList();
            return directionTypes.Aggregate(DirectionType.None, (acc, cur) => acc | cur);
        }


        public static int Degrees(this DirectionType direction)
        {
            return direction switch
            {
                DirectionType.Up => 0,
                DirectionType.Right => 90,
                DirectionType.Down => 180,
                DirectionType.Left => 270
            };
        }

        public static Vector2Int ToVector2Int(this DirectionType direction)
        {
            return direction switch
            {
                DirectionType.Right => Vector2Int.right,
                DirectionType.Left => Vector2Int.left,
                DirectionType.Up => Vector2Int.up,
                DirectionType.Down => Vector2Int.down
            };
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

        [Obsolete]
        public static DirectionType ToDirection(this int degrees)
        {
            return degrees switch
            {
                0 => DirectionType.Up,
                90 => DirectionType.Right,
                180 => DirectionType.Down,
                270 => DirectionType.Left
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
                Debug.LogError($"Wrong direction: {direction}");
                return DirectionType.None;
            }
        }

        [Obsolete]
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
                return DirectionType.None;
            }
        }

        public static DirectionType[] GetPerpendicularDirections(this DirectionType direction)
        {
            return direction switch
            {
                DirectionType.Right => new[] {DirectionType.Up, DirectionType.Down},
                DirectionType.Left => new[] {DirectionType.Up, DirectionType.Down},
                DirectionType.Up => new[] {DirectionType.Left, DirectionType.Right},
                DirectionType.Down => new[] {DirectionType.Left, DirectionType.Right},
                _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Not allowed")
            };
        }
    }
}
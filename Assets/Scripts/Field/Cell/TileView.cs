using System;
using System.Linq;
using UnityEngine;

public class TileView : MonoBehaviour
{
    public SpriteRenderer First;
    public SpriteRenderer Second;
    public SpriteRenderer Third;
    public SpriteRenderer Fourth;

    public Sprite OuterCorner;
    public Sprite InnerCorner;
    public Sprite Vertical;


    public void DrawBorders(DirectionType borders, Vector2Int position, DirectionType[,] bordersMap)
    {
        switch (borders)
        {
            case DirectionType.All:
                MakeSquared();
                break;
            case DirectionType.Left:
            case DirectionType.Up:
            case DirectionType.Right:
            case DirectionType.Down:
                MakePlain(borders);
                break;
            case DirectionType.Down | DirectionType.Up:
                MakePlain(DirectionType.Down);
                MakePlain(DirectionType.Up);
                break;
            case DirectionType.Left | DirectionType.Right:
                MakePlain(DirectionType.Left);
                MakePlain(DirectionType.Right);
                break;
            case DirectionType.Up | DirectionType.Right:
            case DirectionType.Up | DirectionType.Left:
            case DirectionType.Down | DirectionType.Left:
            case DirectionType.Down | DirectionType.Right:
                MakeCornered(borders);
                break;
            case DirectionType.Left | DirectionType.Up | DirectionType.Right:
                MakeInpass(DirectionType.Down);
                break;
            case DirectionType.Up | DirectionType.Right | DirectionType.Down:
                MakeInpass(DirectionType.Left);
                break;
            case DirectionType.Right | DirectionType.Down | DirectionType.Left:
                MakeInpass(DirectionType.Up);
                break;
            case DirectionType.Down | DirectionType.Left | DirectionType.Up:
                MakeInpass(DirectionType.Right);
                break;
        }

        this.FillCorners(position, bordersMap);
    }

    private void FillCorners(Vector2Int cellPosition, DirectionType[,] bordersMap)
    {
        void CheckCorner(DirectionType currentTile, DirectionType firstTile, DirectionType secondTile, DirectionType firstDirection, DirectionType secondDirection)
        {
            if (!currentTile.HasFlag(firstDirection) && firstTile.HasFlag(secondDirection) && !currentTile.HasFlag(secondDirection) && secondTile.HasFlag(firstDirection))
            {
                SpriteRenderer tile = this.SelectTiles(secondDirection | firstDirection).Single();
                tile.sprite = this.OuterCorner;
                SetupOuterCorner(secondDirection | firstDirection, tile);
            }
        }

        DirectionType GerBorders(Vector2Int coords)
        {
            if (coords.x < 0 || coords.y < 0 || coords.x >= bordersMap.GetLength(0) || coords.y >= bordersMap.GetLength(1))
                return DirectionType.None;
            return bordersMap[coords.x, coords.y];
        }

        DirectionType current = GerBorders(cellPosition);

        var top = GerBorders(cellPosition + Vector2Int.up);
        var down = GerBorders(cellPosition + Vector2Int.down);
        var right = GerBorders(cellPosition + Vector2Int.right);
        var left = GerBorders(cellPosition + Vector2Int.left);

        CheckCorner(current, top, right, DirectionType.Right, DirectionType.Up);
        CheckCorner(current, right, down, DirectionType.Down, DirectionType.Right);
        CheckCorner(current, down, left, DirectionType.Left, DirectionType.Down);
        CheckCorner(current, left, top, DirectionType.Up, DirectionType.Left);
    }

    private SpriteRenderer[] SelectTiles(DirectionType borders)
    {
        return borders switch
        {
            DirectionType.None => Array.Empty<SpriteRenderer>(),
            DirectionType.Right => new[] {Second, Third},
            DirectionType.Left => new[] {First, Fourth},
            DirectionType.Up => new[] {First, Second},
            DirectionType.Down => new[] {Third, Fourth},
            DirectionType.Left | DirectionType.Up => new[] {First},
            DirectionType.Right | DirectionType.Up => new[] {Second},
            DirectionType.Right | DirectionType.Down => new[] {Third},
            DirectionType.Left | DirectionType.Down => new[] {Fourth},
            _ => throw new ArgumentOutOfRangeException(nameof(borders), "Can't get borders")
        };
    }

    private void MakeSquared()
    {
        SetupInnerCorner(DirectionType.Up | DirectionType.Left, First);
        SetupInnerCorner(DirectionType.Up | DirectionType.Right, Second);
        SetupInnerCorner(DirectionType.Down | DirectionType.Right, Third);
        SetupInnerCorner(DirectionType.Down | DirectionType.Left, Fourth);
    }

    private void MakePlain(DirectionType pointingDirection)
    {
        SpriteRenderer[] sides = SelectTiles(pointingDirection);
        foreach (SpriteRenderer side in sides)
            side.sprite = Vertical;

        SetupSide(pointingDirection, sides[0]);
        SetupSide(pointingDirection, sides[1]);
    }

    private void MakeCornered(DirectionType direction)
    {
        First.sprite = InnerCorner;
        Second.sprite = Vertical;
        Fourth.sprite = Vertical;

        SetupInnerCorner(DirectionType.Left | DirectionType.Up, First);
        SetupSide(DirectionType.Left, Fourth);
        SetupSide(DirectionType.Up, Second);

        TrySetRotation(direction, DirectionType.Left | DirectionType.Up, transform, 0);
        TrySetRotation(direction, DirectionType.Right | DirectionType.Up, transform, 90);
        TrySetRotation(direction, DirectionType.Right | DirectionType.Down, transform, 180);
        TrySetRotation(direction, DirectionType.Left | DirectionType.Down, transform, 270);
    }

    private void MakeInpass(DirectionType direction)
    {
        First.sprite = InnerCorner;
        Second.sprite = InnerCorner;
        Third.sprite = Vertical;
        Fourth.sprite = Vertical;

        SetupInnerCorner(DirectionType.Left | DirectionType.Up, First);
        SetupInnerCorner(DirectionType.Right | DirectionType.Up, Second);
        SetupSide(DirectionType.Left, Fourth);
        SetupSide(DirectionType.Right, Third);

        TrySetRotation(direction, DirectionType.Down, transform, 0);
        TrySetRotation(direction, DirectionType.Left, transform, 90);
        TrySetRotation(direction, DirectionType.Up, transform, 180);
        TrySetRotation(direction, DirectionType.Right, transform, 270);
    }

    private void TrySetRotation(DirectionType sourceDirection, DirectionType targetDirection, Transform tile, int angle)
    {
        if (sourceDirection.HasFlag(targetDirection))
            tile.rotation = Quaternion.AngleAxis(angle, Vector3.back);
    }

    private void SetupOuterCorner(DirectionType pointingDirection, SpriteRenderer tile)
    {
        TrySetRotation(pointingDirection, DirectionType.Down | DirectionType.Right, tile.transform, 0);
        TrySetRotation(pointingDirection, DirectionType.Down | DirectionType.Left, tile.transform, 90);
        TrySetRotation(pointingDirection, DirectionType.Up | DirectionType.Left, tile.transform, 180);
        TrySetRotation(pointingDirection, DirectionType.Up | DirectionType.Right, tile.transform, 270);
    }

    private void SetupInnerCorner(DirectionType pointingDirection, SpriteRenderer tile)
    {
        TrySetRotation(pointingDirection, DirectionType.Up | DirectionType.Left, tile.transform, 0);
        TrySetRotation(pointingDirection, DirectionType.Up | DirectionType.Right, tile.transform, 90);
        TrySetRotation(pointingDirection, DirectionType.Down | DirectionType.Right, tile.transform, 180);
        TrySetRotation(pointingDirection, DirectionType.Down | DirectionType.Left, tile.transform, 270);
    }

    private void SetupSide(DirectionType pointingDirection, SpriteRenderer tile)
    {
        TrySetRotation(pointingDirection, DirectionType.Right, tile.transform, 0);
        TrySetRotation(pointingDirection, DirectionType.Up, tile.transform, 270);
        TrySetRotation(pointingDirection, DirectionType.Left, tile.transform, 180);
        TrySetRotation(pointingDirection, DirectionType.Down, tile.transform, 90);
    }
}
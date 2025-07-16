using System;
using System.Linq;
using LevelEditor;
using UnityEngine;

namespace Game.Cell
{
    public class TileView : MonoBehaviour
    {
        public SpriteRenderer First;
        public SpriteRenderer Second;
        public SpriteRenderer Third;
        public SpriteRenderer Fourth;

        public Sprite OuterCorner;
        public Sprite InnerCorner;
        public Sprite Vertical;

        [SerializeField] private Vector2Int _position;


        public void DrawBorders(CellData current, CellData? up, CellData? down, CellData? right, CellData? left, Vector2Int currentPosition)
        {
            _position = currentPosition;

            switch (current.Borders)
            {
                case DirectionType.All:
                    MakeSquared();
                    break;
                case DirectionType.Left:
                case DirectionType.Up:
                case DirectionType.Right:
                case DirectionType.Down:
                    MakePlain(current.Borders);
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
                    MakeCornered(current.Borders);
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

            FillCorners(current, up, down, right, left);
        }

        private void FillCorners(CellData currentTile, CellData? top, CellData? down, CellData? right, CellData? left)
        {
            void TryPlaceCorners(CellData source, CellData? neighbour1, CellData? neighbour2, DirectionType firstDirection, DirectionType secondDirection)
            {
                if (source.Type != neighbour1?.Type || source.Type != neighbour2?.Type)
                {
                    return;
                }

                DirectionType current = source.Borders;
                DirectionType borders1 = neighbour1?.Borders ?? DirectionType.None;
                DirectionType borders2 = neighbour2?.Borders ?? DirectionType.None;
                DirectionType cornerDirection = firstDirection | secondDirection;

                if (!current.HasFlag(firstDirection) && !current.HasFlag(secondDirection) && borders1 != borders2.Negate() )
                {
                    // rich shape
                    if (borders1.HasFlagEq(secondDirection) && borders2.HasFlagEq(firstDirection))
                    {
                        SpriteRenderer tile = SelectTiles(cornerDirection).Single();
                        tile.sprite = OuterCorner;
                        SetupOuterCorner(cornerDirection, tile);
                    }
                    // outside of map corners
                    else if (borders1.HasFlagEq(firstDirection) && borders2.HasFlagEq(secondDirection))
                    {
                        SpriteRenderer tile = SelectTiles(cornerDirection).Single();
                        tile.sprite = OuterCorner;
                        SetupOuterCorner(cornerDirection, tile);
                    }
                }
            }

            TryPlaceCorners(currentTile, top, right, DirectionType.Right, DirectionType.Up);
            TryPlaceCorners(currentTile, right, down, DirectionType.Down, DirectionType.Right);
            TryPlaceCorners(currentTile, down, left, DirectionType.Left, DirectionType.Down);
            TryPlaceCorners(currentTile, left, top, DirectionType.Up, DirectionType.Left);
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
            First.sprite = InnerCorner;
            Second.sprite = InnerCorner;
            Third.sprite = InnerCorner;
            Fourth.sprite = InnerCorner;

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
            DirectionType rightSide = direction.RotateRight();
            DirectionType downSide = rightSide.RotateRight().RotateRight();

            SpriteRenderer corner = SelectTiles(direction).Single();
            SpriteRenderer right = SelectTiles(rightSide).Single();
            SpriteRenderer down = SelectTiles(downSide).Single();

            right.sprite = Vertical;
            corner.sprite = InnerCorner;
            down.sprite = Vertical;

            SetupSide(rightSide & direction, right);
            SetupInnerCorner(direction, corner);
            SetupSide(downSide & direction, down);
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
}
using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Field.Cell;
using LevelEditor;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game
{
    public class LevelFactory
    {
        private IObjectResolver _resolver;
        private GameSettings _gameSettings;

        static Vector3 left = Vector3.left * 0.25f;
        static Vector3 up = Vector3.up * 0.25f;
        static Vector3 right = -left;
        static Vector3 down = -up;

        public LevelFactory(IObjectResolver resolver, GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
            _resolver = resolver;
        }

        public Character CreateCharacter(GameLevel level, Field field)
        {
            List<CharacterPart> parts = new List<CharacterPart>();
            Character character = _resolver.Instantiate(_gameSettings.CharacterPrefab);
            character.Initialize(field);
            var playerParts = level.PlayerParts.Where(part => part.IsActive);
            foreach (var part in playerParts)
            {
                CharacterPart characterPart = CreateCharacterPart(field, part);
                characterPart.Initialize(new Vector2Int(part.X, part.Y), true, field, part.Rotation, part.Color);

                characterPart.CharacterPartAttachment.AttachParts();
                field.Get(part.X, part.Y).AssignCharacterPart(characterPart);
                parts.Add(characterPart);
            }

            character.AddParts(parts);
            return character;
        }

        public Field CreateField(GameLevel level)
        {
            var cells = new Cell[level.MapWidth, level.MapHeight];
            var characterParts = new List<CharacterPart>();

            DirectionType[,] cellsBorders = level.GetCellsBorders();
            Field field = _resolver.Instantiate(_gameSettings.FieldPrefab);
            field.SetCells(cells);
            SetupCamera(level);

            for (int j = 0; j < level.MapHeight; j++)
            {
                for (int i = 0; i < level.MapWidth; i++)
                {
                    CellContainer cellData = level.Get(i, j);
                    DirectionType borders = cellsBorders[i, j];
                    Vector2Int cellPosition = new Vector2Int(i, j);
                    Cell newCell = CreateCell(i, j, field.transform, cellData);
                    TileView tileView = newCell.GetComponent<TileView>();

                    if (newCell.IsWall() || newCell.IsPit())
                    {
                        drawBorders(borders, tileView);
                    }

                    newCell.Initialize(cellPosition, cellData.Type, borders);
                    cells[i, j] = newCell;
                }
            }

            var inactiveParts = level.PlayerParts.Where(part => !part.IsActive);
            foreach (var playerPart in inactiveParts)
            {
                CharacterPart newCharacterPart = CreateCharacterPart(field, playerPart);
                newCharacterPart.Initialize(new Vector2Int(playerPart.X, playerPart.Y), false, field, playerPart.Rotation, playerPart.Color);
                characterParts.Add(newCharacterPart);
            }

            foreach (var part in characterParts)
            {
                part.CharacterPartAttachment.AttachParts();
                cells[part.Position.x, part.Position.y].AssignCharacterPart(part);
            }


            List<Cell> finishCells = FindFinishCells(cells);
            field.Setup(finishCells, level.FinishColor);

            return field;
        }

        private void drawBorders(DirectionType currentBorders, TileView view)
        {
            switch (currentBorders)
            {
                case DirectionType.All:
                    makeSquared(view);
                    break;
                case DirectionType.Left:
                case DirectionType.Up:
                case DirectionType.Right:
                case DirectionType.Down:
                    makePlain(currentBorders, view);
                    break;
                case DirectionType.Down | DirectionType.Up:
                    makePlain(DirectionType.Down, view);
                    makePlain(DirectionType.Up, view);
                    break;
                case DirectionType.Left | DirectionType.Right:
                    makePlain(DirectionType.Left, view);
                    makePlain(DirectionType.Right, view);
                    break;
                case DirectionType.Up | DirectionType.Right:
                    spawnCornered(DirectionType.Up | DirectionType.Right, view);
                    break;
                case DirectionType.Up | DirectionType.Left:
                    spawnCornered(DirectionType.Up | DirectionType.Left, view);
                    break;
                case DirectionType.Down | DirectionType.Left:
                    spawnCornered(DirectionType.Down | DirectionType.Left, view);
                    break;
                case DirectionType.Down | DirectionType.Right:
                    spawnCornered(DirectionType.Down | DirectionType.Right, view);
                    break;
                case DirectionType.Left | DirectionType.Up | DirectionType.Right:
                    spawnInpass(DirectionType.Down, view);
                    break;
                case DirectionType.Up | DirectionType.Right | DirectionType.Down:
                    spawnInpass(DirectionType.Left, view);
                    break;
                case DirectionType.Right | DirectionType.Down | DirectionType.Left:
                    spawnInpass(DirectionType.Up, view);
                    break;
                case DirectionType.Down | DirectionType.Left | DirectionType.Up:
                    spawnInpass(DirectionType.Right, view);
                    break;
                // case DirectionType.None:
                //     fillCorners(cellPosition, level, borders, view);
                //     break;
            }
        }


        // private void fillCorners(Vector2Int cellPosition, GameLevel level, Transform parent, GameObject outerCornerPrefab)
        // {
        //     DirectionType getBorders(Vector2Int coords)
        //     {
        //         CellContainer cell = level.Get(cellPosition + Vector2Int.up);
        //         if (cell == null) return DirectionType.None;
        //         return cell.Type == CellType.Wall ? getBorderDirections(level, CellType.Wall, coords.x, coords.y) : DirectionType.None;
        //     }
        //
        //     var top = getBorders(cellPosition + Vector2Int.up);
        //     var down = getBorders(cellPosition + Vector2Int.down);
        //     var right = getBorders(cellPosition + Vector2Int.right);
        //     var left = getBorders(cellPosition + Vector2Int.left);
        //
        //     if (top.HasFlag(DirectionType.Right) && right.HasFlag(DirectionType.Up))
        //         spawnOuterCorner(DirectionType.Right | DirectionType.Up, parent, outerCornerPrefab);
        //
        //     if (right.HasFlag(DirectionType.Down) && down.HasFlag(DirectionType.Right))
        //         spawnOuterCorner(DirectionType.Down | DirectionType.Right, parent, outerCornerPrefab);
        //
        //     if (down.HasFlag(DirectionType.Left) && left.HasFlag(DirectionType.Down))
        //         spawnOuterCorner(DirectionType.Left | DirectionType.Down, parent, outerCornerPrefab);
        //
        //     if (top.HasFlag(DirectionType.Left) && left.HasFlag(DirectionType.Up))
        //         spawnOuterCorner(DirectionType.Left | DirectionType.Up, parent, outerCornerPrefab);
        // }


        private void spawnInpass(DirectionType direction, TileView view)
        {
            
            setupInnerCorner(DirectionType.Left | DirectionType.Up, view.First);
            setupInnerCorner(DirectionType.Right | DirectionType.Up, view.Second);
            setupSide(DirectionType.Left, view.Fourth);
            setupSide(DirectionType.Right, view.Third);
            
            trySetRotation(direction, DirectionType.Up, view.transform, 0);
            trySetRotation(direction, DirectionType.Right, view.transform, 90);
            trySetRotation(direction, DirectionType.Down, view.transform, 180);
            trySetRotation(direction, DirectionType.Left, view.transform, 270);
            
        }

        private void spawnCornered(DirectionType direction, TileView view)
        {
            setupInnerCorner(DirectionType.Left | DirectionType.Up, view.First);
            setupSide(DirectionType.Left, view.Fourth);
            setupSide(DirectionType.Up, view.Second);
            
            trySetRotation(direction, DirectionType.Left | DirectionType.Up, view.transform, 0);
            trySetRotation(direction, DirectionType.Right | DirectionType.Up, view.transform, 90);
            trySetRotation(direction, DirectionType.Right | DirectionType.Down, view.transform, 180);
            trySetRotation(direction, DirectionType.Left | DirectionType.Down, view.transform, 270);
        }

        private void makePlain(DirectionType pointingDirection, TileView view)
        {
            SpriteRenderer[] sides = selectTiles(view, pointingDirection);
            setupSide(pointingDirection, sides[0]);
            setupSide(pointingDirection, sides[1]);
        }

        private SpriteRenderer[] selectTiles(TileView view, DirectionType borders)
        {
            return borders switch
            {
                DirectionType.None => Array.Empty<SpriteRenderer>(),
                DirectionType.Right => new[] {view.Second, view.Third},
                DirectionType.Left => new[] {view.First, view.Fourth},
                DirectionType.Up => new[] {view.First, view.Second},
                DirectionType.Down => new[] {view.Third, view.Fourth},
                DirectionType.Left | DirectionType.Up => new[] {view.First},
                DirectionType.Right | DirectionType.Up => new[] {view.Second},
                DirectionType.Right | DirectionType.Down => new[] {view.Third},
                DirectionType.Left | DirectionType.Down => new[] {view.Fourth},
                _ => throw new ArgumentOutOfRangeException(nameof(borders), "Can't get borders")
            };
        }

        void trySetRotation(DirectionType sourceDirection, DirectionType targetDirection, Transform tile, int angle)
        {
            if (sourceDirection.HasFlag(targetDirection))
                tile.rotation = Quaternion.AngleAxis(angle, Vector3.back);
        }

        private void makeSquared(TileView tile)
        {
            setupInnerCorner(DirectionType.Up | DirectionType.Left, tile.First);
            setupInnerCorner(DirectionType.Up | DirectionType.Right, tile.Second);
            setupInnerCorner(DirectionType.Down | DirectionType.Right, tile.Third);
            setupInnerCorner(DirectionType.Down | DirectionType.Left, tile.Fourth);
        }

        private void setupOuterCorner(DirectionType pointingDirection, SpriteRenderer tile)
        {
            trySetRotation(pointingDirection, DirectionType.Up | DirectionType.Left, tile.transform, 0);
            trySetRotation(pointingDirection, DirectionType.Up | DirectionType.Right, tile.transform, 90);
            trySetRotation(pointingDirection, DirectionType.Down | DirectionType.Right, tile.transform, 180);
            trySetRotation(pointingDirection, DirectionType.Down | DirectionType.Left, tile.transform, 270);
        }

        private void setupInnerCorner(DirectionType pointingDirection, SpriteRenderer tile)
        {
            trySetRotation(pointingDirection, DirectionType.Up | DirectionType.Left, tile.transform, 0);
            trySetRotation(pointingDirection, DirectionType.Up | DirectionType.Right, tile.transform, 90);
            trySetRotation(pointingDirection, DirectionType.Down | DirectionType.Right, tile.transform, 180);
            trySetRotation(pointingDirection, DirectionType.Down | DirectionType.Left, tile.transform, 270);
        }

        private void setupSide(DirectionType pointingDirection, SpriteRenderer tile)
        {
            trySetRotation(pointingDirection, DirectionType.Right, tile.transform, 0);
            trySetRotation(pointingDirection, DirectionType.Up, tile.transform, 270);
            trySetRotation(pointingDirection, DirectionType.Left, tile.transform, 180);
            trySetRotation(pointingDirection, DirectionType.Down, tile.transform, 90);
        }

        // private static Transform rotateAndTranslate(Transform tile, DirectionType pointingDirection, float angle)
        // {
        //     tile.Rotate(Vector3.back, angle, Space.Self);
        //     if (pointingDirection.HasFlag(DirectionType.Up))
        //         tile.Translate(up, Space.World);
        //     if (pointingDirection.HasFlag(DirectionType.Right))
        //         tile.Translate(right, Space.World);
        //     if (pointingDirection.HasFlag(DirectionType.Down))
        //         tile.Translate(down, Space.World);
        //     if (pointingDirection.HasFlag(DirectionType.Left))
        //         tile.Translate(left, Space.World);
        //
        //     return tile;
        // }

        private Cell CreateCell(int x, int y, Transform parent, CellContainer cellContainer)
        {
            Vector3 cellPosition = new Vector3(x, y, -1);
            Cell cell = cellContainer.Type switch
            {
                CellType.Wall => _resolver.Instantiate(_gameSettings.WallCellPrefab, cellPosition, Quaternion.identity, parent),
                CellType.Empty => _resolver.Instantiate(_gameSettings.EmptyCellPrefab, cellPosition, Quaternion.identity, parent),
                CellType.Pit => _resolver.Instantiate(_gameSettings.HoleCellPrefab, cellPosition, Quaternion.identity, parent),
                CellType.Finish => _resolver.Instantiate(_gameSettings.FinishCellPrefab, cellPosition, Quaternion.identity, parent),
                _ => throw new ArgumentOutOfRangeException(nameof(cellContainer.Type), "Unknown cell type")
            };
            return cell;
        }

        private CharacterPart CreateCharacterPart(Field field, CharacterPartData partData)
        {
            Vector3 partPosition = field.Get(partData.X, partData.Y).gameObject.transform.position - Vector3.forward;
            CharacterPart characterPart = _resolver.Instantiate(_gameSettings.CharacterPartPrefab,
                partPosition, Quaternion.identity);

            var partView = characterPart.GetComponent<CharacterPartView>();

            if (partData.Ability == AbilityType.Hook)
            {
                var renderer = characterPart.transform.GetComponentInChildren<SpriteRenderer>();
                var hookView = _resolver.Instantiate(_gameSettings.HookPrefab, renderer.transform);
                hookView.transform.rotation = Quaternion.identity;
            }

            partView.Initialize(partData);
            characterPart.CharacterPartView = partView;

            field.Get(partData.X, partData.Y).AssignCharacterPart(characterPart);
            setupAbilities(partData, field, characterPart);

            var characterMovement = setupCharacterMovement(field, characterPart);
            var characterAttachment = setupCharacterAttachment(field, characterPart);

            characterPart.CharacterPartMovement = characterMovement;
            characterPart.CharacterPartAttachment = characterAttachment;
            return characterPart;
        }

        private List<Cell> FindFinishCells(Cell[,] cells)
        {
            var finishCells = new List<Cell>();

            for (int j = 0; j < cells.GetLength(1); j++)
            {
                for (int i = 0; i < cells.GetLength(0); i++)
                {
                    if (cells[i, j].CellType == CellType.Finish)
                    {
                        finishCells.Add(cells[i, j]);
                    }
                }
            }

            return finishCells;
        }

        private static CharacterPartAttachment setupCharacterAttachment(Field field, CharacterPart characterPart)
        {
            var characterAttachment = characterPart.GetOrAddComponent<CharacterPartAttachment>();
            characterAttachment.Initialize(characterPart, field);
            return characterAttachment;
        }

        private static CharacterPartMovement setupCharacterMovement(Field field, CharacterPart characterPart)
        {
            var characterMovement = characterPart.GetOrAddComponent<CharacterPartMovement>();
            characterMovement.Initialize(field, characterPart);
            return characterMovement;
        }

        private static void setupAbilities(CharacterPartData partData, Field field, CharacterPart characterPart)
        {
            if (partData.Ability == AbilityType.Hook)
            {
                var pullAbility = characterPart.GetOrAddComponent<PullAbility>();
                pullAbility.Initialize(characterPart, field, directionFromAngle(partData.Rotation));
            }
            else if (partData.Ability == AbilityType.Rotation)
            {
                var rotateAbility = characterPart.GetOrAddComponent<RotateAbility>();
                rotateAbility.Initialize(characterPart, field);
            }
        }

        private void SetupCamera(GameLevel level)
        {
            Camera.main.transform.position =
                new Vector3(level.MapWidth / 2f - 0.5f, level.MapHeight / 2f - 0.5f, -10);
        }

        private static DirectionType directionFromAngle(int partRotation)
        {
            return partRotation switch
            {
                0 => DirectionType.Up,
                360 => DirectionType.Up,
                90 => DirectionType.Left,
                -270 => DirectionType.Left,
                180 => DirectionType.Down,
                -180 => DirectionType.Down,
                -90 => DirectionType.Right,
                270 => DirectionType.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(partRotation), partRotation, "Wrong rotation angle")
            };
        }
    }
}
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

            Field field = _resolver.Instantiate(_gameSettings.FieldPrefab);
            field.SetCells(cells);
            SetupCamera(level);

            for (int j = 0; j < level.MapHeight; j++)
            {
                for (int i = 0; i < level.MapWidth; i++)
                {
                    CellContainer cellData = level.Get(i, j);
                    DirectionType borders = getBorderDirections(level, cellData.Type, i, j);
                    Vector2Int cellPosition = new Vector2Int(i, j);
                    Cell newCell = CreateCell(i, j, field.transform, cellData);

                    if (newCell.IsWall() || newCell.IsPit())
                        drawBorders(borders, cellPosition, level, newCell.transform);

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
            field.Setup(finishCells);

            return field;
        }

        private void drawBorders(DirectionType borderDirections, Vector2Int cellPosition, GameLevel level, Transform wallTransform)
        {
            CellType type = level.Get(cellPosition).Type;

            GameObject sidePrefab;
            GameObject innerCornerPrefab;
            GameObject outerCornerPrefab;

            if (type == CellType.Wall)
            {
                sidePrefab = _gameSettings.WallCellPrefabs.VerticalWall;
                innerCornerPrefab = _gameSettings.WallCellPrefabs.InnerCorner;
                outerCornerPrefab = _gameSettings.WallCellPrefabs.OuterCorner;
            } else if (type == CellType.Pit)
            {
                sidePrefab = _gameSettings.PitCellPrefabs.Vertical;
                innerCornerPrefab = _gameSettings.PitCellPrefabs.InnerCorner;
                outerCornerPrefab = _gameSettings.PitCellPrefabs.OuterCorner;
            }
            else
            {
                return;
            }
            
            switch (borderDirections)
            {
                case DirectionType.All:
                    spawnSquared(wallTransform, innerCornerPrefab);
                    break;
                case DirectionType.Left:
                    spawnPlain(DirectionType.Left, wallTransform, sidePrefab);
                    break;
                case DirectionType.Up:
                    spawnPlain(DirectionType.Up, wallTransform, sidePrefab);
                    break;
                case DirectionType.Right:
                    spawnPlain(DirectionType.Right, wallTransform, sidePrefab);
                    break;
                case DirectionType.Down:
                    spawnPlain(DirectionType.Down, wallTransform, sidePrefab);
                    break;
                case DirectionType.Down | DirectionType.Up:
                    spawnPlain(DirectionType.Down, wallTransform, sidePrefab);
                    spawnPlain(DirectionType.Up, wallTransform, sidePrefab);
                    break;
                case DirectionType.Left | DirectionType.Right:
                    spawnPlain(DirectionType.Left, wallTransform, sidePrefab);
                    spawnPlain(DirectionType.Right, wallTransform, sidePrefab);
                    break;
                case DirectionType.Up | DirectionType.Right:
                    spawnCornered(DirectionType.Up | DirectionType.Right, wallTransform, innerCornerPrefab, sidePrefab);
                    break;
                case DirectionType.Up | DirectionType.Left:
                    spawnCornered(DirectionType.Up | DirectionType.Left, wallTransform, innerCornerPrefab, sidePrefab);
                    break;
                case DirectionType.Down | DirectionType.Left:
                    spawnCornered(DirectionType.Down | DirectionType.Left, wallTransform, innerCornerPrefab, sidePrefab);
                    break;
                case DirectionType.Down | DirectionType.Right:
                    spawnCornered(DirectionType.Down | DirectionType.Right, wallTransform, innerCornerPrefab, sidePrefab);
                    break;
                case DirectionType.Left | DirectionType.Up | DirectionType.Right:
                    spawnInpass(DirectionType.Down, wallTransform, innerCornerPrefab, sidePrefab);
                    break;
                case DirectionType.Up | DirectionType.Right | DirectionType.Down:
                    spawnInpass(DirectionType.Left, wallTransform, innerCornerPrefab, sidePrefab);
                    break;
                case DirectionType.Right | DirectionType.Down | DirectionType.Left:
                    spawnInpass(DirectionType.Up, wallTransform, innerCornerPrefab, sidePrefab);
                    break;
                case DirectionType.Down | DirectionType.Left | DirectionType.Up:
                    spawnInpass(DirectionType.Right, wallTransform, innerCornerPrefab, sidePrefab);
                    break;
                case DirectionType.None:
                    fillCorners(cellPosition, level, wallTransform, outerCornerPrefab);
                    break;
            }
        }

        private void fillCorners(Vector2Int cellPosition, GameLevel level, Transform parent, GameObject outerCornerPrefab)
        {
            DirectionType getBorders(Vector2Int coords)
            {
                CellContainer cell = level.Get(cellPosition + Vector2Int.up);
                if (cell == null) return DirectionType.None;
                return cell.Type == CellType.Wall ? getBorderDirections(level, CellType.Wall, coords.x, coords.y) : DirectionType.None;
            }

            var top = getBorders(cellPosition + Vector2Int.up);
            var down = getBorders(cellPosition + Vector2Int.down);
            var right = getBorders(cellPosition + Vector2Int.right);
            var left = getBorders(cellPosition + Vector2Int.left);

            if (top.HasFlag(DirectionType.Right) && right.HasFlag(DirectionType.Up))
                spawnOuterCorner(DirectionType.Right | DirectionType.Up, parent, outerCornerPrefab);

            if (right.HasFlag(DirectionType.Down) && down.HasFlag(DirectionType.Right))
                spawnOuterCorner(DirectionType.Down | DirectionType.Right, parent, outerCornerPrefab);

            if (down.HasFlag(DirectionType.Left) && left.HasFlag(DirectionType.Down))
                spawnOuterCorner(DirectionType.Left | DirectionType.Down, parent, outerCornerPrefab);

            if (top.HasFlag(DirectionType.Left) && left.HasFlag(DirectionType.Up))
                spawnOuterCorner(DirectionType.Left | DirectionType.Up, parent, outerCornerPrefab);
        }

        // private void spawnInpassWall(DirectionType direction, Transform parent)
        // {
        //     spawnInpass(direction, parent, _gameSettings)
        // }
        
        private void spawnInpass(DirectionType direction, Transform parent, GameObject innerCornerPrefab, GameObject sidePrefab)
        {
            //todo: посмотреть можно ли проще
            spawnInnerCorner(DirectionType.Left | DirectionType.Up, parent, innerCornerPrefab);
            spawnInnerCorner(DirectionType.Right | DirectionType.Up, parent, innerCornerPrefab);

            spawnSide(DirectionType.Left, parent, sidePrefab).Translate(down, Space.World);
            spawnSide(DirectionType.Right, parent, sidePrefab).Translate(down, Space.World);

            if (direction == DirectionType.Left)
                parent.Rotate(Vector3.back, 90, Space.Self);
            if (direction == DirectionType.Down)
                parent.Rotate(Vector3.back, 180, Space.Self);
            if (direction == DirectionType.Right)
                parent.Rotate(Vector3.back, 270, Space.Self);
        }

        private void spawnCornered(DirectionType direction, Transform parent, GameObject innerCornerPrefab, GameObject sidePrefab)
        {
            Transform corner = spawnInnerCorner(direction, parent, innerCornerPrefab);
            Vector3 horizontal = corner.right * 0.25f;
            Vector3 vertical = -corner.up * 0.25f;

            // TODO: разобраться как упростить
            if (direction.HasFlag(DirectionType.Right))
                (horizontal, vertical) = (vertical, horizontal);

            if (direction.HasFlag(DirectionType.Down))
                (horizontal, vertical) = (vertical, horizontal);

            var flags = direction.GetFlags().ToArray();

            var horizontalDirection = flags[1];
            var verticalDirection = flags[2];

            Transform wall1 = spawnSide(horizontalDirection, parent, sidePrefab);
            wall1.Translate(vertical, Space.World);
            Transform wall2 = spawnSide(verticalDirection, parent, sidePrefab);
            wall2.Translate(horizontal, Space.World);
        }

        private void spawnPlain(DirectionType pointingDirection, Transform parent, GameObject sidePrefab)
        {
            Transform side1 = spawnSide(pointingDirection, parent, sidePrefab);
            Transform side2 = spawnSide(pointingDirection, parent, sidePrefab);

            if (pointingDirection == DirectionType.Down || pointingDirection == DirectionType.Up)
            {
                side1.Translate(left, Space.World);
                side2.Translate(right, Space.World);
            }

            if (pointingDirection == DirectionType.Left || pointingDirection == DirectionType.Right)
            {
                side1.Translate(up, Space.World);
                side2.Translate(down, Space.World);
            }
        }

        private void spawnSquared(Transform parent, GameObject cornerPrefab)
        {
            spawnInnerCorner(DirectionType.Up | DirectionType.Right, parent, cornerPrefab);
            spawnInnerCorner(DirectionType.Up | DirectionType.Left, parent, cornerPrefab);
            spawnInnerCorner(DirectionType.Down | DirectionType.Right, parent, cornerPrefab);
            spawnInnerCorner(DirectionType.Down | DirectionType.Left, parent, cornerPrefab);
        }

        private Transform spawnOuterCorner(DirectionType pointingDirection, Transform parent, GameObject outerCorner)
        {
            void trySetDirection(DirectionType checkDirection, Transform tile, int angle)
            {
                if (pointingDirection.HasFlag(checkDirection))
                    rotateAndTranslate(tile, pointingDirection, angle);
            }

            var gameObject = GameObject.Instantiate(outerCorner, parent);

            var tileTransform = gameObject.transform;

            trySetDirection(DirectionType.Up | DirectionType.Left, tileTransform, 0);
            trySetDirection(DirectionType.Up | DirectionType.Right, tileTransform, 90);
            trySetDirection(DirectionType.Down | DirectionType.Right, tileTransform, 180);
            trySetDirection(DirectionType.Down | DirectionType.Left, tileTransform, 270);

            return tileTransform;
        }

        private Transform spawnInnerCorner(DirectionType pointingDirection, Transform parent, GameObject cornerPrefab)
        {
            void trySetDirection(DirectionType checkDirection, Transform tile, int angle)
            {
                if (pointingDirection.HasFlag(checkDirection))
                    rotateAndTranslate(tile, pointingDirection, angle);
            }

            var gameObject = GameObject.Instantiate(cornerPrefab, parent);

            var tileTransform = gameObject.transform;

            trySetDirection(DirectionType.Up | DirectionType.Left, tileTransform, 0);
            trySetDirection(DirectionType.Up | DirectionType.Right, tileTransform, 90);
            trySetDirection(DirectionType.Down | DirectionType.Right, tileTransform, 180);
            trySetDirection(DirectionType.Down | DirectionType.Left, tileTransform, 270);

            return tileTransform;
        }

        private Transform spawnSide(DirectionType pointingDirection, Transform parent, GameObject sideSprite)
        {
            void trySetDirection(DirectionType directionType, Transform transform, int angle)
            {
                if (pointingDirection == directionType)
                    rotateAndTranslate(transform, directionType, angle);
            }

            var gameObject = GameObject.Instantiate(sideSprite, parent);

            Transform wallTransform = gameObject.transform;

            trySetDirection(DirectionType.Right, wallTransform, 0);
            trySetDirection(DirectionType.Up, wallTransform, 270);
            trySetDirection(DirectionType.Left, wallTransform, 180);
            trySetDirection(DirectionType.Down, wallTransform, 90);

            return wallTransform;
        }

        private static Transform rotateAndTranslate(Transform tile, DirectionType pointingDirection, float angle)
        {
            tile.Rotate(Vector3.back, angle, Space.Self);
            if (pointingDirection.HasFlag(DirectionType.Up))
                tile.Translate(up, Space.World);
            if (pointingDirection.HasFlag(DirectionType.Right))
                tile.Translate(right, Space.World);
            if (pointingDirection.HasFlag(DirectionType.Down))
                tile.Translate(down, Space.World);
            if (pointingDirection.HasFlag(DirectionType.Left))
                tile.Translate(left, Space.World);

            return tile;
        }

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

        private DirectionType getBorderDirections(GameLevel level, CellType fillingType, int x, int y)
        {
            DirectionType borders = DirectionType.None;
            if (level.Get(x, y)?.Type != fillingType) return borders;

            var leftCell = level.Get(x - 1, y);
            var rightCell = level.Get(x + 1, y);
            var upCell = level.Get(x, y + 1);
            var downCell = level.Get(x, y - 1);

            if (leftCell != null && leftCell.Type != fillingType)
                borders = borders | DirectionType.Left;
            if (rightCell != null && rightCell.Type != fillingType)
                borders = borders | DirectionType.Right;
            if (upCell != null && upCell.Type != fillingType)
                borders = borders | DirectionType.Up;
            if (downCell != null && downCell.Type != fillingType)
                borders = borders | DirectionType.Down;

            return borders;
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
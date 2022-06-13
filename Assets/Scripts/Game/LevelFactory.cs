using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Field.Cell;
using LevelEditor;
using Unity.VisualScripting;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using CellType = LevelEditor.CellType;

namespace Game
{
    public class LevelFactory
    {
        private IObjectResolver _resolver;
        private GameSettings _gameSettings;

        public LevelFactory(IObjectResolver resolver, GameSettings gameSettings)
        {
            _gameSettings = gameSettings;
            _resolver = resolver;
        }

        public Field CreateField(GameLevel level)
        {
            var cells = new Cell[level.MapWidth, level.MapHeight];
            var characterParts = new List<CharacterPart>();

            Field field = _resolver.Instantiate(_gameSettings.FieldPrefab);

            for (int j = 0; j < level.MapHeight; j++)
            {
                for (int i = 0; i < level.MapWidth; i++)
                {
                    Cell newCell = CreateCell(i, j, field.transform, level.Cells[i][j]);
                    cells[i, j] = newCell;
                }
            }

            foreach (var playerPart in level.PlayerParts)
            {
                CharacterPart newCharacterPart = _resolver.Instantiate(_gameSettings.CharacterPartPrefab,
                    new Vector3(playerPart.X, playerPart.Y, -2),
                    Quaternion.identity,
                    field.transform);
                newCharacterPart.Initialize(new Vector2Int(playerPart.X, playerPart.Y), false, field, playerPart.Rotation);
                characterParts.Add(newCharacterPart);
            }

            field.Setup(cells, characterParts);
            return field;
        }

        private Cell CreateCell(int i, int j, Transform parent, CellContainer cellContainer)
        {
            return cellContainer.Type switch
            {
                CellType.Wall => _resolver.Instantiate(_gameSettings.WallCellPrefab, new Vector3(i, j, -1), Quaternion.identity, parent),
                CellType.Empty => _resolver.Instantiate(_gameSettings.EmptyCellPrefab, new Vector3(i, j, -1), Quaternion.identity, parent),
                CellType.Pit => _resolver.Instantiate(_gameSettings.HoleCellPrefab, new Vector3(i, j, -1), Quaternion.identity, parent),
                CellType.Finish => _resolver.Instantiate(_gameSettings.FinishCellPrefab, new Vector3(i, j, -1), Quaternion.identity, parent),
                _ => throw new ArgumentOutOfRangeException(nameof(cellContainer.Type), "Unknown cell type")
            };
        }

        public Character CreateCharacter(GameLevel level, Field field)
        {
            List<CharacterPart> parts = new List<CharacterPart>();
            Character characterParts = _resolver.Instantiate(_gameSettings.CharacterPrefab);

            var playerParts = level.PlayerParts.Where(part => part.IsActive);
            foreach (var part in playerParts)
            {
                CharacterPart characterPart = _resolver.Instantiate(_gameSettings.CharacterPartPrefab,
                    new Vector3(part.X, part.Y, -2), Quaternion.identity);

                setupAbilities(part, field, characterPart);

                CharacterPartMovement characterMovement = setupCharacterMovement(field, characterPart);
                CharacterPartAttachment characterAttachment = setupCharacterAttachment(field, characterPart);

                characterPart.Initialize(new Vector2Int(part.X, part.Y), true, field, part.Rotation);

                characterAttachment.AttachParts();
                parts.Add(characterPart);
            }

            characterParts.AddParts(parts);
            return characterParts;
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
            characterMovement.Initialize(field);
            return characterMovement;
        }

        private static void setupAbilities(PlayerPart part, Field field, CharacterPart characterPart)
        {
            if (part.Ability == PlayerPartType.Hook)
            {
                var pullAbility = characterPart.GetOrAddComponent<PullAbility>();
                pullAbility.Initialize(characterPart, field, directionFromAngle(part.Rotation));
            }
            else if (part.Ability == PlayerPartType.Rotation)
            {
                var rotateAbility = characterPart.GetOrAddComponent<RotateAbility>();
                rotateAbility.Initialize(characterPart, field);
            }
        }

        private static DirectionType directionFromAngle(int partRotation)
        {
            return partRotation switch
            {
                0 | 360 => DirectionType.Up,
                90 | -270 => DirectionType.Right,
                180 | -180 => DirectionType.Down,
                -90 | 270 => DirectionType.Left,
                _ => throw new ArgumentOutOfRangeException(nameof(partRotation), partRotation, "Wrong rotation angle")
            };
        }
    }
}
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
        private PlayerInputs _inputs;

        public LevelFactory(IObjectResolver resolver, GameSettings gameSettings, PlayerInputs inputs)
        {
            _inputs = inputs;
            _gameSettings = gameSettings;
            _resolver = resolver;
        }

        public Field CreateField(GameLevel level)
        {
            var cells = new Cell[level.MapWidth, level.MapHeight];
            var characterParts = new List<CharacterPart>();

            Field field = _resolver.Instantiate(_gameSettings.FieldPrefab);

            field.SetCells(cells);
            for (int j = 0; j < level.MapHeight; j++)
            {
                for (int i = 0; i < level.MapWidth; i++)
                {
                    Cell newCell = CreateCell(i, j, field.transform, level.Cells[j][i]);
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

            field.Setup(characterParts);

            return field;
        }

        private Cell CreateCell(int x, int y, Transform parent, CellContainer cellContainer)
        {
            Cell cell = cellContainer.Type switch
            {
                CellType.Wall => _resolver.Instantiate(_gameSettings.WallCellPrefab, new Vector3(x, y, -1), Quaternion.identity, parent),
                CellType.Empty => _resolver.Instantiate(_gameSettings.EmptyCellPrefab, new Vector3(x, y, -1), Quaternion.identity, parent),
                CellType.Pit => _resolver.Instantiate(_gameSettings.HoleCellPrefab, new Vector3(x, y, -1), Quaternion.identity, parent),
                CellType.Finish => _resolver.Instantiate(_gameSettings.FinishCellPrefab, new Vector3(x, y, -1), Quaternion.identity, parent),
                _ => throw new ArgumentOutOfRangeException(nameof(cellContainer.Type), "Unknown cell type")
            };
            cell.Initialize(new Vector2Int(x, y), cellContainer.Type);
            return cell;
        }

        public Character CreateCharacter(GameLevel level, Field field)
        {
            List<CharacterPart> parts = new List<CharacterPart>();
            Character character = _resolver.Instantiate(_gameSettings.CharacterPrefab);
            character.Initialize(field);
            var inputListener = character.GetOrAddComponent<InputListener>();
            inputListener.Initialize(_inputs, character);
            var playerParts = level.PlayerParts.Where(part => part.IsActive);
            foreach (var part in playerParts)
            {
                CharacterPart characterPart = CreateCharacterPart(field, part);
                characterPart.Initialize(new Vector2Int(part.X, part.Y), true, field, part.Rotation, part.Color);

                characterPart.CharacterPartAttachment.AttachParts();
                parts.Add(characterPart);
            }

            character.AddParts(parts);
            return character;
        }

        private CharacterPart CreateCharacterPart(Field field, CharacterPartData partData)
        {
            CharacterPart characterPart = _resolver.Instantiate(_gameSettings.CharacterPartPrefab,
                new Vector3(partData.X, partData.Y, -2), Quaternion.identity);

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
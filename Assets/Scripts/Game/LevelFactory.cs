using System.Collections.Generic;
using System.Linq;
using Game.Cell;
using Game.Character;
using Game.Systems;
using LevelEditor;
using UnityEngine;
using VContainer;
using CharacterController = Game.Character.CharacterController;

namespace Game
{
    public class LevelFactory
    {
        private readonly CharacterFactory _characterFactory;

        private readonly FieldFactory _fieldFactory;
        private readonly IObjectResolver _resolver;
        private readonly GameSettings _gameSettings;
        private readonly AttachmentSystem _attachmentSystem;

        public LevelFactory(
            IObjectResolver resolver, 
            GameSettings gameSettings, 
            CharacterFactory characterFactory,
            FieldFactory fieldFactory,
            AttachmentSystem attachmentSystem)
        {
            _attachmentSystem = attachmentSystem;
            _gameSettings = gameSettings;
            _resolver = resolver;
            _fieldFactory = fieldFactory;
            _characterFactory = characterFactory;
        }

        public Field CreateField(GameLevel level)
        {
            Field field = _fieldFactory.CreateField(level);
            PaintFinishCells(field.FinishCells, level.FinishColor);
            SetupCamera(level);

            return field;
        }

        public CharacterController CreateCharacter(GameLevel level)
        {
            List<CharacterPart> parts = CreateCharacterParts(level);
            var character = _resolver.Resolve<CharacterController>();
            character.Initialize(parts.First(), _gameSettings.HookRange);
            return character;
        }

        private List<CharacterPart> CreateCharacterParts(GameLevel level)
        {
            List<CharacterPart> activeParts = new();

            foreach (CharacterPartData partData in level.PlayerParts)
            {
                CharacterPartContainer partContainer = _characterFactory.CreateCharacterPart(partData);

                _attachmentSystem.UpdateLinks(partContainer.Part);

                if (partData.IsActive)
                {
                    activeParts.Add(partContainer.Part);
                }
            }

            return activeParts;
        }
        
        private void PaintFinishCells(Cell.Cell[] finishCells, ColorType color)
        {
            foreach (var cell in finishCells)
                cell.GetComponent<FinishView>().SetColor(color);
        }

        private void SetupCamera(GameLevel level)
        {
            Camera.main.transform.position =
                new Vector3(level.MapWidth / 2f - 0.5f, level.MapHeight / 2f - 0.5f, -10);
        }
    }
}
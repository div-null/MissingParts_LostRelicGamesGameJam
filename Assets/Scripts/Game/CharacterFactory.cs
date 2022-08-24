using System;
using System.Collections.Generic;
using System.Linq;
using Game.Character;
using Game.Systems;
using JetBrains.Annotations;
using LevelEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;
using CharacterController = Game.Character.CharacterController;

namespace Game
{
    public class CharacterFactory : IDisposable
    {
        private readonly Field _field;
        private readonly IObjectResolver _resolver;
        private readonly GameSettings _gameSettings;
        private readonly HashSet<CharacterPartContainer> _cachedParts = new();

        public CharacterFactory(IObjectResolver resolver, Field field, GameSettings gameSettings)
        {
            _resolver = resolver;
            _field = field;
            _gameSettings = gameSettings;
        }

        public CharacterController CreateCharacter(GameLevel level)
        {
            List<CharacterPart> parts = new List<CharacterPart>();
            AttachmentSystem attachmentSystem = _resolver.Resolve<AttachmentSystem>();

            var playerParts = level.PlayerParts.Where(part => part.IsActive);
            foreach (CharacterPartData partData in playerParts)
            {
                CharacterPartContainer partContainer = CreateCharacterPart(partData);

                attachmentSystem.UpdateLinks(partContainer.Part);

                parts.Add(partContainer.Part);
                _cachedParts.Add(partContainer);
            }


            var character = _resolver.Resolve<CharacterController>();
            character.Initialize(parts.First(), 4);
            return character;
        }

        public CharacterPartContainer CreateCharacterPart(CharacterPartData partData)
        {
            Vector3 partPosition = _field.Get(partData.X, partData.Y).gameObject.transform.position - Vector3.forward;
            var position = new Vector2Int(partData.X, partData.Y);

            CharacterPartContainer partContainer = _resolver.Instantiate(_gameSettings.CharacterPartPrefab, partPosition, Quaternion.identity);

            partContainer.Part = new CharacterPart(
                position,
                partData.IsActive,
                DirectionFromAngle(partData.Rotation),
                partData.Color,
                partData.Ability);

            _field.Attach(partContainer, position);

            if (partData.Ability == AbilityType.Hook)
            {
                var renderer = partContainer.transform.GetComponentInChildren<SpriteRenderer>();
                HookView hookView = _resolver.Instantiate(_gameSettings.HookPrefab, renderer.transform);
                hookView.Initialize(partContainer.Part);
                partContainer.HookView = hookView;
            }

            partContainer.PartView.Initialize(partContainer.Part, partData.Sprite);

            return partContainer;
        }

        public void Dispose()
        {
            foreach (CharacterPartContainer part in _cachedParts)
                if (part != null && part.gameObject != null)
                    GameObject.Destroy(part.gameObject);

            _cachedParts.Clear();
        }

        private static DirectionType DirectionFromAngle(int partRotation)
        {
            return partRotation switch
            {
                0 => DirectionType.Up,
                360 => DirectionType.Up,
                -90 => DirectionType.Left,
                270 => DirectionType.Left,
                180 => DirectionType.Down,
                -180 => DirectionType.Down,
                90 => DirectionType.Right,
                -270 => DirectionType.Right,
                _ => throw new ArgumentOutOfRangeException(nameof(partRotation), partRotation, "Wrong rotation angle")
            };
        }
    }
}
using System;
using System.Collections.Generic;
using Game.Character;
using LevelEditor;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game
{
    public class CharacterFactory : IDisposable
    {
        private const float PartsLayer = -1;

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

        public CharacterPartContainer CreateCharacterPart(CharacterPartData partData)
        {
            Vector3 spawnPosition = new(partData.X, partData.Y, PartsLayer);
            CharacterPartContainer partContainer = _resolver.Instantiate(_gameSettings.CharacterPartPrefab, spawnPosition, Quaternion.identity);

            var position = new Vector2Int(partData.X, partData.Y);
            partContainer.Part = new CharacterPart(
                position,
                partData.IsActive,
                DirectionExtensions.FromAngle(partData.Rotation),
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

            _cachedParts.Add(partContainer);
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
    }
}
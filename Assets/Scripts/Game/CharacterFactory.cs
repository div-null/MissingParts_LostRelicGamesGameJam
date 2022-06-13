using System.Collections.Generic;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game
{
    public class CharacterFactory
    {
        private Field _field;
        private IObjectResolver _resolver;
        private GameSettings _settings;

        public CharacterFactory(Field field, IObjectResolver resolver, GameSettings settings)
        {
            _settings = settings;
            _resolver = resolver;
            _field = field;
        }

        public Character Create()
        {
            var positions = new[]
            {
                new Vector2Int(1, 1),
                new Vector2Int(2, 1)
            };

            List<CharacterPart> parts = new List<CharacterPart>();
            global::Character character = _resolver.Instantiate(_settings.CharacterPrefab);

            for (int i = 0; i < 2; i++)
            {
                CharacterPart part = _resolver.Instantiate(_settings.CharacterPartPrefab,
                    new Vector3(positions[i].x, positions[i].y, -2), Quaternion.identity);

                part.Initialize(positions[i], true, _field);
                part.TryJoinAllDirections();
                parts.Add(part);
            }

            character.AddParts(parts);
            return character;
        }
    }
}
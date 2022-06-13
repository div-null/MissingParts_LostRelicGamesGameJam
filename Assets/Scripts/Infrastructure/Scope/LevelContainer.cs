using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Field.Cell;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class LevelContainer : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<CharacterFactory>(Lifetime.Scoped);
            builder.Register<LevelFactory>(Lifetime.Scoped);
            builder.RegisterEntryPoint<LevelEntryPoint>();

            builder.Register(container =>
            {
                var levelEntryPoint = container.Resolve<LevelFactory>();
                return levelEntryPoint.Create(8, 8);
            }, Lifetime.Scoped);

            builder.Register(container =>
            {
                var characterFactory = container.Resolve<CharacterFactory>();
                return characterFactory.Create();
            }, Lifetime.Scoped);

            // builder.RegisterEntryPointExceptionHandler(exception =>
            // {
            //     Debug.Log(exception);
            //     Debug.Log(exception.StackTrace);
            // });
        }
    }

    public class LevelEntryPoint : IStartable
    {
        private Field _field;
        private Character _character;

        public LevelEntryPoint(Field field, Character character)
        {
            _character = character;
            _field = field;
        }

        public void Start()
        {
        }
    }

    public class LevelFactory
    {
        private IObjectResolver _resolver;
        private Cell _wallCellPrefab;
        private Cell _emptyCellPrefab;
        private CharacterPart _characterPartPrefab;
        private Field _fieldPrefab;

        public LevelFactory(IObjectResolver resolver, GameSettings gameSettings)
        {
            _fieldPrefab = gameSettings.FieldPrefab;
            _characterPartPrefab = gameSettings.CharacterPartPrefab;
            _emptyCellPrefab = gameSettings.EmptyCellPrefab;
            _wallCellPrefab = gameSettings.WallCellPrefab;
            _resolver = resolver;
        }

        public Field Create(int w, int h)
        {
            var cells = new Cell[w, h];
            var characterParts = new List<CharacterPart>();

            Field field = _resolver.Instantiate(_fieldPrefab);

            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    Cell newCell;
                    if (i == 0 || i == w || j == 0 || j == h)
                        newCell = _resolver.Instantiate(_wallCellPrefab, new Vector3(i, j, -1), Quaternion.identity, field.transform);
                    else
                        newCell = _resolver.Instantiate(_emptyCellPrefab, new Vector3(i, j, -1), Quaternion.identity, field.transform);

                    cells[i, j] = newCell;

                    if (i == 3 && j == 2)
                    {
                        // CharacterPart newCharacterPart = _resolver.Instantiate(_characterPartPrefab, new Vector3(i, j, -2), Quaternion.identity, field.transform);
                        // characterParts.Add(newCharacterPart);
                    }
                }
            }

            field.Setup(cells, characterParts);
            return field;
        }
    }

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

            for (int i = 0; i < 2; i++)
            {
                CharacterPart part = _resolver.Instantiate(_settings.CharacterPartPrefab);

                part.IsActive = true;
                part.Position = positions[i];

                part.TryJoinAllDirections();
                parts.Add(part);
            }

            Character character = _resolver.Instantiate(_settings.CharacterPrefab);
            character.AddParts(parts);
            return character;
        }
    }
}
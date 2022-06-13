﻿using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Game
{
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
                    if (i == 0 || i == w - 1 || j == 0 || j == h - 1)
                        newCell = _resolver.Instantiate(_wallCellPrefab, new Vector3(i, j, -1), Quaternion.identity, field.transform);
                    else
                        newCell = _resolver.Instantiate(_emptyCellPrefab, new Vector3(i, j, -1), Quaternion.identity, field.transform);

                    cells[i, j] = newCell;

                    if (i == 3 && j == 2)
                    {
                        CharacterPart newCharacterPart = _resolver.Instantiate(_characterPartPrefab, new Vector3(i, j, -2), Quaternion.identity, field.transform);
                        newCharacterPart.Initialize(new Vector2Int(i, j), false, field);
                        characterParts.Add(newCharacterPart);
                    }
                }
            }

            field.Setup(cells, characterParts);
            return field;
        }
    }
}
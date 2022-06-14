using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Assets.Scripts.Field.Cell;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using UnityEngine;

namespace LevelEditor
{
    public class LevelLoader
    {
        private JsonSerializerSettings _settings;

        public enum Level
        {
            Lvl1,
            Lvl2,
            Lvl3,
            Lvl4
        }

        // Assets/Resources/Level/Level1.json
        public const string Level1 = "Level/Level1";
        public const string Level2 = "Level/Level2";
        public const string Level3 = "Level/Level3";
        public const string Level4 = "Level/Level4";

        public LevelLoader()
        {
            _settings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Converters = new List<JsonConverter>()
                {
                    new StringEnumConverter()
                    {
                        NamingStrategy = new CamelCaseNamingStrategy()
                    }
                }
            };
        }

        public GameLevel LoadLevel(Level level)
        {
            string levelPath = level switch
            {
                Level.Lvl1 => Level1,
                Level.Lvl2 => Level2,
                Level.Lvl3 => Level3,
                Level.Lvl4 => Level4,
                _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Level does not exist")
            };

            TextAsset levelAsset = Resources.Load(levelPath) as TextAsset;
            return DeserializeLevel(levelAsset.text);
        }

        public string SerializeLevel(GameLevel level)
        {
            var serializedLevel = JsonConvert.SerializeObject(level, Formatting.Indented, _settings);
            return serializedLevel;
        }

        public GameLevel DeserializeLevel(string json)
        {
            var gameLevel = JsonConvert.DeserializeObject<GameLevel>(json, _settings);
            return gameLevel;
        }

        private GameLevel DeserializeLevelOld(string json)
        {
            var gameLevel = JsonUtility.FromJson<SerializedLevel>(json);
            return gameLevel.ToLevel();
        }

        // public void SaveLevel(Level level, GameLevel data)
        // {
        //     string levelPath = level switch
        //     {
        //         Level.Lvl1 => Level1,
        //         Level.Lvl2 => Level2,
        //         Level.Lvl3 => Level3,
        //         Level.Lvl4 => Level4,
        //         _ => throw new ArgumentOutOfRangeException(nameof(level), level, "Level does not exist")
        //     };
        //     TextAsset levelAsset = Resources.Load(levelPath) as TextAsset;
        //     var levelJson = JsonUtility.ToJson(data);
        //     levelAsset.text = levelJson;
        // }

        [Serializable]
        private class SerializedLevel
        {
            public Wrapper<CellJSON>[] cells;
            public int height = 2;
            public int width = 2;
            public string end_point_color;
            public CharacterPartData[] character_parts;
            public Misc[] misc;

            public SerializedLevel(GameLevel level)
            {
                width = level.MapWidth;
                height = level.MapHeight;
                end_point_color = level.FinishColor.ToString().ToLowerInvariant();
                character_parts = level.PlayerParts;
                misc = level.Misc;
                cells = level.Cells
                    .Select(row => new Wrapper<CellJSON>()
                    {
                        row = row
                            .Select(cell => new CellJSON(cell))
                            .ToArray()
                    })
                    .ToArray();
            }

            CellContainer[][] fromRaw(Wrapper<CellJSON>[] raw)
            {
                return raw
                    .Select(row =>
                    {
                        return row?.row
                            .Select(cell => cell.ToCell())
                            .ToArray() ?? Array.Empty<CellContainer>();
                    })
                    .ToArray();
            }

            public GameLevel ToLevel()
            {
                Enum.TryParse<ColorType>(end_point_color, true, out var finishColor);
                return new GameLevel()
                {
                    MapHeight = height,
                    MapWidth = width,
                    FinishColor = finishColor,
                    PlayerParts = character_parts,
                    Misc = misc,
                    Cells = fromRaw(cells)
                };
            }
        }

        [Serializable]
        public class CellJSON
        {
            public string type;

            public CellJSON(CellContainer cell)
            {
                type = cell.Type.ToString().ToLowerInvariant();
            }

            public CellContainer ToCell()
            {
                if (Enum.TryParse<CellType>(type, true, out var cellType))
                    return new CellContainer(cellType);

                throw new SerializationException($"Wrong cell type = \"{type}\"");
            }
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] row;
        }
    }
}
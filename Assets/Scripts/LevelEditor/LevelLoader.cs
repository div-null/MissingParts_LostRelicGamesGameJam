using System;
using System.Linq;
using System.Runtime.Serialization;
using UnityEngine;

namespace LevelEditor
{
    public class LevelLoader
    {
        public enum Level
        {
            Lvl1,
            Lvl2,
            Lvl3,
            Lvl4
        }

        public const string Level1 = ";";
        public const string Level2 = ";";
        public const string Level3 = ";";
        public const string Level4 = ";";

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
            var serializedLevel = new SerializedLevel(level);
            return JsonUtility.ToJson(serializedLevel);
        }

        public GameLevel DeserializeLevel(string json)
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
            public PlayerPart[] character_parts;
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
                        Row = row
                            .Select(cell => new CellJSON(cell))
                            .ToArray()
                    })
                    .ToArray();
            }

            CellContainer[][] fromRaw(Wrapper<CellJSON>[] raw)
            {
                return raw
                    .Select(row => row.Row
                        .Select(cell => cell.ToCell())
                        .ToArray())
                    .ToArray();
            }

            public GameLevel ToLevel()
            {
                CellColor.TryParse<CellColor>(end_point_color, true, out var finishColor);
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
                if (Enum.TryParse<CellType>(type, out var cellType))
                    return new CellContainer(cellType);

                throw new SerializationException("Wrong cell type");
            }
        }

        [Serializable]
        private class Wrapper<T>
        {
            public T[] Row;
        }
    }
}
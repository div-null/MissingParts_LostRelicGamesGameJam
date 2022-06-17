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
            Lvl4,
            Lvl5,
            Lvl6,
            Lvl7
        }

        // Assets/Resources/Level/Level1.json
        public const string Level1 = "Level/Level1";
        public const string Level2 = "Level/Level2";
        public const string Level3 = "Level/Level3";
        public const string Level4 = "Level/Level4";
        public const string Level5 = "Level/Level5";
        public const string Level6 = "Level/Level6";
        public const string Level7 = "Level/Level7";

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
                Level.Lvl5 => Level5,
                Level.Lvl6 => Level6,
                Level.Lvl7 => Level7,
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
    }
}
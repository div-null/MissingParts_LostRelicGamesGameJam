using System;
using System.Collections.Generic;
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
            Unknown,
            Lvl1,
            Lvl2,
            Lvl3,
            Lvl4,
            Lvl5,
            Lvl6,
            Lvl7,
            Lvl8,
            Lvl9,
            Lvl10,
            Lvl11,
            Lvl12,
            Lvl13,
            Lvl14,
            Lvl15,
            Lvl16,
            Lvl17,
            Lvl18,
            Lvl21,
        }

        // Assets/Resources/Level/Level1.json
        public const string Level1 = "Level/Level1";
        public const string Level2 = "Level/Level2";
        public const string Level3 = "Level/Level3";
        public const string Level4 = "Level/Level4";
        public const string Level5 = "Level/Level5";
        public const string Level6 = "Level/Level6";
        public const string Level7 = "Level/Level7";
        public const string Level8 = "Level/Level8";
        public const string Level9 = "Level/Level9";
        public const string Level10 = "Level/Level10";
        public const string Level11 = "Level/Level11";
        public const string Level12 = "Level/Level12";
        public const string Level13 = "Level/Level13";
        public const string Level14 = "Level/Level14";
        public const string Level15 = "Level/Level15";
        public const string Level16 = "Level/Level16";

        public const string Level17 = "Level/Level17";

        //public const string Level18 = "Level/Level18";
        public const string Level21 = "Level/Level21";

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
                Level.Lvl8 => Level8,
                Level.Lvl9 => Level9,
                Level.Lvl10 => Level10,
                Level.Lvl11 => Level11,
                Level.Lvl12 => Level12,
                Level.Lvl13 => Level13,
                Level.Lvl14 => Level14,
                Level.Lvl15 => Level15,
                Level.Lvl16 => Level16,
                Level.Lvl17 => Level17,
                //Level.Lvl18=> Level18,
                Level.Lvl21 => Level21,
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
            var level = JsonConvert.DeserializeObject<LevelData>(json, _settings);
            return new GameLevel(level);
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
using Game;
using LevelEditor;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class LevelEntryPoint : IStartable
    {
        private LevelFactory _factory;
        private int _level;
        private LevelLoader _levelLoader;

        public LevelEntryPoint(LevelFactory factory, LevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
            _factory = factory;
            _level = 1;
        }

        public void Start()
        {
        }

        public void LoadNextLevel()
        {
            GameLevel level = _level switch
            {
                1 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl1),
                2 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl2),
                3 => _levelLoader.LoadLevel(LevelLoader.Level.Lvl3),
            };
            Field field = _factory.CreateField(level);
            Character character = _factory.CreateCharacter(level, field);
        }
    }
}
using Game;
using VContainer.Unity;

namespace Infrastructure.Scope
{
    public class LevelEntryPoint : IStartable
    {
        private readonly GameController _gameController;


        public LevelEntryPoint(GameController gameController)
        {
            _gameController = gameController;
        }

        public void Start()
        {
            _gameController.Start();
        }

    }
}
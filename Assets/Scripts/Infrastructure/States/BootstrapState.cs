using LevelEditor;
using UnityEngine;

namespace Infrastructure.States
{
    public class BootstrapState : IState
    {
        private SceneLoader _loader;
        private LevelLoader _levelLoader;

        public BootstrapState(SceneLoader loader, LevelLoader levelLoader)
        {
            _levelLoader = levelLoader;
            _loader = loader;
        }

        public void Exit()
        {
            Debug.Log("Exited Boostrap state");
            throw new System.NotImplementedException();
        }

        public void Enter()
        {
            Debug.Log("Entered Boostrap state");
            var gameLevel = new GameLevel();
            gameLevel.PlayerParts = new[]
            {
                new PlayerPart()
                {
                    Ability = PlayerPartType.Common, Color = CellColor.Red, X = 0, Y = 0, Sprite = 1, Rotation = 0,
                    IsActive = true
                }
            };
            string json = _levelLoader.SerializeLevel(gameLevel);
            Debug.Log(json);
        }
    }
}
using Game.Cell;
using Game.Character;
using Infrastructure.Scope;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Create settings", order = 0)]
public class GameSettings : ScriptableObject
{
    public Cell WallCellPrefab;
    public Cell EmptyCellPrefab;
    public Cell PitCellPrefab;
    public Cell FinishCellPrefab;
    public CharacterPartContainer CharacterPartPrefab;
    public GameLifetimeScope GameLifetimeScope;
    public GameContainer LevelContainer;
    public MenuLifetime MenuLifetime;
    public HookView HookPrefab;
}
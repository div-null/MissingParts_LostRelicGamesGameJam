using Game.Character;
using Game.Level;
using Infrastructure.Scope;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Create settings", order = 0)]
public class GameSettings : ScriptableObject
{
    [Header("Prefabs")]
    public Cell WallCellPrefab;
    public Cell EmptyCellPrefab;
    public Cell PitCellPrefab;
    public Cell FinishCellPrefab;
    public CharacterPartContainer CharacterPartPrefab;
    public GameLifetimeScope GameLifetimeScope;
    public GameContainer LevelContainer;
    public MenuLifetime MenuLifetime;
    public HookView HookPrefab;

    [Header("Prefabs")]
    public int HookRange = 4;
}
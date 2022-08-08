using Game;
using Game.Cell;
using Game.Character;
using Infrastructure.Scope;
using UnityEngine;
using CharacterController = Game.Character.CharacterController;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Create settings", order = 0)]
public class GameSettings : ScriptableObject
{
    // public PitCellPrefab PitCellPrefabs;
    // public WallCellPrefab WallCellPrefabs;
    public Cell WallCellPrefab;
    public Cell EmptyCellPrefab;
    public Cell PitCellPrefab;
    public Cell FinishCellPrefab;
    public CharacterPartContainer CharacterPartPrefab;
    public Field FieldPrefab;
    public CharacterController CharacterPrefab;
    public GameLifetimeScope GameLifetimeScope;
    public LevelContainer LevelContainer;
    public MenuLifetime MenuLifetime;
    public HookView HookPrefab;
}
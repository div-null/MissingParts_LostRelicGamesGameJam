using Assets.Scripts.Field.Cell;
using Infrastructure.Scope;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Create settings", order = 0)]
public class GameSettings : ScriptableObject
{
    // public PitCellPrefab PitCellPrefabs;
    // public WallCellPrefab WallCellPrefabs;
    public Cell WallCellPrefab;
    public Cell EmptyCellPrefab;
    public Cell PitCellPrefab;
    public Cell FinishCellPrefab;
    public CharacterPart CharacterPartPrefab;
    public Field FieldPrefab;
    public Character CharacterPrefab;
    public GameLifetimeScope GameLifetimeScope;
    public LevelContainer LevelContainer;
    public MenuLifetime MenuLifetime;
    public HookView HookPrefab;
}
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using Infrastructure.Scope;
using UnityEngine;
using VContainer.Unity;

[CreateAssetMenu(fileName = "Create settings", order = 0)]
public class GameSettings : ScriptableObject
{
    public Cell WallCellPrefab;
    public Cell EmptyCellPrefab;
    public Cell HoleCellPrefab;
    public CharacterPart CharacterPartPrefab;
    public Field FieldPrefab;
    public Character CharacterPrefab;
    public GameLifetimeScope GameLifetimeScope;
    public LevelContainer LevelContainer;
    public MenuLifetime MenuLifetime;
}
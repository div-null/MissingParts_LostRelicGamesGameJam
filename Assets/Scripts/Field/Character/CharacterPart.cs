using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using JetBrains.Annotations;
using LevelEditor;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterPart : IEnumerable<CharacterPart>
{
    public readonly ReactiveCommand Deleted = new();
    public readonly IObservable<ColorType> ColorChanged;
    public readonly IObservable<Vector2Int> PositionChanged;
    public readonly IObservable<DirectionType> LookChanged;
    public readonly IObservable<bool> IsActiveChanged;

    public AbilityType Ability { get; private set; }

    public ColorType Color
    {
        get => _color.Value;
        set => _color.Value = value;
    }

    public Vector2Int Position
    {
        get => _position.Value;
        set => _position.Value = value;
    }

    public DirectionType Look
    {
        get => _look.Value;
        set => _look.Value = value;
    }

    public bool IsActive
    {
        get => _isActive.Value;
        set => _isActive.Value = value;
    }

    public CharacterPart Right { get; private set; }
    public CharacterPart Left { get; private set; }
    public CharacterPart Up { get; private set; }
    public CharacterPart Down { get; private set; }

    private readonly ReactiveProperty<ColorType> _color = new();
    private readonly ReactiveProperty<Vector2Int> _position = new();
    private readonly ReactiveProperty<DirectionType> _look = new();
    private readonly ReactiveProperty<bool> _isActive = new();


    public CharacterPart()
    {
        ColorChanged = _color.AsObservable();
        LookChanged = _look.AsObservable();
        PositionChanged = _position.AsObservable();
        IsActiveChanged = _isActive.AsObservable();
    }

    public void Initialize(Vector2Int position, bool isActive, DirectionType lookDirection, ColorType color)
    {
        Position = position;
        Color = color;
        Look = lookDirection;
        IsActive = isActive;
    }

    public void Rotate()
    {
        //change sprite rotation
        RotateLinks();
        Look = Look.RotateRight();
    }

    public void SetActiveToAllParts(bool isActive)
    {
        //Обойти все части characterPart'а и изменить им Active
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        void visitNode(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);
            part.IsActive = isActive;

            visitNode(part.Down);
            visitNode(part.Up);
            visitNode(part.Right);
            visitNode(part.Left);
        }

        visitNode(this);
    }

    public void SetColorToAllParts(ColorType color)
    {
        //Обойти все части characterPart'а и изменить им Active
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        void visitNode(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);
            part.Color = color;

            visitNode(part.Down);
            visitNode(part.Up);
            visitNode(part.Right);
            visitNode(part.Left);
        }

        visitNode(this);
    }

    public bool IsLeaf() =>
        Right == null || Left == null || Up == null || Down == null;

    public void Delete()
    {
        RemoveLinks();
        Deleted.Execute();
        Debug.Log("destroying part");
    }

    public void RemoveLinks()
    {
        RemoveLinkInDirection(DirectionType.Up);
        RemoveLinkInDirection(DirectionType.Right);
        RemoveLinkInDirection(DirectionType.Down);
        RemoveLinkInDirection(DirectionType.Left);
    }

    public CharacterPart GetPartFromDirection(DirectionType direction)
    {
        return direction switch
        {
            DirectionType.Right => Right,
            DirectionType.Left => Left,
            DirectionType.Up => Up,
            DirectionType.Down => Down,
            _ => throw new ArgumentOutOfRangeException(nameof(direction), direction, "Can't map invalid direction to CharacterPart")
        };
    }

    public bool HasPartInDirection(DirectionType direction) =>
        GetPartFromDirection(direction) != null;

    private void RotateLinks()
    {
        CharacterPart temp = Up;
        Up = Left;
        Left = Down;
        Down = Right;
        Right = temp;
    }

    public void Join(CharacterPart part, bool setActive = true)
    {
        Vector2Int joinPosition = part.Position - Position;
        Assert.IsTrue(joinPosition.magnitude == 1);

        SetLinkInDirection(part, joinPosition.ToDirection());

        SetActiveToAllParts(setActive);

        if (Color != part.Color)
            SetColorToAllParts(part.Color);
    }

    public void RemoveLinkInDirection(DirectionType direction)
    {
        CharacterPart removedPart = this.GetPartFromDirection(direction);

        RemoveLink(this, direction);
        RemoveLink(removedPart, direction.InvertSingle());
    }

    private void SetLinkInDirection(CharacterPart part, DirectionType direction)
    {
        switch (direction)
        {
            case DirectionType.Left:
            {
                Left = part;
                part.Right = this;
                break;
            }
            case DirectionType.Right:
            {
                Right = part;
                part.Left = this;
                break;
            }
            case DirectionType.Up:
            {
                Up = part;
                part.Down = this;

                break;
            }
            case DirectionType.Down:
            {
                Down = part;
                part.Up = this;
                break;
            }
        }
    }

    private static void RemoveLink(CharacterPart part, DirectionType direction)
    {
        switch (direction)
        {
            case DirectionType.Up:
            {
                part.Up = null;
                break;
            }
            case DirectionType.Right:
            {
                part.Right = null;
                break;
            }
            case DirectionType.Down:
            {
                part.Down = null;
                break;
            }
            case DirectionType.Left:
            {
                part.Left = null;
                break;
            }
        }
    }

    public IEnumerator<CharacterPart> GetEnumerator()
    {
        HashSet<CharacterPart> returned = new();
        HashSet<CharacterPart> visited = new();


        CharacterPart? nextValue = NextRecursive(this);

        while (true)
        {
            yield return nextValue!;

            visited.Clear();
            nextValue = AnyNeighbourNode(nextValue!) ?? NextRecursive(nextValue!);
            if (nextValue != null)
                returned.Add(nextValue);
            else
                yield break;
        }


        CharacterPart? NextRecursive(CharacterPart? part)
        {
            if (part == null) return null;
            if (visited.Contains(part)) return null;
            visited.Add(part);

            if (!returned.Contains(part))
                return part;

            return NextRecursive(part.Left)
                   ?? NextRecursive(part.Right)
                   ?? NextRecursive(part.Up)
                   ?? NextRecursive(part.Down);
        }

        CharacterPart? AnyNeighbourNode(CharacterPart part) =>
            (!returned.Contains(part.Left) ? part.Left : null) ??
            (!returned.Contains(part.Up) ? part.Up : null) ??
            (!returned.Contains(part.Right) ? part.Right : null) ??
            (!returned.Contains(part.Down) ? part.Down : null);
    }

    IEnumerator IEnumerable.GetEnumerator() =>
        GetEnumerator();
}
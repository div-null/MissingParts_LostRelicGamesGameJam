using System;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UniRx;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterPart
{
    public readonly ReactiveCommand Deleted = new();
    public readonly IObservable<ColorType> ColorChanged;
    public readonly IObservable<Vector2Int> PositionChanged;
    public readonly IObservable<DirectionType> LookChanged;
    public readonly IObservable<bool> IsActiveChanged;
    
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

    public CharacterPart Right;
    public CharacterPart Left;
    public CharacterPart Up;
    public CharacterPart Down;

    private readonly ReactiveProperty<ColorType> _color = new();
    private readonly ReactiveProperty<Vector2Int> _position = new();
    private readonly ReactiveProperty<DirectionType> _look = new();
    private readonly ReactiveProperty<bool> _isActive = new();

    
    private Field _field;
    public CharacterPartMovement CharacterPartMovement;
    public CharacterPartAttachment CharacterPartAttachment;


    public CharacterPart()
    {
        ColorChanged = _color.AsObservable();
        LookChanged = _look.AsObservable();
        PositionChanged = _position.AsObservable();
        IsActiveChanged = _isActive.AsObservable();
    }
    
    public void Initialize(Vector2Int position, bool isActive, Field field, DirectionType lookDirection, ColorType color)
    {
        _field = field;
        Position = position;
        Color = color;
        Look = lookDirection;
        IsActive = isActive;
    }

    public void SetPosition(Vector2Int destination)
    {
        _field.Get(Position).RemoveCharacterPart(this);
        _field.Get(destination).AssignCharacterPart(this);

        Position = destination;
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


    public void TryJoin(DirectionType direction)
    {
        var checkPosition = Position + direction.ToVector2Int();
        var characterPart = _field.Get(checkPosition)?.CharacterPart;
        if (characterPart != null)
        {
            Join(characterPart, IsActive);
        }
        else
        {
            RemoveLinkInDirection(direction);
        }
    }

    public void TryJoinAllDirections()
    {
        TryJoin(DirectionType.Down);
        TryJoin(DirectionType.Up);
        TryJoin(DirectionType.Left);
        TryJoin(DirectionType.Right);
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

    public void Delete()
    {
        _field.Get(Position).RemoveCharacterPart(this);
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

    private void RotateLinks()
    {
        CharacterPart temp = Up;
        Up = Left;
        Left = Down;
        Down = Right;
        Right = temp;
    }

    public bool HasPartInDirection(DirectionType direction) =>
        GetPartFromDirection(direction) != null;

    public bool HasRightShape(HashSet<CharacterPart> visitedParts)
    {
        bool visitNode(CharacterPart part)
        {
            if (part == null) return true;
            if (visitedParts.Contains(part)) return true;
            visitedParts.Add(part);

            Cell cell = _field.Get(part.Position);
            if (cell != null && !cell.IsFinish())
            {
                return false;
            }

            return visitNode(part.Down) && visitNode(part.Up) && visitNode(part.Right) && visitNode(part.Left);
        }

        return visitNode(this);
    }

    private void RemoveLinkInDirection(DirectionType direction)
    {
        switch (direction)
        {
            case DirectionType.Up:
            {
                if (Up != null)
                    Up.Down = null;

                Up = null;
                break;
            }
            case DirectionType.Right:
            {
                if (Right != null)
                    Right.Left = null;

                Right = null;
                break;
            }
            case DirectionType.Down:
            {
                if (Down != null)
                    Down.Up = null;

                Down = null;
                break;
            }
            case DirectionType.Left:
            {
                if (Left != null)
                    Left.Right = null;

                Left = null;
                break;
            }
        }
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
}
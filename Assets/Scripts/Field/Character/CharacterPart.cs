using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;
using UnityEngine.Assertions;
using VContainer;

public class CharacterPart : MonoBehaviour
{
    public CharacterPart Right;
    public CharacterPart Left;
    public CharacterPart Up;
    public CharacterPart Down;

    public ColorType Color;
    public Vector2Int Position;
    public int Rotation;

    public bool IsActive;
    private Field _field;

    public CharacterPartMovement CharacterPartMovement;
    public CharacterPartAttachment CharacterPartAttachment;

    public void Initialize(Vector2Int position, bool isActive, Field field, int rotation, ColorType color)
    {
        Position = position;
        Rotation = rotation;
        IsActive = isActive;
        Color = color;
        _field = field;
    }

    public void SetActive(bool isActive)
    {
        //change active to this character part
        IsActive = isActive;
    }

    public bool IsLeaf() =>
        Right == null || Left == null || Up == null || Down == null;

    
    public void TryJoin(DirectionType direction)
    {
        if (GetPartFromDirection(direction) == null)
        {
            var checkPosition = Position + direction.ToVector();
            var characterPart = _field.Get(checkPosition)?.CharacterPart;
            if (characterPart != null)
                Join(characterPart, IsActive);
        }
    }
    
    public void Join(CharacterPart part, bool setActive = true)
    {
        Vector2Int joinPosition = part.Position - Position;
        Assert.IsTrue(joinPosition.magnitude == 1);

        switch (joinPosition.ToDirection())
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
            default:
            {
                Down = part;
                part.Up = this;
                break;
            }
        }

        part.SetActive(setActive);
    }

    public void SetActiveToAllParts(CharacterPart characterPart, bool isActive)
    {
        //Обойти все части characterPart'а и изменить им Active
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        void visitNode(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);
            part.SetActive(isActive);

            visitNode(part.Down);
            visitNode(part.Up);
            visitNode(part.Right);
            visitNode(part.Left);
        }

        visitNode(characterPart);
    }

    public CharacterPart GetPartFromDirection(DirectionType direction)
    {
        return direction switch
        {
            DirectionType.Right => Right,
            DirectionType.Left => Left,
            DirectionType.Up => Up,
            DirectionType.Down => Down
        };
    }
    
    public CharacterPart GetPartFromDirection(float degrees)
    {
        int direction = (int)(degrees % 90);
        return direction switch
        {
            0 => Up,
            1 => Left,
            2 => Down,
            3 => Right
        };
    }

    public void SetPosition(Vector2Int destination)
    {
        Position = destination;
        Debug.Log($"New position {Position}");
        //TODO: set transform
    }

    public bool HasPartInDirection(DirectionType direction)
    {
        return GetPartFromDirection(direction) != null;
    }

    public void SetRotation()
    {
        //change sprite rotation
        Rotation = (Rotation + 270) % 360;
    }
}
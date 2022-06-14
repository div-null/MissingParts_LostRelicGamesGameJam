using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterPartView))]
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

    public CharacterPartView CharacterPartView;
    
    public void Initialize(Vector2Int position, bool isActive, Field field, int rotation, ColorType color)
    {
        Position = position;
        SetRotation(rotation);
        SetActive(isActive);
        Color = color;
        _field = field;
    }

    public void SetActive(bool isActive)
    {
        //change active to this character part
        IsActive = isActive;
        CharacterPartView.SetActive(isActive);
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
            {
                Join(characterPart, IsActive);
            }
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

        SetActiveToAllParts(setActive);
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
            part.SetActive(isActive);

            visitNode(part.Down);
            visitNode(part.Up);
            visitNode(part.Right);
            visitNode(part.Left);
        }

        visitNode(this);
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

    public CharacterPart[] GetPartsFromDirections()
    {
        List<CharacterPart> characterParts = new List<CharacterPart>();
        if (Up != null) characterParts.Add(Up);
        if (Right != null) characterParts.Add(Right);
        if (Down != null) characterParts.Add(Down);
        if (Left != null) characterParts.Add(Left);

        return characterParts.ToArray();
    }

    public void SetPosition(Vector2Int destination)
    {
        _field.Get(Position).RemoveCharacterPart();
        _field.Get(destination).AssignCharacterPart(this);
        
        Position = destination;
        //TODO: set transform
        this.transform.position = _field.Get(destination).gameObject.transform.position - Vector3.forward;
    }
    
    public void SetRotation()
    {
        //change sprite rotation
        SetRotation((Rotation + 270) % 360);
    }
    
    public void SetRotation(int degrees)
    {
        //change sprite rotation
        Rotation = degrees;
        
    }

    public bool HasPartInDirection(DirectionType direction)
    {
        return GetPartFromDirection(direction) != null;
    }

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

    public void RemoveLinkWith(CharacterPart characterPart)
    {
        Vector2Int directionVector = characterPart.Position - Position;
        switch (directionVector.ToDirection())
        {
            case DirectionType.Up:
            {
                Up = null;
                break;
            }
            case DirectionType.Right:
            {
                Right = null;
                break;
            }  
            case DirectionType.Down:
            {
                Down = null;
                break;
            }
            default:
            {
                Left = null;
                break;
            }
        }
    }

    public void Delete()
    {
        Destroy(this.gameObject);
        Debug.Log("destroying part");
        //Delete after entering the pit
    }

    public void RemoveLinks()
    {
        if (Right != null)
        {
            Right.Left = null;
            Right = null;
        }
        
        if (Left != null)
        {
            Left.Right = null;
            Left = null;
        }

        if (Up != null)
        {
            Up.Down = null;
            Up = null;
        }

        if (Down != null)
        {
            Down.Up = null;
            Down = null;
        }
    }
}
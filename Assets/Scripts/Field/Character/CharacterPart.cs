using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using static DG.Tweening.DOTween;

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

    public void SetPosition(Vector2Int destination)
    {
        _field.Get(Position).RemoveCharacterPart(this);
        _field.Get(destination).AssignCharacterPart(this);

        Position = destination;
        //TODO: set transform
        Vector3 newPosition = _field.Get(destination).gameObject.transform.position - Vector3.forward;
        this.transform.DOMove(newPosition, 0.1f).SetEase(Ease.Flash).onComplete += TweenCallback;
    }

    public void SetRotation()
    {
        //change sprite rotation
        RotateLinks();
        SetRotation((Rotation + 90) % 360);
    }

    public void SetRotation(int degrees)
    {
        //change sprite rotation
        Rotation = degrees;
        CharacterPartView.SetRotation(degrees);
    }

    public void SetActive(bool isActive)
    {
        //change active to this character part
        IsActive = isActive;
        CharacterPartView.SetActive(isActive);
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

    public void SetColor(ColorType color)
    {
        Color = color;
        CharacterPartView.SetColor(color);
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
            part.SetColor(color);

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
        var checkPosition = Position + direction.ToVector();
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
        int direction = (int) (degrees % 90);
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

    private void TweenCallback()
    {
        Debug.Log("Moved!");
    }

    public void OnMoved(TweenCallback tweenCallback)
    {
    }

    public void RotateLinks()
    {
        CharacterPart temp = Up;
        Up = Left;
        Left = Down;
        Down = Right;
        Right = temp;
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

    public void RemoveLinkInDirection(DirectionType direction)
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
            default:
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
            default:
            {
                Down = part;
                part.Up = this;
                break;
            }
        }
    }

    public void Delete()
    {
        _field.Get(Position).RemoveCharacterPart(this);
        RemoveLinks();
        Quaternion quaternion = Quaternion.Euler(0, 0, -180);
        transform.DORotate(quaternion.eulerAngles, 0.25f, RotateMode.Fast).SetLoops(3).SetEase(Ease.Linear);
        transform.DOScale(0.01f, 0.5f).SetEase(Ease.Linear);
        Destroy(this.gameObject, 0.6f);
        Debug.Log("destroying part");
    }

    public void RemoveLinks()
    {
        RemoveLinkInDirection(DirectionType.Up);
        RemoveLinkInDirection(DirectionType.Right);
        RemoveLinkInDirection(DirectionType.Down);
        RemoveLinkInDirection(DirectionType.Left);
    }
}
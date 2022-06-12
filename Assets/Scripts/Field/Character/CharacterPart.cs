using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;
using UnityEngine.Assertions;

public class CharacterPart : MonoBehaviour
{
    public CharacterPart Right;
    public CharacterPart Left;
    public CharacterPart Up;
    public CharacterPart Down;

    public Vector2Int Position;
    public bool IsActive;
    public bool IsMoving;

    public void TurnOn()
    {
        //turn on this character part and all parts from graph
    }

    public void TurnOff()
    {
        //turn off this character part and all parts from graph
    }

    public bool IsLeaf() =>
        Right == null || Left == null || Up == null || Down == null;

    public void Join(CharacterPart part)
    {
        Vector2Int joinPosition = part.Position - Position;
        Assert.IsTrue(joinPosition.magnitude == 1);

        if (joinPosition == Vector2Int.left)
        {
            Left = part;
            part.Right = this;
            part.IsActive = true;
        }
        else if (joinPosition == Vector2Int.right)
        {
            Right = part;
            part.Left = this;
            part.IsActive = true;
        }
        else if (joinPosition == Vector2Int.up)
        {
            Up = part;
            part.Down = this;
            part.IsActive = true;
        }
        else if (joinPosition == Vector2Int.down)
        {
            Down = part;
            part.Up = this;
            part.IsActive = true;
        }
        else
        {
            Debug.LogError("Can't join parts");
        }
    }

    // public bool CanMove(DirectionType direction)
    // {
    //     CharacterPart partFromSide;
    //     partFromSide = GetPartFromDirection(direction);
    //
    //     if (partFromSide == null)
    //     {
    //         Vector2Int directionVector = GetVectorFromDirection(direction);
    //         Cell foundCell = Field.Instance.GetCell(Position + directionVector);
    //         if (foundCell.CellType == CellType.Wall)
    //             return false;
    //         else
    //             return true;
    //     }
    //     else
    //     {
    //         return partFromSide.CanMove(direction);
    //     }
    // }

    // public void Move(DirectionType direction)
    // {
    //     CharacterPart partFromSide;
    //     partFromSide = GetPartFromDirection(direction);
    //
    //     if (partFromSide == null)
    //     {
    //         Vector2Int directionVector = direction.ToVector();
    //         Cell foundCell = Field.GetCell(Position + directionVector);
    //         CharacterPart foundCharacterPart = foundCell.CharacterPart;
    //         if (foundCharacterPart != null)
    //             //attach foundCharacterPart to this part
    //             //Left/Right = ...
    //     }
    // }

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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetPosition(Vector2Int destination)
    {
        Position = destination;
        Debug.Log($"New position {Position}");
    }
}
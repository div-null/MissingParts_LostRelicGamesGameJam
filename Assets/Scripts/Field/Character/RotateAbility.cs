using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;


public class RotateAbility : Ability
{
    private List<Vector2Int> newPositions;

    public override void Apply()
    {
        //TODO: actions and may be something else
        TryToRotate();
    }

    public bool TryToRotate()
    {
        //Up -> Right, Right -> Down, Down -> Left, Left -> Up
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();
        Dictionary<CharacterPart, Vector2Int> partsWithNewPositions = new Dictionary<CharacterPart, Vector2Int>();
        
        bool canRotate(CharacterPart part, Vector2Int sourcePosition, Vector2Int deltaPosition)
        {
            if (part == null) return true;
            if (visited.Contains(part)) return true;
            visited.Add(part);
            //d = p2 - p1
            //x1 + dy, y1 - dx
            Vector2Int newPosition = new Vector2Int(sourcePosition.x + deltaPosition.y, sourcePosition.y - deltaPosition.x);
            partsWithNewPositions.Add(part, newPosition);
            Cell cell = _field.Get(newPosition);
            
            if (cell != null && cell.CellType == CellType.Wall)
                return false;

            //TODO: Check for walls that located near the part
            
            return canRotate(part.Left, sourcePosition, deltaPosition + Vector2Int.left) &&
                   canRotate(part.Right, sourcePosition, deltaPosition + Vector2Int.right) &&
                   canRotate(part.Up, sourcePosition, deltaPosition + Vector2Int.up) &&
                   canRotate(part.Down, sourcePosition, deltaPosition + Vector2Int.down);
        }

        if (canRotate(_characterPart, _characterPart.Position, Vector2Int.zero))
        {
            foreach(var part in partsWithNewPositions)
            {
                part.Key.SetPosition(part.Value);
                part.Key.SetRotation();
            }
            
            return true;
        }
        else
        {
            return false;
        }
    }
}

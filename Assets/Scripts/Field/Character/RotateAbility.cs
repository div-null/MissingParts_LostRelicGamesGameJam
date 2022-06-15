using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        bool canRotate(CharacterPart movingPart, Vector2Int rotationCenter)
        {
            if (movingPart == null) return true;
            if (visited.Contains(movingPart)) return true;

            visited.Add(movingPart);
            //d = p2 - p1
            //x1 + dy, y1 - dx
            Vector2Int relativeCoordinates = movingPart.Position - rotationCenter;

            Vector2Int newRelativePosition = new Vector2Int(relativeCoordinates.y, -relativeCoordinates.x);
            Vector2Int newPosition = newRelativePosition + rotationCenter;

            partsWithNewPositions.Add(movingPart, newPosition);
            Cell cell = _field.Get(newPosition);

            if (cell != null)
            {
                if (cell.IsWall())
                    return false;
                else if (cell.CharacterPart != null && !cell.CharacterPart.IsActive)
                    return false;
            }

            //TODO: Check for walls that located near the part

            return canRotate(movingPart.Left, rotationCenter) &&
                   canRotate(movingPart.Right, rotationCenter) &&
                   canRotate(movingPart.Up, rotationCenter) &&
                   canRotate(movingPart.Down, rotationCenter);
        }

        if (canRotate(_characterPart, _characterPart.Position))
        {
            foreach (var (part, value) in partsWithNewPositions)
            {
                part.SetPosition(value);
                part.SetRotation();
            }

            _characterPart.CharacterPartAttachment.DetachParts();
            _characterPart.CharacterPartAttachment.AttachParts();

            return true;
        }
        else
        {
            return false;
        }
    }
}
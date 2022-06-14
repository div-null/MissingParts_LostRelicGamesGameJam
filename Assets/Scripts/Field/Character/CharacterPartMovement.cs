using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;

public class CharacterPartMovement: MonoBehaviour
{
    private CharacterPart _characterPart;
    private Field _field;

    public void Initialize(Field field, CharacterPart characterPart)
    {
        _field = field;
        _characterPart = characterPart;
    }
    
    public bool CanMove(CharacterPart characterPart, DirectionType direction)
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        bool visitNode(CharacterPart part)
        {
            if (part == null) return true;
            if (visited.Contains(part)) return true;
            visited.Add(part);
            
            var destination = part.Position + direction.ToVector();
            if (HasWallInCell(destination)) return false;

            return visitNode(part.Left) && visitNode(part.Right) && visitNode(part.Up) && visitNode(part.Down);
        }

        return visitNode(characterPart);
    }

    public bool CanMove(CharacterPart characterPart, Vector2Int deltaPosition)
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        bool visitNode(CharacterPart part)
        {
            if (part == null) return true;
            if (visited.Contains(part)) return true;
            visited.Add(part);
            
            var destination = part.Position + deltaPosition;
            if (HasWallInCell(destination)) return false;

            return visitNode(part.Left) && visitNode(part.Right) && visitNode(part.Up) && visitNode(part.Down);
        }

        return visitNode(characterPart);
    }
    
    public bool Move(DirectionType direction)
    {
        if (!CanMove(_characterPart, direction)) return false;

        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        void visitNode(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);
            var destination = part.Position + direction.ToVector();
            part.SetPosition(destination);

            visitNode(part.Left);
            visitNode(part.Right);
            visitNode(part.Up);
            visitNode(part.Down);
        }

        visitNode(_characterPart);
        return true;
    }

    private bool HasWallInCell(Vector2Int position)
    {
        Cell cell = _field.Get(position);
        
        if (cell != null && cell.IsWall())
            return true;
        
        return false;
    }
    
    private bool HasPitInCell(Vector2Int position)
    {
        Cell cell = _field.Get(position);
        
        if (cell != null && cell.IsPit())
            return true;
        
        return false;
    }

    public bool CanThisMove(DirectionType direction)
    {
        Vector2Int directionVector = direction.ToVector();
        Cell cell = _field.Get(_characterPart.Position + directionVector);
        if (cell.IsWall())
            return false;
        else
            return true;
    }

    public void MoveThis(DirectionType direction)
    {
        var destination = _characterPart.Position + direction.ToVector();
        _characterPart.SetPosition(destination);
    }
}
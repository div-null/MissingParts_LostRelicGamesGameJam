using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;

public class CharacterPartMovement: MonoBehaviour
{
    private Field _field;
    
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

    public void Move(CharacterPart characterPart,  DirectionType direction)
    {
        if (!CanMove(characterPart, direction)) return;

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

        visitNode(characterPart);
    }
    
    public void Move(DirectionType direction)
    {
        CharacterPart characterPart = this.GetComponent<CharacterPart>();
        Move(characterPart, direction);
    }
    
    public void Move(DirectionType direction, int numberOfSteps)
    {
        CharacterPart characterPart = this.GetComponent<CharacterPart>();

        for (int i = 0; i < numberOfSteps; i++)
        {
            if (!CanMove(characterPart, direction)) return;

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

            visitNode(characterPart);
        }
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
}
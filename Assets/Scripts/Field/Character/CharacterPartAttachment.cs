using System.Collections.Generic;
using UnityEngine;

public class CharacterPartAttachment: MonoBehaviour
{
    public void AttachParts()
    {
        CharacterPart characterPart = this.GetComponent<CharacterPart>();
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        void visitNode(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);
            if (!part.IsLeaf())
            {
                visitNode(part.Left);
                visitNode(part.Right);
                visitNode(part.Up);
                visitNode(part.Down);
                return;
            }

            TryAttachCells(part);
        }

        visitNode(characterPart);
    }

    public void TryAttachCells(CharacterPart part)
    {
        part.TryJoin(DirectionType.Down);
        part.TryJoin(DirectionType.Up);
        part.TryJoin(DirectionType.Left);
        part.TryJoin(DirectionType.Right);
    }
    
    public void CheckForPits()
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        void visitNode(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);

            if (HasPitInCell(part.Position))
                _characterPart.PutAway();

            visitNode(part.Left);
            visitNode(part.Right);
            visitNode(part.Up);
            visitNode(part.Down);
        }

        visitNode(_characterPart);
    }
}
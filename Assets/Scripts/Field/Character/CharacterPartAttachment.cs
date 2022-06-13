using System.Collections.Generic;
using UnityEngine;

public class CharacterPartAttachment : MonoBehaviour
{
    private CharacterPart _characterPart;
    private Field _field;

    public void Initialize(CharacterPart characterPart, Field field)
    {
        _characterPart = characterPart;
        _field = field;
    }

    public void AttachParts()
    {
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

            tryJoinAllDirections(part);
        }

        visitNode(_characterPart);
    }

    private void tryJoinAllDirections(CharacterPart part)
    {
        part.TryJoin(DirectionType.Down);
        part.TryJoin(DirectionType.Up);
        part.TryJoin(DirectionType.Left);
        part.TryJoin(DirectionType.Right);
    }
}
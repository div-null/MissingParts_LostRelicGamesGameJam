using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;
using UnityEngine.InputSystem;
using VContainer;

public class Character : MonoBehaviour
{
    private CharacterPart _mainPart;
    private ColorType _characterColor;

    public void AddParts(List<CharacterPart> parts)
    {
        // Взяли первый элемент
        _mainPart = parts.GetEnumerator().Current;
        _mainPart = parts.First();
    }
    }

    public void Move(Vector2 vectorDirection)
    {
        _mainPart.CharacterPartMovement.Move(vectorDirection.ToDirection());
        _mainPart.CharacterPartAttachment.AttachParts();
    }

    //FindMax
    //ChangeMainPart
    //Get cell from field for logic
   
    public int PlayerSize()
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        void visitNode(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            visited.Add(part);

            visitNode(part.Down);
            visitNode(part.Up);
            visitNode(part.Right);
            visitNode(part.Left);
        }

        visitNode(_mainPart);
        return visited.Count;
    }
}
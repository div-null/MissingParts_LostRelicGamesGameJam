using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;
using VContainer;

public class Character : MonoBehaviour
{
    private CharacterPart _mainPart;
    private ColorType _characterColor;
    private Field _field;

    [Inject]
    public void Initialize(Field field)
    {
        _field = field;
    }

    public void AddParts(IEnumerable<CharacterPart> parts)
    {
        // Взяли первый элемент
        _mainPart = parts.GetEnumerator().Current;
    }

    public void Disengage()
    {
    }

    //FindMax
    //ChangeMainPart
    //Get cell from field for logic

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

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

    public bool CanMove(DirectionType direction)
    {
        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        bool visitNode(CharacterPart part)
        {
            if (part == null) return true;
            if (visited.Contains(part)) return true;
            var destination = part.Position + direction.ToVector();
            Cell cell = _field.Get(destination);
            if (cell != null && cell.CellType == CellType.Wall)
                return false;

            return visitNode(part.Left) && visitNode(part.Right) && visitNode(part.Up) && visitNode(part.Down);
        }

        return visitNode(_mainPart);
    }

    public void AttachPart()
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

            TryAttachCells(part);
        }

        visitNode(_mainPart);
    }

    public void TryAttachCells(CharacterPart part)
    {
        part.TryJoin(DirectionType.Down);
        part.TryJoin(DirectionType.Up);
        part.TryJoin(DirectionType.Left);
        part.TryJoin(DirectionType.Right);
    }


    public void Move(DirectionType direction)
    {
        if (!CanMove(direction)) return;

        HashSet<CharacterPart> visited = new HashSet<CharacterPart>();

        void visitNode(CharacterPart part)
        {
            if (part == null) return;
            if (visited.Contains(part)) return;
            var destination = part.Position + direction.ToVector();
            part.SetPosition(destination);

            visitNode(part.Left);
            visitNode(part.Right);
            visitNode(part.Up);
            visitNode(part.Down);
        }

        visitNode(_mainPart);
    }
}
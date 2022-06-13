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

    public void Move(Vector2 vectorDirection)
    {
        _mainPart.CharacterPartMovement.Move(vectorDirection.ToDirection());
        _mainPart.CharacterPartAttachment.AttachParts();
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
}
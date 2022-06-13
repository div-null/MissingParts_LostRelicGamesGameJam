using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;

public class Field : MonoBehaviour
{
    private Cell[,] Cells;
    private List<CharacterPart> CharacterParts;

    public void SetCells(Cell[,] cells)
    {
        Cells = cells;
    }

    public void Setup(List<CharacterPart> parts)
    {
        CharacterParts = parts;

        foreach (var part in CharacterParts)
            part.CharacterPartAttachment.AttachParts();
    }

    public Cell Get(Vector2Int coordinates)
    {
        if (coordinates.x < 0 || coordinates.y < 0 || coordinates.x > Cells.GetLength(0) - 1 || coordinates.y > Cells.GetLength(1) - 1)
            return null;
        return Cells[coordinates.x, coordinates.y];
    }

    void LoadField()
    {
        //TODO: read field + initialize field and character
    }

    //TODO: Join all CharacterParts to each other at start like how it implement in Character.cs in TryAttachCells

    // Start is called before the first frame update
    // void Start()
    // {
    //     Cells = new Cell[5, 5];
    //     CharacterParts = new List<CharacterPart>();
    //
    //     for (int j = 0; j < 5; j++)
    //     {
    //         for (int i = 0; i < 5; i++)
    //         {
    //             Cell newCell;
    //             if (i == 0 || i == 4 || j == 0 || j == 4)
    //                 newCell = Instantiate(WallCellPrefab, new Vector3(i, j, -1), Quaternion.identity).GetComponent<Cell>();
    //             else
    //                 newCell = Instantiate(EmptyCellPrefab, new Vector3(i, j, -1), Quaternion.identity).GetComponent<Cell>();
    //
    //             newCell.transform.SetParent(transform);
    //             Cells[i, j] = newCell;
    //
    //             if (i == 3 && j == 2)
    //             {
    //                 CharacterPart newCharacterPart = Instantiate(CharacterPartPrefab, new Vector3(i, j, -2), Quaternion.identity).GetComponent<CharacterPart>();
    //                 CharacterParts.Add(newCharacterPart);
    //                 newCharacterPart.transform.SetParent(transform);
    //             }
    //         }
    //     }
    // }

    // Update is called once per frame
    void Update()
    {
    }
}
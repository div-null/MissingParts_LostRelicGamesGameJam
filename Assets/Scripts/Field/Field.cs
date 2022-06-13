using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using UnityEngine;

public class Field : MonoBehaviour
{
    private Cell[,] _cells;
    private List<CharacterPart> _characterParts;
    private List<Cell> _finishCells;

    public event Action Finished;

    public void SetCells(Cell[,] cells)
    {
        _cells = cells;
    }

    public void Setup(List<CharacterPart> parts)
    {
        _characterParts = parts;

        foreach (var part in _characterParts)
            part.CharacterPartAttachment.AttachParts();

        FindFinishCells();
    }

    private void FindFinishCells()
    {
        _finishCells = new List<Cell>();
        
        for (int j = 0; j < _cells.GetLength(1); j++)
        {
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                if (_cells[i, j].CellType == CellType.Finish)
                {
                    _finishCells.Add(_cells[i, j]);
                }
            }
        }
    }

    public void CheckForFinish()
    {
        if (IsFinished())
        {
            Finished?.Invoke();
            Debug.Log("Level complete!");
        }
    }
    
    public bool IsFinished()
    {
        HashSet<CharacterPart> visitedParts = new HashSet<CharacterPart>();
        foreach (var finishCell in _finishCells)
        {
            if (finishCell.CharacterPart == null)
                return false;

            if (!finishCell.CharacterPart.HasRightShape(visitedParts))
                return false;
        }

        return true;
    }

    public void DestroyField()
    {
        for (int j = 0; j < _cells.GetLength(1); j++)
        {
            for (int i = 0; i < _cells.GetLength(0); i++)
            {
                Destroy(_cells[i, j]);
            }
        }

        foreach (var part in _characterParts)
        {
            Destroy(part);
        }
    }

    public Cell Get(Vector2Int coordinates)
    {
        return Get(coordinates.x, coordinates.y);
    }

    public Cell Get(int x, int y)
    {
        if (x < 0 || y < 0 || x > _cells.GetLength(0) - 1 || y > _cells.GetLength(1) - 1)
            return null;
        return _cells[x, y];
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
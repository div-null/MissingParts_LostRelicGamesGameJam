using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Field.Cell;
using Unity.VisualScripting;
using UnityEngine;

public class Field : MonoBehaviour
{
    private Cell[,] _cells;
    private List<Cell> _finishCells;
    private ColorType _finishColor;
    public event Action Finished;

    public void SetCells(Cell[,] cells)
    {
        _cells = cells;
    }

    public void Setup(List<Cell> finishCells, ColorType finishColor)
    {
        _finishCells = finishCells;
        _finishColor = finishColor;
        foreach (var finishCell in finishCells)
        {
            finishCell.GetComponent<FinishView>().SetColor(_finishColor);
        }
    }

    public void CheckForFinish()
    {
        if (IsFinished())
            Finished?.Invoke();
    }

    public bool IsFinished()
    {
        HashSet<CharacterPart> visitedParts = new HashSet<CharacterPart>();
        foreach (var finishCell in _finishCells)
        {
            if (finishCell.CharacterPart == null)
                return false;

            if (finishCell.CharacterPart.Color != _finishColor || !finishCell.CharacterPart.HasRightShape(visitedParts))
                return false;
        }

        return true;
    }

    public void Destroy()
    {
        for (int j = 0; j < _cells.GetLength(1); j++)
        for (int i = 0; i < _cells.GetLength(0); i++)
            Destroy(_cells[i, j].gameObject);

        Destroy(gameObject);
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

    public bool TryGet(int x, int y, out Cell cell)
    {
        cell = Get(x, y);
        return cell != null;
    }

    public bool TryGet(Vector2Int coordinates, out Cell cell)
    {
        cell = Get(coordinates);
        return cell != null;
    }

}
using System;
using System.Collections.Generic;
using UnityEngine;

public class GridModel
{
    private int[,] grid;

    public int[,] Grid => grid;

    private int _row;

    private int _collumn;

    public event  Action<Vector2Int, int> OnCellChanged;

    public event Action<List<Vector2Int>> OnLinesCleared;

    public void InitGrid(int row, int collumn)
    {
        _row = row;
        _collumn = collumn;
        grid = new int[row, collumn];
    }

    public void SetCell(Vector2Int pos, int value)
    {
        grid[pos.x,pos.y] = value;
        OnCellChanged?.Invoke(pos, value);
    }
    public int GetCell(Vector2Int pos )
    {
        return grid[pos.x, pos.y];
    }
    public bool IsEmpty(Vector2Int pos)
    {
        return grid[pos.x, pos.y] == 0;
    }

    public bool IsInBound(Vector2Int pos)
    {
        if(pos.x < 0 || pos.x >= grid.GetLength(0))
        {
            return false;
        }
        if(pos.y < 0 || pos.y >=grid.GetLength(1))
        {
            return false;
        }
        return true;
    }
    public void Clear()
    {
        grid = new int[_row,_collumn];
    }

    public void PublishOnLineCleared(List<Vector2Int> vector2Ints)
    {
        OnLinesCleared?.Invoke(vector2Ints);
    }


}

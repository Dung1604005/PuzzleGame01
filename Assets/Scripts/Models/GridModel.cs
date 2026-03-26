using System;
using System.Collections.Generic;
using UnityEngine;

public class GridModel
{
    private int[,] grid;
    private Sprite[,] cellSprites;

    public int[,] Grid => grid;
    public Sprite[,] CellSprites => cellSprites;

    private int _row;

    public int Row => _row;

    private int _collumn;

    public int Collumn => _collumn;


    public void InitGrid(int row, int collumn)
    {
        _row = row;
        _collumn = collumn;
        grid = new int[row, collumn];
        cellSprites = new Sprite[row, collumn];
    }

    public void SetCell(Vector2Int pos, int value, Sprite sprite)
    {
        grid[pos.x, pos.y] = value;
        cellSprites[pos.x, pos.y] = sprite;
        EventBus.Instance.Publish(new OnCellChanged
        {
            position = pos,
            newValue = value,
            newSprite = sprite
        });

    }
    public int GetCell(Vector2Int pos)
    {
        return grid[pos.x, pos.y];
    }
    public bool IsEmpty(Vector2Int pos)
    {
        return grid[pos.x, pos.y] == 0;
    }

    public bool IsInBound(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x >= grid.GetLength(0))
        {
            return false;
        }
        if (pos.y < 0 || pos.y >= grid.GetLength(1))
        {
            return false;
        }
        return true;
    }
    public void Clear()
    {
        grid = new int[_row, _collumn];
        cellSprites = new Sprite[_row, _collumn];
        for (int i = 0; i < _row; i++)
        {
            for (int j = 0; j < _collumn; j++)
            {
                EventBus.Instance.Publish(new OnCellChanged
                {
                    position = new Vector2Int(i, j),
                    newValue = 0,
                    newSprite = GameManager.Instance.ThemeData.GridSprite
                });
            }
        }
    }




}

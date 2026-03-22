using System.Collections.Generic;
using UnityEngine;

public static class PlacementClearPreviewCalculator
{
    public static List<Vector2Int> GetPreviewCells(int[,] grid, PieceData pieceData, Vector2Int origin)
    {
        List<Vector2Int> previewCells = new List<Vector2Int>();

        if (grid == null || pieceData == null)
        {
            return previewCells;
        }

        HashSet<Vector2Int> placedCells = BuildPlacedCells(pieceData, origin);

        List<int> fullRows = FindFullRowsAfterPlacement(grid, placedCells);
        List<int> fullColumns = FindFullColumnsAfterPlacement(grid, placedCells);

        HashSet<Vector2Int> uniqueCells = new HashSet<Vector2Int>();

        for (int row = 0; row < grid.GetLength(0); row++)
        {
            if (!fullRows.Contains(row))
            {
                continue;
            }

            for (int column = 0; column < grid.GetLength(1); column++)
            {
                uniqueCells.Add(new Vector2Int(row, column));
            }
        }

        for (int column = 0; column < grid.GetLength(1); column++)
        {
            if (!fullColumns.Contains(column))
            {
                continue;
            }

            for (int row = 0; row < grid.GetLength(0); row++)
            {
                uniqueCells.Add(new Vector2Int(row, column));
            }
        }

        previewCells.AddRange(uniqueCells);
        return previewCells;
    }

    private static HashSet<Vector2Int> BuildPlacedCells(PieceData pieceData, Vector2Int origin)
    {
        HashSet<Vector2Int> placedCells = new HashSet<Vector2Int>();

        foreach (Vector2Int offset in pieceData.CellOffsets)
        {
            placedCells.Add(origin + offset);
        }

        return placedCells;
    }

    private static List<int> FindFullRowsAfterPlacement(int[,] grid, HashSet<Vector2Int> placedCells)
    {
        List<int> fullRows = new List<int>();

        for (int row = 0; row < grid.GetLength(0); row++)
        {
            bool isFull = true;

            for (int column = 0; column < grid.GetLength(1); column++)
            {
                if (grid[row, column] == 0 && !placedCells.Contains(new Vector2Int(row, column)))
                {
                    isFull = false;
                    break;
                }
            }

            if (isFull)
            {
                fullRows.Add(row);
            }
        }

        return fullRows;
    }

    private static List<int> FindFullColumnsAfterPlacement(int[,] grid, HashSet<Vector2Int> placedCells)
    {
        List<int> fullColumns = new List<int>();

        for (int column = 0; column < grid.GetLength(1); column++)
        {
            bool isFull = true;

            for (int row = 0; row < grid.GetLength(0); row++)
            {
                if (grid[row, column] == 0 && !placedCells.Contains(new Vector2Int(row, column)))
                {
                    isFull = false;
                    break;
                }
            }

            if (isFull)
            {
                fullColumns.Add(column);
            }
        }

        return fullColumns;
    }
}

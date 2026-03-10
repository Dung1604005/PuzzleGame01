using System.Collections.Generic;
using UnityEngine;

public class LineClearCalculator
{
    public static List<int> FindFullRows(int[,] grid)
    {
        int row = grid.GetLength(0);
        int collumn = grid.GetLength(1);
        List<int> fullRows = new List<int>();
        for (int posY = 0; posY < row; posY++)
        {
            // Danh dau lastCell la gia tri cell gan nhat. Ban dau khong co nen la 0
            int lastCell = 0;
            for (int posX = 0; posX < collumn; posX++)
            {
                // Gap cell rong thi stop
                // Neu lastCell khac rong ma current cell != lastCell thi dong nay khong tinh => bo
                // Neu currentCell == lastCell thi tiep tuc
                if (grid[posY, posX] == 0)
                {

                    lastCell = 0;
                    break;
                }
                else
                {
                    if (lastCell == 0)
                    {
                        lastCell = grid[posY, posX];
                    }
                    else if (lastCell != grid[posY, posX])
                    {
                        lastCell = 0;
                        break;
                    }
                }
            }
            if (lastCell > 0)
            {
                fullRows.Add(posY);
            }
        }
        return fullRows;
    }
    public static List<int> FindFullCollumn(int[,] grid)
    {
        int row = grid.GetLength(0);
        int collumn = grid.GetLength(1);
        List<int> fullCollumn = new List<int>();
        // Tuong tu logic voi row
        for (int posX = 0; posX < collumn; posX++)
        {
            int lastCell = 0;
            for (int posY = 0; posY < row; posY++)
            {
                if (grid[posY, posX] == 0)
                {
                    lastCell = 0;
                    break;
                }
                else
                {
                    if (lastCell == 0)
                    {
                        lastCell = grid[posY, posX];
                    }
                    else if (lastCell != grid[posY, posX])
                    {
                        lastCell = 0;
                        break;
                    }
                }
            }
            if (lastCell > 0)
            {
                fullCollumn.Add(posX);
            }
        }
        return fullCollumn;
    }
    public static List<Vector2Int> GetCellsToRemove(int[,] grid)
    {
        List<int> fullRows = FindFullRows(grid);
        List<int> fullCollumns = FindFullCollumn(grid);

        List<Vector2Int> cellsToRemove = new List<Vector2Int>();


        for (int posY = 0; posY < grid.GetLength(0); posY++)
        {
            for (int posX = 0; posX < grid.GetLength(1); posX++)
            {
                //Dung de kiem tra cell nay da duoc tinh chua 

                bool added = false;

                //Kiem tra xem o nay co thuoc hang bi xoa khong 
                for (int index = 0; index < fullRows.Count; index++)
                {
                    if (fullRows[index] == posY)
                    {
                        added = true;
                        cellsToRemove.Add(new Vector2Int(posY, posX));
                    }
                }
                if (!added)
                {
                    //Kiem tra xem o nay co thuoc cot bi xoa khong 
                    for (int index = 0; index < fullCollumns.Count; index++)
                    {
                        if (fullCollumns[index] == posX)
                        {
                            added = true;
                            cellsToRemove.Add(new Vector2Int(posY, posX));
                        }
                    }
                }
            }
            
        }
        return cellsToRemove;


    }
}

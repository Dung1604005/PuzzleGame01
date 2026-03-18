

using UnityEngine;

public class GridCoordinateConverter 
{
    // Chuyen tu toa do world sang toa do luoi 
    public static Vector2Int WorldToGrid(Vector2 worldPos, GridConfig gridConfig)
    {
        Debug.Log(worldPos);
        if(worldPos.x < gridConfig.gridOrigin.x || worldPos.y < gridConfig.gridOrigin.y 
        || worldPos.x >gridConfig.gridOrigin.x+ gridConfig.cellSize.x*gridConfig.gridWidth + gridConfig.offsetCell.x*(gridConfig.gridWidth - 1)
        || worldPos.y >gridConfig.gridOrigin.y+ gridConfig.cellSize.y*gridConfig.gridHeight + gridConfig.offsetCell.y*(gridConfig.gridHeight - 1))
        {
            Debug.Log("WORLD POSITION OUT OF GRID");
            return new Vector2Int(-1, -1);
        }
        int haveOffsetX = 0;
        int haveOffsetY = 0;
        if(worldPos.x >= gridConfig.cellSize.x)
        {
            haveOffsetX = 1;
        }
        if(worldPos.y >= gridConfig.cellSize.y)
        {
            haveOffsetY =1;
        }
        int x = Mathf.FloorToInt((worldPos.x - gridConfig.gridOrigin.x - haveOffsetX)/(gridConfig.offsetCell.x + gridConfig.cellSize.x));
        int y = Mathf.FloorToInt((worldPos.y - gridConfig.gridOrigin.y - haveOffsetY)/ (gridConfig.offsetCell.y + gridConfig.cellSize.y));
        return new Vector2Int(x, y);
    }
}

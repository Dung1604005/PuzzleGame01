

using UnityEngine;

public class GridCoordinateConverter 
{
    // Chuyen tu toa do world sang toa do luoi 
    public static Vector2Int WorldToGrid(Vector2 worldPos)
    {
        GridConfig gridConfig = GameManager.Instance.GridConfig;
        if(worldPos.x < gridConfig.gridOrigin.x || worldPos.y < gridConfig.gridOrigin.y 
        || worldPos.x >gridConfig.gridOrigin.x+ gridConfig.cellSize.x*gridConfig.gridWidth + gridConfig.offsetCell.x*(gridConfig.gridWidth - 1)
        || worldPos.y >gridConfig.gridOrigin.y+ gridConfig.cellSize.y*gridConfig.gridHeight + gridConfig.offsetCell.y*(gridConfig.gridHeight - 1))
        {
            Debug.Log("WORLD POSITION OUT OF GRID");
            return new Vector2Int(-1, -1);
        }
        float haveOffsetX = 0;
        float haveOffsetY = 0;
        if(worldPos.x >= gridConfig.cellSize.x)
        {
            haveOffsetX = gridConfig.offsetCell.x;
        }
        if(worldPos.y >= gridConfig.cellSize.y)
        {
            haveOffsetY =gridConfig.offsetCell.y;
        }
        int x = Mathf.FloorToInt((worldPos.x - gridConfig.gridOrigin.x + haveOffsetX)/(gridConfig.offsetCell.x + gridConfig.cellSize.x));
        int y = Mathf.FloorToInt((worldPos.y - gridConfig.gridOrigin.y + haveOffsetY)/ (gridConfig.offsetCell.y + gridConfig.cellSize.y));
        return new Vector2Int(x, y);
    }

    public static Vector3 GridToWorld(Vector2Int gridPos)
    {
        GridConfig config = GameManager.Instance.GridConfig;
        float x = config.gridOrigin.x + (gridPos.x - 1)*(config.offsetCell.x + config.cellSize.x) + config.cellSize.x/2f;
        float y = config.gridOrigin.y + (gridPos.y - 1)*(config.offsetCell.y + config.cellSize.y) + config.cellSize.y/2f;
        return new Vector3(x, y, 0f);
        
    }
}

using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private GridConfig gridConfig;


    public void OnDrawGizmos()
    {
        if (gridConfig == null) return;

        Gizmos.color = new Color(1f, 1f, 1f, 0.4f);

        for(int posY = 0; posY < gridConfig.gridHeight; posY++)
        {
            for(int posX = 0;posX< gridConfig.gridWidth; posX++)
            {
                Vector2 center = gridConfig.gridOrigin + new Vector2(0, posY*(gridConfig.cellSize.y + gridConfig.offsetCell.y))
                + new Vector2(posX*(gridConfig.cellSize.x + gridConfig.offsetCell.x), 0) + gridConfig.cellSize/2;
                Gizmos.DrawCube(center, gridConfig.cellSize);
            }
        }
        

    }
}

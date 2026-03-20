using System.Collections.Generic;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private GridConfig gridConfig;

    private List<List<GameObject>> blockList = new List<List<GameObject>>();


    public void OnEnable()
    {
        EventBus.Instance.Subscribe<OnCellChanged>(OnDrawCell);
    }

    public void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnCellChanged>(OnDrawCell);
    }


    [ContextMenu("Ve grid")]
    public void OnDraw()
    {
        if (gridConfig == null) return;

        Gizmos.color = new Color(1f, 1f, 1f, 0.4f);

        for(int posY = 0; posY < gridConfig.gridHeight; posY++)
        {
            blockList.Add(new List<GameObject>());
            for(int posX = 0;posX< gridConfig.gridWidth; posX++)
            {
                Vector2 center = gridConfig.gridOrigin + new Vector2(0, posY*(gridConfig.cellSize.y + gridConfig.offsetCell.y))
                + new Vector2(posX*(gridConfig.cellSize.x + gridConfig.offsetCell.x), 0) + gridConfig.cellSize/2;

                GameObject block = Instantiate(GameManager.Instance.BlockPrefab, this.transform);
                block.transform.position = center;
                block.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.ThemeData.GridSprite;
                blockList[posY].Add(block);
                       
            }
        }
    }

    public void OnDrawCell(OnCellChanged onCellChanged)
    {
        GameObject block = blockList[onCellChanged.position.y][onCellChanged.position.x] ;
        if(onCellChanged.newValue == 0 )
        {
            block.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.ThemeData.GridSprite;
        }
        else
        {
            block.GetComponent<SpriteRenderer>().sprite = onCellChanged.newSprite;
        }
    }


}

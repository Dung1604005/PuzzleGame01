using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private GridConfig gridConfig;

    private List<List<GameObject>>  blockList = new List<List<GameObject>>();


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
        block.GetComponent<SpriteRenderer>().DOFade(1f, 0.05f);
        if(onCellChanged.newValue == 0 )
        {
            block.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.ThemeData.GridSprite;
        }
        else
        {
            block.GetComponent<SpriteRenderer>().sprite = onCellChanged.newSprite;
        }
    }

    public void DrawCellPreview(Vector2Int position, Sprite sprite, bool isReverse)
    {
        if(sprite == null)
        {
            Debug.Log("PREVIEW CELL DONT HAVE SPRITE");
            return;
        }
        GameObject block = blockList[position.y][position.x] ;
        // Dua block ve trang thai ban dau
        if (isReverse)
        {
            block.GetComponent<SpriteRenderer>().sprite = sprite;
            block.GetComponent<SpriteRenderer>().DOFade(1f, 0.05f);
        }
        else
        {
            block.GetComponent<SpriteRenderer>().sprite = sprite;
         block.GetComponent<SpriteRenderer>().DOFade(0.3f, 0.05f);
        }
         

        
    }

    void Start()
    {
        OnDraw();
    }


}

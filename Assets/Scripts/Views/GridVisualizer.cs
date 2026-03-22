using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GridVisualizer : MonoBehaviour
{
    [SerializeField] private GridConfig gridConfig;

    [Header("Grid Border")]
    [SerializeField] private bool drawBorder = true;
    [SerializeField] private Color borderColor = new Color(0.15f, 0.15f, 0.15f, 1f);
    [SerializeField] private float borderWidth = 0.06f;
    [SerializeField] private float borderPadding = 0.04f;
    [SerializeField] private int borderSortingOrder = 3;

    private List<List<GameObject>>  blockList = new List<List<GameObject>>();
    private LineRenderer borderRenderer;


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
        
        blockList.Clear();
        

        for(int posY = 0; posY < gridConfig.gridHeight; posY++)
        {
            blockList.Add(new List<GameObject>());
            for(int posX = 0;posX< gridConfig.gridWidth; posX++)
            {
                Vector2 center = gridConfig.gridOrigin + new Vector2(0, posY*(gridConfig.cellSize.y + gridConfig.offsetCell.y))
                + new Vector2(posX*(gridConfig.cellSize.x + gridConfig.offsetCell.x), 0) + gridConfig.cellSize/2;

                GameObject block = ObjectPoolManager.Instance.Spawn(GameManager.Instance.BlockPrefab, center, Quaternion.identity);
                block.GetComponent<SpriteRenderer>().sprite = GameManager.Instance.ThemeData.GridSprite;
                blockList[posY].Add(block);
                       
            }
        }

        DrawGridBorder();
    }

    private void DrawGridBorder()
    {
        if (!drawBorder)
        {
            if (borderRenderer != null)
            {
                borderRenderer.enabled = false;
            }
            return;
        }

        if (borderRenderer == null)
        {
            GameObject borderObj = new GameObject("GridBorder");
            borderObj.transform.SetParent(transform, false);
            borderRenderer = borderObj.AddComponent<LineRenderer>();
            borderRenderer.material = new Material(Shader.Find("Sprites/Default"));
            borderRenderer.loop = true;
            borderRenderer.positionCount = 4;
            borderRenderer.useWorldSpace = true;
            borderRenderer.numCornerVertices = 2;
            borderRenderer.numCapVertices = 2;
        }

        float stepX = gridConfig.cellSize.x + gridConfig.offsetCell.x;
        float stepY = gridConfig.cellSize.y + gridConfig.offsetCell.y;

        float left = gridConfig.gridOrigin.x - borderPadding;
        float bottom = gridConfig.gridOrigin.y - borderPadding;
        float right = gridConfig.gridOrigin.x + (gridConfig.gridWidth - 1) * stepX + gridConfig.cellSize.x + borderPadding;
        float top = gridConfig.gridOrigin.y + (gridConfig.gridHeight - 1) * stepY + gridConfig.cellSize.y + borderPadding;

        borderRenderer.enabled = true;
        borderRenderer.startColor = borderColor;
        borderRenderer.endColor = borderColor;
        borderRenderer.startWidth = borderWidth;
        borderRenderer.endWidth = borderWidth;
        borderRenderer.sortingOrder = borderSortingOrder;

        borderRenderer.SetPosition(0, new Vector3(left, bottom, 0f));
        borderRenderer.SetPosition(1, new Vector3(left, top, 0f));
        borderRenderer.SetPosition(2, new Vector3(right, top, 0f));
        borderRenderer.SetPosition(3, new Vector3(right, bottom, 0f));
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

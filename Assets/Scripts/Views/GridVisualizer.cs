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
    private List<List<SpriteRenderer>> blockRenderers = new List<List<SpriteRenderer>>();
    private Sprite[,] baseGridSprites;
    private LineRenderer borderRenderer;


    public void OnEnable()
    {
        EventBus.Instance.Subscribe<OnCellChanged>(OnDrawCell);
        EventBus.Instance.Subscribe<OnLineCleared>(PlayClearLineEffect);
    }

    public void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnCellChanged>(OnDrawCell);
        EventBus.Instance.UnSubscribe<OnLineCleared>(PlayClearLineEffect);
    }


    [ContextMenu("Ve grid")]
    public void OnDraw()
    {
        if (gridConfig == null) return;

        ClearExistingGrid();
        
        blockList.Clear();
        blockRenderers.Clear();
        baseGridSprites = new Sprite[gridConfig.gridHeight, gridConfig.gridWidth];
        

        for(int posY = 0; posY < gridConfig.gridHeight; posY++)
        {
            blockList.Add(new List<GameObject>());
            blockRenderers.Add(new List<SpriteRenderer>());
            for(int posX = 0;posX< gridConfig.gridWidth; posX++)
            {
                Vector2 center = gridConfig.gridOrigin + new Vector2(0, posY*(gridConfig.cellSize.y + gridConfig.offsetCell.y))
                + new Vector2(posX*(gridConfig.cellSize.x + gridConfig.offsetCell.x), 0) + gridConfig.cellSize/2;

                GameObject block = ObjectPoolManager.Instance.Spawn(GameManager.Instance.BlockPrefab, center, Quaternion.identity);
                SpriteRenderer renderer = block.GetComponent<SpriteRenderer>();
                renderer.sprite = GameManager.Instance.ThemeData.GridSprite;
                blockList[posY].Add(block);
                blockRenderers[posY].Add(renderer);
                baseGridSprites[posY, posX] = GameManager.Instance.ThemeData.GridSprite;
                       
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
        
        if (!IsValidCell(onCellChanged.position))
        {
            return;
        }
       
        SpriteRenderer renderer = blockRenderers[onCellChanged.position.y][onCellChanged.position.x];
        renderer.DOFade(1f, 0.05f);
        Sprite spriteToUse = onCellChanged.newValue == 0
            ? GameManager.Instance.ThemeData.GridSprite
            : onCellChanged.newSprite;

        baseGridSprites[onCellChanged.position.y, onCellChanged.position.x] = spriteToUse;

        renderer.sprite = spriteToUse;
    }

    public void DrawCellPreview(Vector2Int position, Sprite sprite, bool isReverse)
    {
        if (!IsValidCell(position) || sprite == null)
        {
            return;
        }

        SpriteRenderer renderer = blockRenderers[position.y][position.x];
        // Dua block ve trang thai ban dau
        if (isReverse)
        {
            renderer.sprite = sprite;
            renderer.DOFade(1f, 0.05f);
        }
        else
        {
            renderer.sprite = sprite;
            renderer.DOFade(0.3f, 0.05f);
        }
         

        
    }

    public void DrawLineClearPreview(List<Vector2Int> previewCells, Sprite previewSprite)
    {
        if (previewCells == null || previewCells.Count == 0 || previewSprite == null)
        {
            return;
        }

        foreach (Vector2Int cell in previewCells)
        {
            if (!IsValidCell(cell))
            {
                continue;
            }

            SpriteRenderer renderer = blockRenderers[cell.y][cell.x];
            renderer.sprite = previewSprite;
            
        }
    }

    public void ClearLineClearPreview(List<Vector2Int> previewCells)
    {
        if (previewCells == null || previewCells.Count == 0)
        {
            return;
        }

        foreach (Vector2Int cell in previewCells)
        {
            if (!IsValidCell(cell))
            {
                continue;
            }

            SpriteRenderer renderer = blockRenderers[cell.y][cell.x];
            renderer.sprite = baseGridSprites[cell.y, cell.x];
            
        }
    }

    public void PlayClearLineEffect(OnLineCleared onLineCleared)
    {
        if (onLineCleared.position == null || onLineCleared.position.Count == 0)
        {
            return;
        }

        List<GameObject> tilesToClear = new List<GameObject>(onLineCleared.position.Count);

        foreach(Vector2Int pos in onLineCleared.position)
        {
            GameObject spawnedTile = ObjectPoolManager.Instance.Spawn(GameManager.Instance.BlockPrefab, blockList[pos.y][pos.x].transform.position, Quaternion.identity);
            SpriteRenderer renderer = spawnedTile.GetComponent<SpriteRenderer>();
            renderer.sprite = GameManager.Instance.BatchController.CurrentSpritePiece;
            renderer.sortingOrder = 10;
            tilesToClear.Add(spawnedTile);
        }

        Sequence clearSeq = DOTween.Sequence();

        foreach(GameObject tile in tilesToClear)
        {
            // Hieu ung thu nho lai
            clearSeq.Insert(0, tile.transform.DOScale(Vector3.zero, 0.25f).SetEase(Ease.InBack));

            // Hieu ung xoay
            clearSeq.Insert(0, tile.transform.DORotate(new Vector3(0, 0, 90f), 0.25f, RotateMode.FastBeyond360));

        }

        clearSeq.OnComplete(() =>
        {
            foreach(GameObject tile in tilesToClear)
            {
                tile.transform.localScale = Vector3.one;
                tile.transform.rotation = Quaternion.identity;
                ObjectPoolManager.Instance.Despawn(tile);
            }
        });
    }

    private bool IsValidCell(Vector2Int cell)
    {
        if (blockList.Count == 0 || blockRenderers.Count == 0 || baseGridSprites == null)
        {
            return false;
        }

        if (cell.x < 0 || cell.y < 0)
        {
            return false;
        }

        if (cell.y >= blockList.Count || cell.x >= blockList[cell.y].Count)
        {
            return false;
        }

        return true;
    }

    void Start()
    {
        OnDraw();
    }

    private void ClearExistingGrid()
    {
        if (blockList.Count == 0)
        {
            return;
        }

        foreach (List<GameObject> row in blockList)
        {
            foreach (GameObject cell in row)
            {
                if (cell != null)
                {
                    ObjectPoolManager.Instance.Despawn(cell);
                }
            }
        }
    }


}

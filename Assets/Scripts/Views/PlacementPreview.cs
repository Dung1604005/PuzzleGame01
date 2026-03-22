using UnityEngine;
using System.Collections.Generic;

public class PlacementPreview : MonoBehaviour
{
    [SerializeField] private BatchController batchController;

    private GridVisualizer gridVisualizer;
    private LineClearPreviewPresenter lineClearPreviewPresenter;

    // Luu vi tri preview gan nhat

    [SerializeField] private Vector2 lastPosition;

    [SerializeField] private PieceData lastPieceData;

    public void OnEnable()
    {
        EventBus.Instance.Subscribe<OnDragUpdate>(OnPreviewPiece);
        EventBus.Instance.Subscribe<OnDragEnd>(ClearPreviewPiece);
    }
    public void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnDragUpdate>(OnPreviewPiece);
        EventBus.Instance.UnSubscribe<OnDragEnd>(ClearPreviewPiece);
    }



    void Awake()
    {
        gridVisualizer = GetComponent<GridVisualizer>();
        lineClearPreviewPresenter = new LineClearPreviewPresenter(gridVisualizer);
        
    }

    public void OnPreviewPiece(OnDragUpdate onDragUpdate)
    {
        if(batchController == null)
        {
            return;
        }
        if(batchController.CurrentHoldingPiece == null)
        {
            lineClearPreviewPresenter.Clear();
            return;
        }

        Vector2 placePosition = onDragUpdate.pos + batchController.PivotPiece;
        Vector2Int gridOrigin = GridCoordinateConverter.WorldToGrid(placePosition);

        if(GameManager.Instance.GridController.CanPlacePiece(batchController.CurrentHoldingPiece, gridOrigin))
        {
            // Xoa o preview truoc 
            SetPreviewPiece(lastPieceData, GridCoordinateConverter.WorldToGrid(lastPosition), GameManager.Instance.ThemeData.GridSprite, true);
            
            // Ve preview
            SetPreviewPiece(batchController.CurrentHoldingPiece, gridOrigin, batchController.CurrentSpritePiece, false);

            List<Vector2Int> linePreviewCells = PlacementClearPreviewCalculator.GetPreviewCells(
                GameManager.Instance.GridModel.Grid,
                batchController.CurrentHoldingPiece,
                gridOrigin
            );

            lineClearPreviewPresenter.Show(linePreviewCells, batchController.CurrentSpritePiece);

            lastPosition = placePosition;

            lastPieceData = batchController.CurrentHoldingPiece;

        }
        else
        {
            SetPreviewPiece(lastPieceData, GridCoordinateConverter.WorldToGrid(lastPosition), GameManager.Instance.ThemeData.GridSprite, true);
            lineClearPreviewPresenter.Clear();
        }
    }

    public void ClearPreviewPiece(OnDragEnd onDragEnd )
    {
        lineClearPreviewPresenter.Clear();

        if(lastPieceData == null)
        {
            return;
        }
        Vector2Int origin = GridCoordinateConverter.WorldToGrid(lastPosition);
         foreach (Vector2Int offSet in lastPieceData.CellOffsets)
        {
            Vector2Int placePosition = origin + offSet;
            
            if(GameManager.Instance.GridModel == null)
            {
                Debug.Log("GRID MODEL IS NOT INITED!");
            }
            // Kiem tra xem co rong + hop le khong
            if (GameManager.Instance.GridModel.IsInBound(placePosition))
            {
                if (GameManager.Instance.GridModel.IsEmpty(placePosition))
                {
                    gridVisualizer.DrawCellPreview(placePosition, GameManager.Instance.ThemeData.GridSprite, true);
                }
            }
            
        }

        lastPieceData = null;
        lastPosition = Vector2.zero;
    }

    public void SetPreviewPiece(PieceData pieceData, Vector2Int origin, Sprite sprite, bool isReverse)
    {
        if(pieceData == null)
        {
            return;
        }
         foreach (Vector2Int offSet in pieceData.CellOffsets)
        {
            Vector2Int placePosition = origin + offSet;
            
            if(GameManager.Instance.GridModel == null)
            {
                Debug.Log("GRID MODEL IS NOT INITED!");
            }
            // Kiem tra xem co rong + hop le khong
            if (GameManager.Instance.GridModel.IsInBound(placePosition))
            {
                if (GameManager.Instance.GridModel.IsEmpty(placePosition))
                {
                    gridVisualizer.DrawCellPreview(placePosition, sprite, isReverse);
                }
                
                
                
            }
            
        }
    }




}

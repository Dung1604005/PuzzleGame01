using UnityEngine;

public class PlacementPreview : MonoBehaviour
{
    [SerializeField] private BatchController batchController;

    private GridVisualizer gridVisualizer;

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
        
    }

    public void OnPreviewPiece(OnDragUpdate onDragUpdate)
    {
        if(batchController == null)
        {
            Debug.Log("PLACEMENT REVIEW DONT HAVE BATCHCONTROLLER");
            return;
        }
        if(batchController.CurrentHoldingPiece == null)
        {
            Debug.Log("DONT HAVE PIECE HOLDING RIGHT NOW");
            return;
        }
        Vector2 placePosition = onDragUpdate.pos + batchController.PivotPiece;
        if(GameManager.Instance.GridController.CanPlacePiece(batchController.CurrentHoldingPiece, GridCoordinateConverter.WorldToGrid(placePosition)))
        {
            // Xoa o preview truoc 
            SetPreviewPiece(lastPieceData, GridCoordinateConverter.WorldToGrid(lastPosition), GameManager.Instance.ThemeData.GridSprite, true);
            
            // Ve preview
            SetPreviewPiece(batchController.CurrentHoldingPiece, GridCoordinateConverter.WorldToGrid(placePosition), batchController.CurrentSpritePiece, false);

            lastPosition = placePosition;

            lastPieceData = batchController.CurrentHoldingPiece;

        }
        else
        {
            SetPreviewPiece(lastPieceData, GridCoordinateConverter.WorldToGrid(lastPosition), GameManager.Instance.ThemeData.GridSprite, true);
        }
    }

    public void ClearPreviewPiece(OnDragEnd onDragEnd )
    {
        if(lastPieceData == null)
        {
            Debug.Log("PIECEDATA REVIEW IS NULL");
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
    }

    public void SetPreviewPiece(PieceData pieceData, Vector2Int origin, Sprite sprite, bool isReverse)
    {
        if(pieceData == null)
        {
            Debug.Log("PIECEDATA REVIEW IS NULL");
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

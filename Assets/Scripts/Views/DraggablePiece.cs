using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePiece : MonoBehaviour
{
    private int indexBatch;

    [SerializeField] private Vector2 rangeInteract;

    

    private Vector2 pivot;


    [SerializeField] private PieceData _myPieceData ;
    public PieceData MyPieceData => _myPieceData;
    private Vector3 _originalPosition;

    public Vector3 OriginalPosition => _originalPosition;

    [SerializeField] private PieceRenderer pieceRenderer;
    private BatchController batchController;

    private Vector2 currentPlacePosition;

    // Chỉ piece vừa place thành công mới được spawn floating text cho lượt đó.
    private bool _awaitingScoreForPlacedPiece;

    

    private bool isDragging;

    

    void Awake()
    {
        pieceRenderer = GetComponent<PieceRenderer>();
        batchController = GetComponentInParent<BatchController>();
        _originalPosition = transform.position;
    }

    public void SetRadInteract(Vector2 _rangeInteract)
    {
        rangeInteract = _rangeInteract;
    }

    public void SetPivot(float x, float y)
    {
        pivot.x = x;
        pivot.y = y;
    }

    public void Setup(PieceData data, int _index)
    {
        indexBatch = _index;
        pieceRenderer.ClearVisual();
        _myPieceData  = data;
        if(data == null)
        {
            return;
        }
        pieceRenderer.RenderPiece(data);
        transform.DOScale(0.5f, 0.2f);
    }

    void OnEnable()
    {
        EventBus.Instance.Subscribe<OnDragStart>(OnBeginDrag);
        EventBus.Instance.Subscribe<OnDragUpdate>(OnDrag);
        EventBus.Instance.Subscribe<OnDragEnd>(OnEndDrag);
        EventBus.Instance.Subscribe<OnAddScore>(SpawnFloatingText);
    }

    void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnDragStart>(OnBeginDrag);
        EventBus.Instance.UnSubscribe<OnDragUpdate>(OnDrag);
        EventBus.Instance.UnSubscribe<OnDragEnd>(OnEndDrag);
         EventBus.Instance.UnSubscribe<OnAddScore>(SpawnFloatingText);
    }
    public void OnBeginDrag(OnDragStart onDragStart)
    {
        Vector2 range = onDragStart.pos - (Vector2)transform.position;
        if(Mathf.Abs(range.x) <= rangeInteract.x && Mathf.Abs(range.y) <= rangeInteract.y)
        {
            // Safety check - ensure piece data is valid before starting drag
            if (_myPieceData == null)
            {
                Debug.LogWarning("Cannot begin drag - piece data is null");
                return;
            }
            
            transform.position = onDragStart.pos;
            isDragging = true;
            transform.DOKill();
            transform.DOScale(1f, 0.1f);
            batchController.SetCurrentHoldingPiece(_myPieceData);
            batchController.SetCurrentSpritePiece(pieceRenderer.SpriteBlock);
            batchController.SetPivotPiece(pivot);
        }
    }

    public void OnDrag(OnDragUpdate onDragUpdate)
    {
        if (isDragging)
        {
            transform.position = new Vector3(onDragUpdate.pos.x, onDragUpdate.pos.y, 0f);
        }
    }

    public void OnEndDrag(OnDragEnd onDragEnd)
    {
        if (isDragging)
        {
            isDragging = false;
            
            if(_myPieceData == null)
            {
                Debug.Log("CURRENT PIECE HOLDING IS NULL");
                return;
            }
            Vector2 placePosition = onDragEnd.pos + pivot;
            currentPlacePosition = placePosition;
            _awaitingScoreForPlacedPiece = true;
            if( GameManager.Instance.GridController.TryPlacePiece(_myPieceData, GridCoordinateConverter.WorldToGrid(placePosition), pieceRenderer.SpriteBlock))
            {
                
                batchController.SetCurrentHoldingPiece(null);
                batchController.SetCurrentSpritePiece(null);
                batchController.SetPivotPiece(Vector2.zero);
                transform.DOScale(0, 0.1f).OnComplete(() =>
                {
                    
                    transform.DOMove(_originalPosition, 0f).OnComplete(() =>
                    {
                        pieceRenderer.ClearVisual();        
                        GameManager.Instance.GameStateModel.RemovePieceFromBatch(indexBatch);
                        
                    });          
                });   
            }
            else
            {
                _awaitingScoreForPlacedPiece = false;
                batchController.SetCurrentHoldingPiece(null);
                batchController.SetCurrentSpritePiece(null);
                batchController.SetPivotPiece(Vector2.zero);
                transform.DOMove(_originalPosition, 0.3f).SetEase(Ease.OutBack);
                transform.DOScale(0.5f, 0.3f);

            }
            
            
        }

    }

    public void SpawnFloatingText(OnAddScore onAddScore)
    {
        if (!_awaitingScoreForPlacedPiece)
        {
            return;
        }

        _awaitingScoreForPlacedPiece = false;

        if(onAddScore.AddedScore == 0)
        {
            return;
        }
        
        // Score text hiển thị tại vị trí điểm đặt piece
        Vector2 scorePosition = currentPlacePosition;
        
        EventBus.Instance.Publish(new OnAddFloatingText
        {
            Content = "+" + onAddScore.AddedScore,
            FontSize = 70,
            Position = scorePosition,
            TextColor = GameManager.Instance.ThemeData.scoreFloatingText,
            PositionType = FloatingTextPositionType.WorldPosition
        });

        if(onAddScore.CurrentCombo < 2){
            return;
        }

        // Combo text hiển thị giữa màn hình UI
        EventBus.Instance.Publish(new OnAddFloatingText
        {
            Content = "COMBO X" + onAddScore.CurrentCombo,
            FontSize = 110,
            Position = Vector2.zero,
            TextColor = GameManager.Instance.ThemeData.comboFloatingText,
            PositionType = FloatingTextPositionType.ScreenCenter
        });
    }


    
}

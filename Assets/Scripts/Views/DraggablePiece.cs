using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePiece : MonoBehaviour
{
    private int indexBatch;

    [SerializeField] private float radInteract;

    private Vector2 pivot;


    [SerializeField] private PieceData _myPieceData ;
    public PieceData MyPieceData => _myPieceData;
    private Vector3 _originalPosition;

    public Vector3 OriginalPosition => _originalPosition;

    [SerializeField] private PieceRenderer pieceRenderer;

    

    private bool isDragging;

    void Awake()
    {
        pieceRenderer = GetComponent<PieceRenderer>();
        _originalPosition = transform.position;
    }

    public void SetRadInteract(float _radInteract)
    {
        radInteract = _radInteract;
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
        transform.DOScale(0.7f, 0.2f);
    }

    void OnEnable()
    {
        EventBus.Instance.Subscribe<OnDragStart>(OnBeginDrag);
        EventBus.Instance.Subscribe<OnDragUpdate>(OnDrag);
        EventBus.Instance.Subscribe<OnDragEnd>(OnEndDrag);
    }

    void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnDragStart>(OnBeginDrag);
        EventBus.Instance.UnSubscribe<OnDragUpdate>(OnDrag);
        EventBus.Instance.UnSubscribe<OnDragEnd>(OnEndDrag);
    }
    public void OnBeginDrag(OnDragStart onDragStart)
    {
        
        if((onDragStart.pos - ((Vector2)transform.position + pivot)).sqrMagnitude <= radInteract*radInteract)
        {
            // Safety check - ensure piece data is valid before starting drag
            if (_myPieceData == null)
            {
                Debug.LogWarning("Cannot begin drag - piece data is null");
                return;
            }
            
            transform.position = onDragStart.pos;
            isDragging = true;
            transform.DOScale(1f, 0.1f);
            this.GetComponentInParent<BatchController>().SetCurrentHoldingPiece(_myPieceData);
            this.GetComponentInParent<BatchController>().SetCurrentSpritePiece(pieceRenderer.SpriteBlock);
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
            if( GameManager.Instance.GridController.TryPlacePiece(_myPieceData, GridCoordinateConverter.WorldToGrid(placePosition), pieceRenderer.SpriteBlock))
            {
                pieceRenderer.ClearVisual();
                this.GetComponentInParent<BatchController>().SetCurrentHoldingPiece(null);
                this.GetComponentInParent<BatchController>().SetCurrentSpritePiece(null);
                transform.DOScale(0, 0.2f).OnComplete(() =>
                {
                    
                    transform.DOMove(_originalPosition, 0f).OnComplete(() =>
                    {        
                        GameManager.Instance.GameStateModel.RemovePieceFromBatch(indexBatch);
                        
                    });          
                });   
            }
            else
            {
                this.GetComponentInParent<BatchController>().SetCurrentHoldingPiece(null);
                this.GetComponentInParent<BatchController>().SetCurrentSpritePiece(null);
                transform.DOMove(_originalPosition, 0.3f).SetEase(Ease.OutBack);
                transform.DOScale(0.7f, 0.3f);

            }
            
            
        }

    }


    
}

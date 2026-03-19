using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class DraggablePiece : MonoBehaviour
{

    [SerializeField] private float radInteract;
    [SerializeField] private PieceData _myPieceData ;
    public PieceData MyPieceData => _myPieceData;
    private Vector3 _originalPosition;

    public Vector3 OriginalPosition => _originalPosition;

    [SerializeField] private SpriteRenderer visual;

    

    private bool isDragging;

    void Awake()
    {
        _originalPosition = transform.position;
    }

    public void Setup(PieceData data)
    {
        _myPieceData  = data;
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
        if((onDragStart.pos - (Vector2)transform.position).sqrMagnitude <= radInteract*radInteract)
        {
            transform.position = onDragStart.pos;
            isDragging = true;
            transform.DOScale(1.2f, 0.1f);
        }
    }

    public void OnDrag(OnDragUpdate onDragUpdate)
    {
        if (isDragging)
        {
            transform.DOMove(onDragUpdate.pos, 0.1f);
        }
    }

    public void OnEndDrag(OnDragEnd onDragEnd)
    {
        if (isDragging)
        {
            if( GameManager.Instance.GridController.TryPlacePiece(MyPieceData, GridCoordinateConverter.WorldToGrid(onDragEnd.pos)))
            {
                transform.DOScale(0, 0.2f).OnComplete(() =>
                {
                    this.gameObject.SetActive(false);
                });
            }
            else
            {
                transform.DOMove(_originalPosition, 0.3f).SetEase(Ease.OutBack);
                transform.DOScale(1f, 0.3f);
            }
            
        }

    }


    
}

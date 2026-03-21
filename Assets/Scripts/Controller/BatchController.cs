using System.Collections.Generic;
using UnityEngine;

public class BatchController : MonoBehaviour
{
    [SerializeField] List<DraggablePiece> _listPiece;

    public List<DraggablePiece> ListPiece => _listPiece;

    [SerializeField] private PieceData _currentHoldingPiece;

    public PieceData CurrentHoldingPiece => _currentHoldingPiece;

    [SerializeField] private Sprite _currentSpritePiece;

    public Sprite CurrentSpritePiece => _currentSpritePiece;

    [SerializeField] private Vector2 _pivotPiece;

    public Vector2 PivotPiece => _pivotPiece;

    void OnEnable()
    {
        EventBus.Instance.Subscribe<OnBatchChanged>(SetUpBatch);
    }
    void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnBatchChanged>(SetUpBatch);
    }

    public void SetPivotPiece(Vector2 pivot)
    {
        _pivotPiece = pivot;
    }

    public void SetCurrentHoldingPiece(PieceData currentHoldingPiece)
    {
        _currentHoldingPiece = currentHoldingPiece;
    }

    public void SetCurrentSpritePiece(Sprite currentSpritePiece)
    {
        _currentSpritePiece = currentSpritePiece;
    }


    public void SetUpBatch( OnBatchChanged onBatchChanged)
    {
        if(_listPiece.Count < 3)
        {
            Debug.Log("BATCH DONT HAVE ENOUGH SLOT");
            return;
        }
        _listPiece[0].Setup( onBatchChanged.FirstPiece, 0);
        _listPiece[1].Setup(onBatchChanged.SecondPiece, 1);
        _listPiece[2].Setup(onBatchChanged.ThirdPiece, 2);
    }


}

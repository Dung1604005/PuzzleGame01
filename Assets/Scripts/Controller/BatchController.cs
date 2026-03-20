using System.Collections.Generic;
using UnityEngine;

public class BatchController : MonoBehaviour
{
    [SerializeField] List<DraggablePiece> _listPiece;

    public List<DraggablePiece> ListPiece => _listPiece;

    void OnEnable()
    {
        EventBus.Instance.Subscribe<OnBatchChanged>(SetUpBatch);
    }
    void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnBatchChanged>(SetUpBatch);
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

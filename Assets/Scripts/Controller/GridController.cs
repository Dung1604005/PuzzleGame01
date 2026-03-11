using UnityEngine;

public class GridController : MonoBehaviour
{
    private GridModel _gridModel;

    public void Init(GridModel gridModel)
    {
        _gridModel = gridModel;
    }
    public bool CanPlacePiece(PieceData piece, Vector2Int origin)
    {
        bool canPlace = true;

        foreach (Vector2Int offSet in piece.CellOffsets)
        {
            Vector2Int placePosition = origin + offSet;
            // Kiem tra xem co rong + hop le khong
            if (!_gridModel.IsEmpty(placePosition) || !_gridModel.IsInBound(placePosition))
            {
                canPlace = false;
                break;
            }
        }
        return canPlace;
    }

    public bool TryPlacePiece(PieceData piece, Vector2Int origin)
    {
        if (CanPlacePiece(piece, origin))
        {
            foreach (Vector2Int offSet in piece.CellOffsets)
            {
                Vector2Int placePosition = origin + offSet;
                _gridModel.SetCell(placePosition, piece.Value);
            }
            return true;
        }
        else
        {
            return false;
        }
    }




}

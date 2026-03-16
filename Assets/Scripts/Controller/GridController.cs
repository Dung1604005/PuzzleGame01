using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    private GridModel _gridModel;

    public GridModel GridModel => _gridModel;

    private ScoreModel _scoreModel;

    private GameStateModel _gameStateModel;

    public void Init(GridModel gridModel, ScoreModel scoreModel, GameStateModel gameStateModel)
    {
        _gridModel = gridModel;
        _scoreModel = scoreModel;
        _gameStateModel = gameStateModel;
    }
    //Kiem tra xem co the dat khong
    public bool CanPlacePiece(PieceData piece, Vector2Int origin)
    {
        bool canPlace = true;

        foreach (Vector2Int offSet in piece.CellOffsets)
        {
            Vector2Int placePosition = origin + offSet;
            // Kiem tra xem co rong + hop le khong
            if (!_gridModel.IsInBound(placePosition)||!_gridModel.IsEmpty(placePosition) )
            {
                canPlace = false;
                break;
            }
        }
        return canPlace;
    }

    public bool CanPlacePieceAnyWhere(PieceData piece)
    {
        
        for(int i = 0; i < _gridModel.Row; i++)
        {
            for(int j =0; j < _gridModel.Collumn; j++)
            {
                if(CanPlacePiece(piece, new Vector2Int(i, j)))
                {
                    return true;
                }
            }
        }
        return false;
    }
    // Tien hanh dat khoi

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

    // Xoa khoi va tinh diem 
    public void CheckAndClearMatches()
    {
        List<Vector2Int> cellToRemoves = LineClearCalculator.GetCellsToRemove(_gridModel.Grid);

        List<int> fullRows = LineClearCalculator.FindFullRows(_gridModel.Grid);

        List<int> fullCollumn = LineClearCalculator.FindFullCollumn(_gridModel.Grid);

        // Xoa cac cell
        foreach(Vector2Int posCell in cellToRemoves)
        {
            _gridModel.SetCell(posCell, 0);
        }

        // Cong diem
        _scoreModel.AddScore(fullCollumn.Count + fullRows.Count,cellToRemoves.Count);
        // Ban su kien 
        _gridModel.PublishOnLineCleared(cellToRemoves);
    }

    public bool IsGameOver()
    {
        foreach(PieceData pieceData in _gameStateModel.AvailablePieces)
        {
            if (CanPlacePieceAnyWhere(pieceData))
            {
                return false;
            }
        }
        return true;
    }



}

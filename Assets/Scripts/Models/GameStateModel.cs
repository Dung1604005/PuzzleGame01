using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateModel
{
    private GameState currentGameState = GameState.Menu;

    public GameState CurrentGameState => currentGameState;

    [Header("DIFFICULTY")]

    private LevelDifficulty currentLevelDifficulty;

    public LevelDifficulty CurrentLevelDifficulty => currentLevelDifficulty;

    [Header("PIECE")]

    private List<PieceData> availablePieces = new List<PieceData>();

    public List<PieceData> AvailablePieces => availablePieces;

    private List<PieceData> bagPiece = new List<PieceData>();

    public List<PieceData> BagPiece => bagPiece;


    public event Action<GameState> OnStateChanged;

    public event Action<LevelDifficulty> OnDifficultyChanged;

    public void TransitionTo(GameState newGameState)
    {
        currentGameState = newGameState;
        OnStateChanged?.Invoke(newGameState);
    }

    public void UpgradeDifficulty(LevelDifficulty levelDifficulty)
    {
        currentLevelDifficulty = levelDifficulty;
        OnDifficultyChanged?.Invoke(levelDifficulty);
    }

    public void AddPiece(PieceData pieceData)
    {

        availablePieces.Add(pieceData);
        RemovePieceFromBag(pieceData);
    }
    public void ResetBagPiece(List<PieceData> alllPieces, BagComposition bagComposition)
    {
        bagPiece = PieceSpawner.PickBagOfPiece(alllPieces, currentLevelDifficulty, bagComposition);
    }
    public void RemovePieceFromBag(PieceData pieceData)
    {
        int index = bagPiece.FindIndex(p => p.PieceID == pieceData.PieceID);

        if (index != -1) 
        {
            
            Debug.Log($"PIECE {pieceData.PieceID} has been removed from bag");

            
            bagPiece.RemoveAt(index);
        }
        else
        {
            Debug.LogWarning($"CANNOT FIND PIECE {pieceData.PieceID} TO REMOVE FROM BAG");
        }
    }


}
public enum GameState { Menu, Playing, Paused, GameOver };
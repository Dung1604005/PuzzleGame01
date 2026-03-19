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


    

    

    public void TransitionTo(GameState newGameState)
    {
        currentGameState = newGameState;
        EventBus.Instance.Publish(new OnStateChanged
        {
            GameState = newGameState
        });
        
    }

    public void ChangeDifficulty(LevelDifficulty levelDifficulty)
    {
        currentLevelDifficulty = levelDifficulty;
        EventBus.Instance.Publish(new OnDifficultyChanged
        {
            LevelDifficulty = levelDifficulty
        }     
        );
        
    }

    public void AddPieceToBatch(PieceData pieceData)
    {

        availablePieces.Add(pieceData);
        RemovePieceFromBag(pieceData);
    }

    


    public void ClearBatch()
    {
        availablePieces.Clear();
    }
    public void ResetBagPiece(List<PieceData> alllPieces, DifficultyConfig difficultyConfig)
    {
        BagComposition bagComposition = difficultyConfig.Level1;
        if(currentLevelDifficulty == LevelDifficulty.EASY)
        {
            bagComposition = difficultyConfig.Level1;
        }
        else if(currentLevelDifficulty == LevelDifficulty.NORMAL)
        {
            bagComposition = difficultyConfig.Level2;
        }
        else
        {
            bagComposition = difficultyConfig.Level3;
        }
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
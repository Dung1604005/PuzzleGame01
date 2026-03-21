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

    private List<PieceData> batchPieces = new List<PieceData>();

    public List<PieceData> BatchPieces => batchPieces;

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
        // Prevent adding null pieces to batch
        if (pieceData == null)
        {
            Debug.LogWarning("Tried to add null piece to batch - skipping");
            return;
        }

        batchPieces.Add(pieceData);
        RemovePieceFromBag(pieceData);
    }

    public void RemovePieceFromBatch(int index)
    {
        if (index < 0 || index >= 3)
        {
            Debug.Log("INDEX IS OUT OF BOUND OF BATCH");
            return;
        }

        batchPieces[index] = null;
        bool isBatchEmpty = true;
        for (int i = 0; i < 3; i++)
        {
            if (batchPieces[i] != null)
            {
                isBatchEmpty = false;
                break;
            }
        }

        if (isBatchEmpty)
        {
            batchPieces.Clear();
            PieceSpawner.SpawnNewBatch(this, GameManager.Instance.GridController, bagPiece);
        }

    }
    public void ClearBatch()
    {
        batchPieces.Clear();
    }
    public void ResetBagPiece(List<PieceData> alllPieces, DifficultyConfig difficultyConfig)
    {
        BagComposition bagComposition = difficultyConfig.Level1;
        if (currentLevelDifficulty == LevelDifficulty.EASY)
        {
            bagComposition = difficultyConfig.Level1;
        }
        else if (currentLevelDifficulty == LevelDifficulty.NORMAL)
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
        // Safety check to prevent null reference
        if (pieceData == null)
        {
            Debug.LogWarning("Tried to remove null piece from bag");
            return;
        }
        
        int index = bagPiece.FindIndex(p => p.PieceID == pieceData.PieceID);

        if (index != -1)
        {

            Debug.Log($"PIECE {pieceData.PieceID} has been removed from bag");


            bagPiece.RemoveAt(index);
            if(bagPiece.Count == 0)
            {
                ResetBagPiece(GameManager.Instance.PieceDatas, GameManager.Instance.DifficultyConfig);
            }
        }
        else
        {
            Debug.LogWarning($"CANNOT FIND PIECE {pieceData.PieceID} TO REMOVE FROM BAG");
        }
    }


}
public enum GameState { Menu, Playing, Paused, GameOver };
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PieceSpawner
{
    public static PieceData PickPieceByDifficulty(List<PieceData> allPieces, LevelDifficulty levelDifficultyPiece)
    {
        int sumWeight = 0;

        foreach (PieceData pieceData in allPieces)
        {
            if (levelDifficultyPiece == pieceData.LevelPiece)
            {
                sumWeight += pieceData.SpawnWeight;
            }

        }
        int randWeight = UnityEngine.Random.Range(1, sumWeight + 1);
        int currentWeight = 0;
        foreach (PieceData pieceData in allPieces)
        {
            if (levelDifficultyPiece == pieceData.LevelPiece)
            {
                currentWeight += pieceData.SpawnWeight;
                if (currentWeight >= randWeight)
                {
                    return pieceData;

                }
            }
        }
        Debug.Log("CAN'T PICK PIECE");
        return null;

    }
    public static PieceData PickRandomPiece(List<PieceData> allPieces)
    {
        int sumWeight = 0;

        foreach (PieceData pieceData in allPieces)
        {

            sumWeight += pieceData.SpawnWeight;


        }
        int randWeight = UnityEngine.Random.Range(1, sumWeight + 1);
        int currentWeight = 0;
        foreach (PieceData pieceData in allPieces)
        {

            currentWeight += pieceData.SpawnWeight;
            if (currentWeight >= randWeight)
            {
                return pieceData;

            }

        }
        Debug.Log("CAN'T PICK PIECE");
        return null;
    }
    public static List<PieceData> PickBagOfPiece(List<PieceData> allPieces, LevelDifficulty levelDifficulty,
     BagComposition bagComposition)
    {
        List<PieceData> bagPiece = new List<PieceData>();
        
        for (int quantity = 1; quantity <= bagComposition.easyCount; quantity++)
        {
            PieceData pieceData = PickPieceByDifficulty(allPieces, LevelDifficulty.EASY);
            if(pieceData != null)
            {
                bagPiece.Add(pieceData);
            }
            
        }
        for (int quantity = 1; quantity <= bagComposition.mediumCount; quantity++)
        {
            PieceData pieceData = PickPieceByDifficulty(allPieces, LevelDifficulty.NORMAL);
            if(pieceData != null)
            {
                bagPiece.Add(pieceData);
            }
        }
        for (int quantity = 1; quantity <= bagComposition.hardCount; quantity++)
        {
            PieceData pieceData = PickPieceByDifficulty(allPieces, LevelDifficulty.HARD);
            if(pieceData != null)
            {
                bagPiece.Add(pieceData);
            }
        }
        return bagPiece;
    }

    public static void SpawnNewBatch(GameStateModel gameState, GridController gridController, List<PieceData> pool)
    {
        gameState.ClearBatch();
        // Lay 3 piece

        //Lay piece co the dat vao
        bool findSavePiece = false;
        List<PieceData> pieceDatas = new List<PieceData>();

        foreach(PieceData pieceData in pool)
        {
            if (gridController.CanPlacePieceAnyWhere(pieceData))
            {
                gameState.AddPieceToBatch(pieceData);
                findSavePiece = true;
                pieceDatas.Add(pieceData);
                break;
            }
        }
        if (!findSavePiece)
        {
            PieceData randomPiece = PickRandomPiece(pool);
            pieceDatas.Add(randomPiece);
            gameState.AddPieceToBatch(randomPiece);
        }

        // Lay random2  piece con lai
        PieceData randomPiece2 = PickRandomPiece(pool);
        if (randomPiece2 != null)
        {
            pieceDatas.Add(randomPiece2);
            gameState.AddPieceToBatch(randomPiece2);
        }
        else
        {
            Debug.LogError("Failed to pick piece 2 from pool - pool may be empty");
        }
        
        PieceData randomPiece3 = PickRandomPiece(pool);
        if (randomPiece3 != null)
        {
            pieceDatas.Add(randomPiece3);
            gameState.AddPieceToBatch(randomPiece3);
        }
        else
        {
            Debug.LogError("Failed to pick piece 3 from pool - pool may be empty");
        }

        // Only publish event if we have enough pieces (safety check)
        if (pieceDatas.Count >= 3)
        {
            EventBus.Instance.Publish(new OnBatchChanged
            {
                FirstPiece = pieceDatas[0],
                SecondPiece = pieceDatas[1],
                ThirdPiece = pieceDatas[2]
            });
        }
        else
        {
            Debug.LogError($"Failed to create batch with 3 pieces. Only got {pieceDatas.Count} pieces.");
        }

        Debug.Log("SUCCEED CREATE BATCH PIECE");
        

    }
}

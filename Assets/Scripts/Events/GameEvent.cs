using System.Collections.Generic;
using UnityEngine;



public struct OnLineCleared: IEvent
{
    public List<Vector2Int> position;
}

public struct  OnCellChanged: IEvent
{
    public Vector2Int position;

    public int newValue;

    public Sprite newSprite;
}

public struct OnPiecePlaced : IEvent
{
    public PieceData pieceData;
    public Vector2Int origin;
}

public struct OnDifficultyChanged: IEvent
{
    public LevelDifficulty LevelDifficulty;
}

public struct OnStateChanged: IEvent
{
    public GameState GameState;
}

public struct OnScoreUpdated: IEvent
{
    public int CurrentScore;

    public int HighScore;
}

public struct OnBatchChanged : IEvent
{
    public PieceData FirstPiece;

    public PieceData SecondPiece;

    public PieceData ThirdPiece;
}
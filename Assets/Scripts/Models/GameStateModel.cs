using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateModel
{
    private GameState currentGameState;

    public GameState CurrentGameState => currentGameState;


    private List<PieceData> availablePieces ;

    public List<PieceData> AvailablePieces  => availablePieces ;

    public event Action<GameState> OnStateChanged;

    public void TransitionTo(GameState newGameState)
    {
        currentGameState = newGameState;
    }


}
public enum GameState { Menu, Playing, Paused, GameOver };
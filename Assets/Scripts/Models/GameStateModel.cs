using System;
using System.Collections.Generic;
using UnityEngine;

public class GameStateModel
{
    private GameState currentGameState = GameState.Menu;

    public GameState CurrentGameState => currentGameState;


    private List<PieceData> availablePieces ;

    public List<PieceData> AvailablePieces  => availablePieces ;

    public event Action<GameState> OnStateChanged;

    public void TransitionTo(GameState newGameState)
    {
        currentGameState = newGameState;
        OnStateChanged?.Invoke(newGameState);
    }


}
public enum GameState { Menu, Playing, Paused, GameOver };
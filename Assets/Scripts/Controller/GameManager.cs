using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("REFERENCES")]

    [SerializeField] private GridConfig _gridConfig;

    public GridConfig GridConfig => _gridConfig;
    private GridModel _gridModel;
    public GridModel GridModel => _gridModel;

    private GameStateModel _gameStateModel;

    public GameStateModel GameStateModel => _gameStateModel;

    private ScoreModel _scoreModel;

    public ScoreModel ScoreModel => _scoreModel;

    [SerializeField] private GridController _gridController;

    public GridController GridController => _gridController;



    [Header("Data")]

    [SerializeField] private List<PieceData> pieceDatas;

    public List<PieceData> PieceDatas => pieceDatas;

    [SerializeField] private DifficultyConfig difficultyConfig;

    public DifficultyConfig DifficultyConfig => difficultyConfig;

    [SerializeField] private ThemeData themeData;

    public ThemeData ThemeData => themeData;

    [SerializeField] private GameObject blockPrefab;

    public GameObject BlockPrefab => blockPrefab;

    

    

    public void StartGame()
    {
        _gridModel = new GridModel();
        _gridModel.InitGrid(8, 8);
        _gameStateModel = new GameStateModel();
        _scoreModel = new ScoreModel();
        _gameStateModel.TransitionTo(GameState.Playing);

        _gridController.Init(_gridModel, _scoreModel, _gameStateModel);

        _gameStateModel.ResetBagPiece(pieceDatas, difficultyConfig);
        
        PieceSpawner.SpawnNewBatch(_gameStateModel, _gridController, _gameStateModel.BagPiece);
    }

    public void GameOver()
    {
        _gameStateModel.TransitionTo(GameState.GameOver);
        // Sau phai luu HighScore + trang thai gameStateModel
    }

    public void RestartGame()
    {
        _gridModel.Clear();
        _gameStateModel.TransitionTo(GameState.Playing);
        _gameStateModel.ResetBagPiece(pieceDatas, difficultyConfig);
        PieceSpawner.SpawnNewBatch(_gameStateModel, _gridController, pieceDatas);
        _scoreModel.Reset();
    
    }
    public void OnPiecePlaced()
    {
        _gridController.CheckAndClearMatches();
        if (_gridController.IsGameOver())
        {
            GameOver();
        }
    } // Sau khi đặt: CheckClear → check GameOver → spawn mới

    

    void Start()
    {
        StartGame();
    }

    
    


}

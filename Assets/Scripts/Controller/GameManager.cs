using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Header("REFERENCES")]

    [SerializeField] private GridConfig _gridConfig;

    public GridConfig GridConfig => _gridConfig;
    private GridModel _gridModel;
    public GridModel GridModel => _gridModel;

    [SerializeField] private GameStateModel _gameStateModel;

    public GameStateModel GameStateModel => _gameStateModel;

    [SerializeField] private ScoreModel _scoreModel;

    public ScoreModel ScoreModel => _scoreModel;

    [SerializeField] private GridController _gridController;

    public GridController GridController => _gridController;

    [SerializeField] private BatchController _batchController;

    public BatchController BatchController => _batchController;



    [Header("Data")]

    [SerializeField] private List<PieceData> pieceDatas;

    public List<PieceData> PieceDatas => pieceDatas;

    [SerializeField] private DifficultyConfig difficultyConfig;

    public DifficultyConfig DifficultyConfig => difficultyConfig;

    [SerializeField] private ThemeData themeData;

    public ThemeData ThemeData => themeData;

    [SerializeField] private GameObject blockPrefab;

    public GameObject BlockPrefab => blockPrefab;

    [SerializeField] private List<ThemeData> themeDataList = new List<ThemeData>();



    public void OnEnable()
    {
        EventBus.Instance.Subscribe<OnPiecePlaced>(OnPiecePlacedEvent);
        EventBus.Instance.Subscribe<OnScoreUpdated>(UpdateDifficulty);
    }

    public void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnPiecePlaced>(OnPiecePlacedEvent);
        EventBus.Instance.UnSubscribe<OnScoreUpdated>(UpdateDifficulty);
    }

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

        EventBus.Instance.Publish(new OnChangeTheme { });
    }

    public void ContinueGame()
    {
        _gameStateModel.TransitionTo(GameState.Playing);
    }
    public void GameOver()
    {
        _gameStateModel.TransitionTo(GameState.GameOver);
        EventBus.Instance.Publish(new OnGameOver
        {
            CurrentScore = ScoreModel.CurrentScore
        });
        // Sau phai luu HighScore + trang thai gameStateModel
    }

    public void RestartGame()
    {

        _gameStateModel.TransitionTo(GameState.Playing);
        _gameStateModel.ResetBagPiece(pieceDatas, difficultyConfig);
        PieceSpawner.SpawnNewBatch(_gameStateModel, _gridController, pieceDatas);
        _scoreModel.Reset();
        _gridModel.Clear();

    }

    public void UpdateDifficulty(OnScoreUpdated onScoreUpdated)
    {
        if (onScoreUpdated.CurrentScore < GameConfig.SCORED_MILESTONES_EASY)
        {
            ChangeTheme(LevelDifficulty.EASY, themeDataList[0]);
            _gameStateModel.ChangeDifficulty(LevelDifficulty.EASY);

        }
        else if (onScoreUpdated.CurrentScore < GameConfig.SCORED_MILESTONES_NORMAL)
        {
            ChangeTheme(LevelDifficulty.NORMAL, themeDataList[1]);
            _gameStateModel.ChangeDifficulty(LevelDifficulty.NORMAL);

        }
        else
        {
            ChangeTheme(LevelDifficulty.HARD, themeDataList[2]);
            _gameStateModel.ChangeDifficulty(LevelDifficulty.HARD);

        }
    }

    public void ChangeTheme(LevelDifficulty targetDifficulty, ThemeData _themeData)
    {
        if (targetDifficulty == _gameStateModel.CurrentLevelDifficulty)
        {
            return;
        }
        else
        {
            themeData = _themeData;
            EventBus.Instance.Publish(new OnChangeTheme { });
        }
    }
    public void OnPiecePlacedEvent(OnPiecePlaced onPiecePlaced)
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

        if (SaveManager.Instance.HasSave())
        {
            SaveManager.Instance.LoadSavedGame();
            if (_gridController.IsGameOver())
            {
                GameOver();
            }
        }
    }





}

public struct OnGameOver : IEvent
{
    public int CurrentScore;
}

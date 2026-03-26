using System;
using System.Collections;
using System.Collections.Generic;
using Solo.MOST_IN_ONE;
using UnityEngine;

public class SaveManager : Singleton<SaveManager>
{
    private const string SaveKey = "GAME_SAVE_DATA";
    private const string SoundStateKey = "SOUND_STATE";

    [Serializable]
    private class SaveData
    {
        public int version = 1;
        public int rows;
        public int columns;
        public int[] gridCells;
        public string[] gridSpriteNames;

        public string[] batchPieceIds;
        public string[] bagPieceIds;

        public int currentScore;
        public int highScore;
        public int comboCount;

        public int levelDifficulty;

        public bool isSoundOn;
        public bool isHapticOn;
    }

    public bool HasSave()
    {
        return PlayerPrefs.HasKey(SaveKey);
    }

    public void ClearSave()
    {
        PlayerPrefs.DeleteKey(SaveKey);
        PlayerPrefs.Save();
    }

    public void SaveCurrentGame()
    {
        if (!TryBuildSaveData(out SaveData data))
        {
            Debug.LogWarning("SaveManager: cannot save because game state is not initialized yet.");
            return;
        }

        string json = JsonUtility.ToJson(data);
        PlayerPrefs.SetString(SaveKey, json);
        PlayerPrefs.Save();
    }

    public bool LoadSavedGame()
    {
        if (!PlayerPrefs.HasKey(SaveKey))
        {
            return false;
        }

        string json = PlayerPrefs.GetString(SaveKey);
        if (string.IsNullOrWhiteSpace(json))
        {
            return false;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(json);
        if (!IsValidSaveData(data))
        {
            Debug.LogWarning("SaveManager: save data is invalid.");
            return false;
        }

        if (GameManager.Instance == null || GameManager.Instance.GridModel == null ||
            GameManager.Instance.GameStateModel == null || GameManager.Instance.ScoreModel == null)
        {
            Debug.LogWarning("SaveManager: game models are not ready for loading.");
            return false;
        }

        ApplySettings(data);
        ApplyScore(data);
        ApplyGameState(data);
        ApplyGrid(data);
        ApplyBagAndBatch(data);
        StartCoroutine(RefreshGridVisualNextFrame());

        return true;
    }

    private bool TryBuildSaveData(out SaveData data)
    {
        data = null;

        if (GameManager.Instance == null || GameManager.Instance.GridModel == null ||
            GameManager.Instance.GameStateModel == null || GameManager.Instance.ScoreModel == null)
        {
            return false;
        }

        GridModel gridModel = GameManager.Instance.GridModel;
        GameStateModel gameStateModel = GameManager.Instance.GameStateModel;
        ScoreModel scoreModel = GameManager.Instance.ScoreModel;

        data = new SaveData
        {
            rows = gridModel.Row,
            columns = gridModel.Collumn,
            gridCells = FlattenGrid(gridModel.Grid),
            gridSpriteNames = FlattenSpriteNames(gridModel.CellSprites),
            batchPieceIds = SerializePieceList(gameStateModel.BatchPieces),
            bagPieceIds = SerializePieceList(gameStateModel.BagPiece),
            currentScore = scoreModel.CurrentScore,
            highScore = scoreModel.HighScore,
            comboCount = scoreModel.ComboCount,
            levelDifficulty = (int)gameStateModel.CurrentLevelDifficulty,
            isSoundOn = PlayerPrefs.GetInt(SoundStateKey, 1) == 1,
            isHapticOn = MOST_HapticFeedback.HapticsEnabled
        };

        return true;
    }

    private bool IsValidSaveData(SaveData data)
    {
        if (data == null)
        {
            return false;
        }

        if (data.rows <= 0 || data.columns <= 0)
        {
            return false;
        }

        if (data.gridCells == null || data.gridCells.Length != data.rows * data.columns)
        {
            return false;
        }

        return true;
    }

    private static int[] FlattenGrid(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int columns = grid.GetLength(1);
        int[] flat = new int[rows * columns];
        int index = 0;

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                flat[index++] = grid[row, column];
            }
        }

        return flat;
    }

    private static string[] SerializePieceList(List<PieceData> pieces)
    {
        if (pieces == null)
        {
            return Array.Empty<string>();
        }

        string[] ids = new string[pieces.Count];
        for (int i = 0; i < pieces.Count; i++)
        {
            ids[i] = pieces[i] == null ? string.Empty : pieces[i].PieceID;
        }

        return ids;
    }

    private static string[] FlattenSpriteNames(Sprite[,] sprites)
    {
        if (sprites == null)
        {
            return Array.Empty<string>();
        }

        int rows = sprites.GetLength(0);
        int columns = sprites.GetLength(1);
        string[] flat = new string[rows * columns];

        int index = 0;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Sprite sprite = sprites[row, column];
                flat[index++] = sprite == null ? string.Empty : sprite.name;
            }
        }

        return flat;
    }

    private Dictionary<string, PieceData> BuildPieceLookup()
    {
        Dictionary<string, PieceData> lookup = new Dictionary<string, PieceData>();

        foreach (PieceData piece in GameManager.Instance.PieceDatas)
        {
            if (piece == null || string.IsNullOrEmpty(piece.PieceID))
            {
                continue;
            }

            if (!lookup.ContainsKey(piece.PieceID))
            {
                lookup.Add(piece.PieceID, piece);
            }
        }

        return lookup;
    }

    private List<PieceData> DeserializePieceList(string[] pieceIds, Dictionary<string, PieceData> lookup)
    {
        List<PieceData> list = new List<PieceData>();
        if (pieceIds == null)
        {
            return list;
        }

        for (int i = 0; i < pieceIds.Length; i++)
        {
            string id = pieceIds[i];
            if (string.IsNullOrEmpty(id))
            {
                list.Add(null);
                continue;
            }

            if (lookup.TryGetValue(id, out PieceData pieceData))
            {
                list.Add(pieceData);
            }
            else
            {
                Debug.LogWarning($"SaveManager: PieceID '{id}' not found in PieceDatas.");
                list.Add(null);
            }
        }

        return list;
    }

    private void ApplySettings(SaveData data)
    {
        PlayerPrefs.SetInt(SoundStateKey, data.isSoundOn ? 1 : 0);
        MOST_HapticFeedback.HapticsEnabled = data.isHapticOn;

        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.BgmSource.mute = !data.isSoundOn;
            AudioManager.Instance.SfxSource.mute = !data.isSoundOn;
        }

        PlayerPrefs.Save();
    }

    private void ApplyScore(SaveData data)
    {
        GameManager.Instance.ScoreModel.LoadState(data.currentScore, data.highScore, data.comboCount);
    }

    private void ApplyGameState(SaveData data)
    {
        LevelDifficulty levelDifficulty = (LevelDifficulty)data.levelDifficulty;

        GameManager.Instance.GameStateModel.ChangeDifficulty(levelDifficulty);
        GameManager.Instance.GameStateModel.TransitionTo(GameState.Playing);
    }

    private void ApplyGrid(SaveData data)
    {
        GridModel gridModel = GameManager.Instance.GridModel;
        Dictionary<string, Sprite> spriteLookup = BuildGridSpriteLookup();

        gridModel.InitGrid(data.rows, data.columns);

        int index = 0;
        for (int row = 0; row < data.rows; row++)
        {
            for (int column = 0; column < data.columns; column++)
            {
                int value = data.gridCells[index++];
                Vector2Int position = new Vector2Int(row, column);
                Sprite sprite = ResolveCellSprite(data, row, column, value, spriteLookup);

                gridModel.SetCell(position, value, sprite);
            }
        }
    }

    private Dictionary<string, Sprite> BuildGridSpriteLookup()
    {
        Dictionary<string, Sprite> lookup = new Dictionary<string, Sprite>();
        ThemeData theme = GameManager.Instance.ThemeData;

        AddSpriteToLookup(lookup, theme.GridSprite);

        if (theme.BlockSprites != null)
        {
            for (int i = 0; i < theme.BlockSprites.Count; i++)
            {
                AddSpriteToLookup(lookup, theme.BlockSprites[i]);
            }
        }

        return lookup;
    }

    private static void AddSpriteToLookup(Dictionary<string, Sprite> lookup, Sprite sprite)
    {
        if (sprite == null || string.IsNullOrEmpty(sprite.name))
        {
            return;
        }

        if (!lookup.ContainsKey(sprite.name))
        {
            lookup.Add(sprite.name, sprite);
        }
    }

    private Sprite ResolveCellSprite(SaveData data, int row, int column, int value, Dictionary<string, Sprite> spriteLookup)
    {
        if (value == 0)
        {
            return GameManager.Instance.ThemeData.GridSprite;
        }

        int index = row * data.columns + column;
        if (data.gridSpriteNames != null && index >= 0 && index < data.gridSpriteNames.Length)
        {
            string spriteName = data.gridSpriteNames[index];
            if (!string.IsNullOrEmpty(spriteName) && spriteLookup.TryGetValue(spriteName, out Sprite savedSprite))
            {
                return savedSprite;
            }
        }

        if (GameManager.Instance.ThemeData.BlockSprites != null && GameManager.Instance.ThemeData.BlockSprites.Count > 0)
        {
            return GameManager.Instance.ThemeData.BlockSprites[0];
        }

        return GameManager.Instance.ThemeData.GridSprite;
    }

    private void ApplyBagAndBatch(SaveData data)
    {
        Dictionary<string, PieceData> pieceLookup = BuildPieceLookup();

        List<PieceData> loadedBag = DeserializePieceList(data.bagPieceIds, pieceLookup);
        List<PieceData> loadedBatch = DeserializePieceList(data.batchPieceIds, pieceLookup);

        GameStateModel gameStateModel = GameManager.Instance.GameStateModel;

        gameStateModel.BagPiece.Clear();
        gameStateModel.BagPiece.AddRange(loadedBag);

        if (gameStateModel.BagPiece.Count == 0)
        {
            gameStateModel.ResetBagPiece(GameManager.Instance.PieceDatas, GameManager.Instance.DifficultyConfig);
        }

        gameStateModel.BatchPieces.Clear();

        if (loadedBatch.Count == 0)
        {
            loadedBatch.Add(null);
            loadedBatch.Add(null);
            loadedBatch.Add(null);
        }

        while (loadedBatch.Count < 3)
        {
            loadedBatch.Add(null);
        }

        if (loadedBatch.Count > 3)
        {
            loadedBatch.RemoveRange(3, loadedBatch.Count - 3);
        }

        gameStateModel.BatchPieces.AddRange(loadedBatch);

        bool batchHasPlayablePiece = false;
        for (int i = 0; i < gameStateModel.BatchPieces.Count; i++)
        {
            if (gameStateModel.BatchPieces[i] != null)
            {
                batchHasPlayablePiece = true;
                break;
            }
        }

        if (!batchHasPlayablePiece)
        {
            PieceSpawner.SpawnNewBatch(gameStateModel, GameManager.Instance.GridController, gameStateModel.BagPiece);
            return;
        }

        EventBus.Instance.Publish(new OnBatchChanged
        {
            FirstPiece = gameStateModel.BatchPieces[0],
            SecondPiece = gameStateModel.BatchPieces[1],
            ThirdPiece = gameStateModel.BatchPieces[2]
        });
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveCurrentGame();
        }
    }

    private void OnApplicationQuit()
    {
        SaveCurrentGame();
    }

    private IEnumerator RefreshGridVisualNextFrame()
    {
        yield return null;

        if (GameManager.Instance == null || GameManager.Instance.GridModel == null)
        {
            yield break;
        }

        GridModel gridModel = GameManager.Instance.GridModel;

        for (int row = 0; row < gridModel.Row; row++)
        {
            for (int column = 0; column < gridModel.Collumn; column++)
            {
                Vector2Int position = new Vector2Int(row, column);
                int value = gridModel.GetCell(position);
                Sprite sprite = gridModel.CellSprites[row, column];

                if (sprite == null)
                {
                    sprite = value == 0
                        ? GameManager.Instance.ThemeData.GridSprite
                        : ResolveFallbackBlockSprite();
                }

                gridModel.SetCell(position, value, sprite);
            }
        }
    }

    private Sprite ResolveFallbackBlockSprite()
    {
        if (GameManager.Instance.ThemeData.BlockSprites != null && GameManager.Instance.ThemeData.BlockSprites.Count > 0)
        {
            return GameManager.Instance.ThemeData.BlockSprites[0];
        }

        return GameManager.Instance.ThemeData.GridSprite;
    }
}

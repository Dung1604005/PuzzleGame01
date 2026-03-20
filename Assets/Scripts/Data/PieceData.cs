using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PieceData", menuName = "Scriptable Objects/PieceData")]
public class PieceData : ScriptableObject
{
    [SerializeField] private string _pieceID;

    public string PieceID => _pieceID;

    [SerializeField] private Vector2Int[] _cellOffsets;

    public Vector2Int[] CellOffsets => _cellOffsets;

    [SerializeField] private int _value;

    public int Value => _value;

    [SerializeField] private LevelDifficulty levelPiece;

    public LevelDifficulty LevelPiece =>levelPiece;

    [SerializeField] private int _spawnWeight;

    public int SpawnWeight => _spawnWeight;


}
[Serializable]
public enum LevelDifficulty
    {
       HARD,
       NORMAL,
       EASY 
    }

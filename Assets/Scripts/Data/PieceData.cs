using UnityEngine;

[CreateAssetMenu(fileName = "PieceData", menuName = "Scriptable Objects/PieceData")]
public class PieceData : ScriptableObject
{
    [SerializeField] private string _pieceID;

    public string PieceID => _pieceID;

    [SerializeField] private Vector2Int[] _cellOffsets;

    public Vector2Int[] CellOffsets => _cellOffsets;

    [SerializeField] private Sprite _tileSprite;

    public Sprite TileSprite => _tileSprite;

    [SerializeField] private int _value;

    public int Value => _value;

    [SerializeField] private int _spawnWeight;

    public int SpawnWeight => _spawnWeight;


}

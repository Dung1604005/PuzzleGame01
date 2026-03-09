using UnityEngine;

[CreateAssetMenu(fileName = "GridConfig", menuName = "Scriptable Objects/GridConfig")]
public class GridConfig : ScriptableObject
{
    public int gridWidth = 8;

    public int gridHeight = 8;

    public Vector2 cellSize = new Vector2(1f,1f);

    public Vector2 offsetCell = new Vector2(0.5f, 0.5f);

    [Header("Positioning")]

    [Tooltip("Tọa độ của ô góc dưới cùng bên trái")]

    public Vector2 gridOrigin;
}

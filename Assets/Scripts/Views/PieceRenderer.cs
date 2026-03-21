using System;
using UnityEngine;

public class PieceRenderer : MonoBehaviour
{

    private DraggablePiece draggablePiece;

    private Sprite _spriteBlock;

    public Sprite SpriteBlock => _spriteBlock;
    void Awake()
    {
        draggablePiece  = GetComponent<DraggablePiece>();
    }

    public void SetupHitbox(PieceData data)
    {
    
    if (draggablePiece == null)
    {
        Debug.LogError("Chưa gắn DraggablePiece vào Object Cha!");
        return;
    }

    
    float medX = 0f;
    float medY = 0f;

    foreach (Vector2Int celloffset in data.CellOffsets)
    {
        medX += celloffset.x;
        medY += celloffset.y;
    }
    medX /= data.CellOffsets.Length;
    medY /= data.CellOffsets.Length;

    // --- BƯỚC 2: Tìm min, max dựa trên TỌA ĐỘ THỰC TẾ SAU KHI RENDER ---
    float minX = 1000f, maxX = -1000f;
    float minY = 1000f, maxY = -1000f;

    foreach (Vector2Int offset in data.CellOffsets)
    {
        // Tọa độ thực tế của ô vuông sau khi bị bạn "ép" căn giữa
        float renderedX = offset.x - medX;
        float renderedY = offset.y - medY;

        // So sánh bằng renderedX và LƯU cũng bằng renderedX
        if (renderedX < minX) minX = renderedX;
        if (renderedX > maxX) maxX = renderedX;
        if (renderedY < minY) minY = renderedY;
        if (renderedY > maxY) maxY = renderedY;
    }

    // --- BƯỚC 3: Tính toán Size và Offset ---
    float width = (maxX - minX + 1f);
    float height = (maxY - minY + 1f);

    // Dung half-extents (+padding nho) de vung cham trung voi hinh piece.
    float hitPadding = 0.25f;
    draggablePiece.SetRadInteract(new Vector2(width * 0.5f + hitPadding, height * 0.5f + hitPadding));
    
    // Pivot dung de quy doi world -> origin grid cua piece (cell offset (0,0)).
    // Sau khi render, cell (0,0) nam tai local position (-medX, -medY).
    draggablePiece.SetPivot(-medX, -medY);
    
}

    public void RenderPiece(PieceData pieceData)
    {
        Sprite spriteBlock = GameManager.Instance.ThemeData.GetRandomSprite();
        _spriteBlock = spriteBlock;

        float medX = 0;
        float medY = 0;

        foreach (Vector2Int celloffset in pieceData.CellOffsets)
        {
            medX += celloffset.x;
            medY += celloffset.y;
        }
        medX /= pieceData.CellOffsets.Length;
        medY /= pieceData.CellOffsets.Length;

        foreach (Vector2Int celloffset in pieceData.CellOffsets)
        {
            GameObject gameObject = Instantiate(GameManager.Instance.BlockPrefab, this.transform);

            gameObject.transform.localPosition = new Vector3(celloffset.x - medX, celloffset.y - medY, 0f);

            gameObject.GetComponent<SpriteRenderer>().sprite = spriteBlock;
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 5;
        }

        SetupHitbox(pieceData);
    }

    public void ClearVisual()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}

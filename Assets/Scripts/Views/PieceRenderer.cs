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

    // Phóng to hitbox lên 20% cho dễ chạm
    
    draggablePiece.SetRadInteract(Mathf.Max(width, height)*0.7f);
    
    float offsetX = (minX + maxX) / 2f;
    float offsetY = (minY + maxY) / 2f ;

    draggablePiece.SetPivot(offsetX, offsetY);
    
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

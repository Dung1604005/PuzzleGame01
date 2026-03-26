using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFit : MonoBehaviour
{
    [Header("Cấu hình vùng Grid (World Space)")]
    [Tooltip("Độ rộng của Grid + viền mong muốn (ví dụ 10 unit)")]
    public float gridWorldWidth = 10f; 
    [Tooltip("Chiều cao của Grid + viền mong muốn (ví dụ 10 unit)")]
    public float gridWorldHeight = 10f;

    [Header("Cấu hình vùng UI (Screen Space)")]
    [Tooltip("Chiều cao tối thiểu cần chừa ra ở đỉnh (Score/Pause) tính bằng UNIT World (ví dụ 2)")]
    public float requiredTopPaddingWorld = 2.5f;
    [Tooltip("Chiều cao tối thiểu cần chừa ra ở đáy (Tray) tính bằng UNIT World (ví dụ 3)")]
    public float requiredBottomPaddingWorld = 3.5f;

    private Camera _cam;
    private float _basePosY;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        _basePosY = transform.position.y;
        AdjustCamera();
    }


    public void AdjustCamera()
    {
        if (_cam == null) return;

        
        float screenRatio = (float)Screen.width / (float)Screen.height; 

        // 2. Tính toán tổng chiều cao tối thiểu cần thiết trong thế giới game
        // Tổng = Chiều cao Grid + Chiều cao vùng UI Trên + Chiều cao vùng UI Dưới
        float totalRequiredHeight = gridWorldHeight + requiredTopPaddingWorld + requiredBottomPaddingWorld;

        // 3. Tính toán tỉ lệ "Ngưỡng" của bố cục game
        float targetRatio = gridWorldWidth / totalRequiredHeight;

        // 4. Quyết định chế độ Fit dựa trên việc so sánh tỉ lệ
        if (screenRatio >= targetRatio)
        {
            // --- Chế độ IPAD / Màn hình Vuông (Wider than target) ---
            // Màn hình quá bè ngang so với chiều cao game cần. Nếu Fit Width, Grid sẽ đè UI.
            // Giải pháp: Ép khóa theo chiều dọc (Height Fit) để đảm bảo UI fit.
            _cam.orthographicSize = totalRequiredHeight / 2f;
            
        }
        else
        {
            // --- Chế độ IPHONE / Màn hình Dài (Thinner than target) ---
            // Màn hình đủ dài để chứa UI ở trên/dưới.
            // Giải pháp: Khóa theo chiều ngang (Width Fit) để Grid to nhất có thể. (Bố cục cũ).
            _cam.orthographicSize = (gridWorldWidth / 2f) / screenRatio;
            Debug.Log($"[CameraFit] iPhone Detected. Fitting WIDTH. Size: {_cam.orthographicSize}");
        }

        // 5. Căn chỉnh vị trí Camera (Y Offset)
        // Vị trí Y lý tưởng = (Padding Dưới - Padding Trên) / 2
        float yOffset = (requiredBottomPaddingWorld - requiredTopPaddingWorld) / 2f;
        Vector3 newPos = transform.position;
        newPos.y = _basePosY + yOffset;
        transform.position = newPos;
    }
}
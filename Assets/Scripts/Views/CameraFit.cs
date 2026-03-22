using UnityEngine;

public class CameraFit : MonoBehaviour
{
    [Header("Cấu hình kích thước")]
    
    public float targetWorldWidth = 10f; 

    [Tooltip("Size mặc định tối thiểu (Giữ cho màn hình iPad không bị zoom quá to)")]
    public float minOrthographicSize = 6f;

    private Camera _cam;

    void Awake()
    {
        _cam = GetComponent<Camera>();
        AdjustCameraSize();
    }

    
    public void AdjustCameraSize()
    {
        // Thuật toán: Size = (Độ rộng mong muốn / 2) / Tỉ lệ màn hình hiện tại
        float requiredSize = (targetWorldWidth / 2f) / _cam.aspect;

        // Chọn kích thước lớn hơn giữa Size tối thiểu và Size tính toán được
        _cam.orthographicSize = Mathf.Max(minOrthographicSize, requiredSize);
    }
}
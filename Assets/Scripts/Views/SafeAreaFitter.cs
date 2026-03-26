using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _lastSafeArea = new Rect(0, 0, 0, 0);
    private Vector2Int _lastScreenSize;
    private ScreenOrientation _lastOrientation;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        ApplySafeArea();
    }

    private void OnEnable()
    {
        ApplySafeArea();
    }

    

    private void ApplySafeArea()
    {
        // Lấy thông số vùng an toàn từ thiết bị thực tế
        Rect safeArea = Screen.safeArea;
        

        // Lưu lại để so sánh cho frame sau
        _lastSafeArea = safeArea;
        _lastScreenSize = new Vector2Int(Screen.width, Screen.height);
        _lastOrientation = Screen.orientation;

        if (Screen.width <= 0 || Screen.height <= 0)
        {
            return;
        }

        // Quy đổi tọa độ Pixel của thiết bị sang tọa độ phần trăm (0.0 -> 1.0) của Canvas
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;

        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Áp dụng mỏ neo mới cho RectTransform
        _rectTransform.anchorMin = anchorMin;
        _rectTransform.anchorMax = anchorMax;
        _rectTransform.offsetMin = Vector2.zero;
        _rectTransform.offsetMax = Vector2.zero;

        Debug.Log($"[SafeArea] Đã bóp viền né tai thỏ! AnchorMin: {anchorMin}, AnchorMax: {anchorMax}");
    }
}


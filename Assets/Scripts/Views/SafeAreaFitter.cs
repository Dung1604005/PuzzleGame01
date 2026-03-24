using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class SafeAreaFitter : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Rect _lastSafeArea = new Rect(0, 0, 0, 0);

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        ApplySafeArea();
    }

    private void ApplySafeArea()
    {
        // Lấy thông số vùng an toàn từ thiết bị thực tế
        Rect safeArea = Screen.safeArea;
        

        // Lưu lại để so sánh cho frame sau
        _lastSafeArea = safeArea;

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

        Debug.Log($"[SafeArea] Đã bóp viền né tai thỏ! AnchorMin: {anchorMin}, AnchorMax: {anchorMax}");
    }
}


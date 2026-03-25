using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FloatingTextManager : MonoBehaviour
{
    [SerializeField] private Canvas targetCanvas;
    [SerializeField] private RectTransform floatingTextRoot;
    [SerializeField] private Camera worldCamera;

    private Queue<OnAddFloatingText> queue = new Queue<OnAddFloatingText>();

    private RectTransform _canvasRect;
    private Camera _worldCamera;
    private Camera _uiCamera;

    private void Awake()
    {
        if (targetCanvas == null)
        {
            targetCanvas = GetComponentInParent<Canvas>();
        }

        if (targetCanvas != null)
        {
            _canvasRect = targetCanvas.GetComponent<RectTransform>();
            _uiCamera = targetCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : targetCanvas.worldCamera;
        }

        if (floatingTextRoot == null)
        {
            floatingTextRoot = transform as RectTransform;
        }

        _worldCamera = worldCamera != null ? worldCamera : Camera.main;
    }

    public void OnEnable()
    {
        EventBus.Instance.Subscribe<OnAddFloatingText>(AddFloatingTextToQueue);
        EventBus.Instance.Subscribe<OnPopFloatingText>(PopFloatingTextFromQueue);
    }

    public void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnAddFloatingText>(AddFloatingTextToQueue);
        EventBus.Instance.UnSubscribe<OnPopFloatingText>(PopFloatingTextFromQueue);
    }

    public void ActiveFloatingText(OnAddFloatingText onAddFloatingText){
        FloatingText floatingText = ObjectPoolManager.Instance
            .Spawn(ObjectPoolManager.Instance.poolConfigs[1].Prefab, Vector3.zero, Quaternion.identity, floatingTextRoot)
            .GetComponent<FloatingText>();

        Vector2 anchoredPosition = ConvertPositionToCanvasLocal(onAddFloatingText.Position, onAddFloatingText.PositionType);
        floatingText.SetAnchoredPosition(anchoredPosition);

        floatingText.SetText(onAddFloatingText.Content, onAddFloatingText.FontSize, onAddFloatingText.TextColor);
        floatingText.StartFloating();
    }
    public void AddFloatingTextToQueue(OnAddFloatingText onAddFloatingText)
    {
        

        if(queue.Count == 0)
        {
            queue.Enqueue(onAddFloatingText);
            ActiveFloatingText(queue.Peek());
        }
        else
        {
            queue.Enqueue(onAddFloatingText);
        }
    }

    private Vector2 ConvertPositionToCanvasLocal(Vector2 position, FloatingTextPositionType positionType)
    {
        if (_canvasRect == null)
        {
            return position;
        }

        if (_worldCamera == null)
        {
            _worldCamera = Camera.main;
        }

        Vector2 screenPoint;
        if (positionType == FloatingTextPositionType.ScreenCenter)
        {
            // Dùng screen center
            screenPoint = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }
        else
        {
            // Convert world position sang screen point
            screenPoint = RectTransformUtility.WorldToScreenPoint(_worldCamera, position);
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvasRect, screenPoint, _uiCamera, out Vector2 localPoint))
        {
            return localPoint;
        }

        return Vector2.zero;
    }

    public void PopFloatingTextFromQueue(OnPopFloatingText onPopFloatingText)
    {
        if(queue.Count > 0)
        {
            queue.Dequeue();
            if(queue.Count > 0){
                ActiveFloatingText(queue.Peek());
            }
        }
        
    }


}



public enum FloatingTextPositionType
{
    WorldPosition,    // Tọa độ world, convert sang canvas
    ScreenCenter      // Hiển thị ở giữa màn hình
}

public struct OnAddFloatingText: IEvent
{
    public string Content;
    public int FontSize;
    public Vector2 Position;
    public Color TextColor;
    public FloatingTextPositionType PositionType;

    public OnAddFloatingText(string content, int fontSize, Vector2 position, Color textColor, FloatingTextPositionType positionType = FloatingTextPositionType.WorldPosition)
    {
        Content = content;
        FontSize = fontSize;
        Position = position;
        TextColor = textColor;
        PositionType = positionType;
    }
}

public struct OnPopFloatingText:IEvent
{
    
}
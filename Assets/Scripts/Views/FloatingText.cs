using DG.Tweening;
using TMPro;
using UnityEngine;



public class FloatingText : MonoBehaviour
{
    private TextMeshProUGUI text;
    private RectTransform _rectTransform;

    [SerializeField] private float lerpTime;

    void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        _rectTransform = GetComponent<RectTransform>();
    }

    public void SetText(string content, int sizeFont, Color textColor)
    {
        text.text = content;

        text.fontSize = sizeFont;

        text.color = textColor;
    }

    public void SetAnchoredPosition(Vector2 anchoredPosition)
    {
        if (_rectTransform == null)
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        _rectTransform.anchoredPosition = anchoredPosition;
    }


    public void StartFloating()
    {
        this.transform.localScale = Vector3.zero;

        DOTween.Sequence().Append(transform.DOScale(1, lerpTime).SetEase(Ease.OutBack))
        .AppendInterval(1f)
        .OnComplete(() =>
        {
            EventBus.Instance.Publish(new OnPopFloatingText
            {
                
            });
            ObjectPoolManager.Instance.Despawn(this.gameObject);
        });
    }
}

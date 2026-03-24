using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ComboGlow : MonoBehaviour
{
    private Image _glowImage;

    [Header("Cấu hình Combo Thấp (x2 - x4)")]
    
    public float lowPulseDuration = 0.6f;                     // Nhịp đập chậm
    public float lowPulseScale = 1.2f;                        // Phóng to ít

    [Header("Cấu hình Combo Cao (x5+)")]
    
    public float highPulseDuration = 0.3f;                       // Nhịp đập dồn dập
    public float highPulseScale = 1.4f;                          // Phóng to nhiều

    private Tween _scaleTween;
    private Tween _colorTween;

    private GlowState _currentState = GlowState.None;

    private void Awake()
    {
        _glowImage = GetComponent<Image>();

        _glowImage.color = new Color(1, 1, 1, 0);
    }

    public void OnEnable()
    {
        EventBus.Instance.Subscribe<OnScoreUpdated>(UpdateGlowState);
    }
    public void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnScoreUpdated>(UpdateGlowState);
    }

    public void UpdateGlowState(OnScoreUpdated onScoreUpdated)
    {
        int combo = onScoreUpdated.CurrentCombo;
        GlowState targetState = GlowState.None;

        if (combo >= 5) targetState = GlowState.High;
        else if (combo >= 2) targetState = GlowState.Low;

        if (targetState == _currentState)
        {
            return;
        }

        _currentState = targetState;
        _scaleTween?.Kill();
        _colorTween?.Kill();



        if (targetState == GlowState.None)
        {
            _colorTween = _glowImage.DOFade(0, 0.3f);
            _scaleTween = transform.DOScale(Vector3.one, 0.3f);
            return;
        }

        transform.localScale = Vector3.one;
        

        Color targetColor = onScoreUpdated.CurrentCombo >= 5 ? GameManager.Instance.ThemeData.comboHighEffectColor : GameManager.Instance.ThemeData.comboLowEffectColor;

        float pulseDuration = onScoreUpdated.CurrentCombo >= 5 ? highPulseDuration : lowPulseDuration;

        float targetScale = onScoreUpdated.CurrentCombo >= 5 ? highPulseScale : lowPulseScale;

        _glowImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, targetColor.a * 0.5f);



        _glowImage.color = new Color(targetColor.r, targetColor.g, targetColor.b, targetColor.a * 0.5f);

        _scaleTween = transform.DOScale(targetScale, pulseDuration).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);

        _colorTween = _glowImage.DOFade(targetColor.a, pulseDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);
    }
}
public enum GlowState { None, Low, High }
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ScoreUI : MonoBehaviour
{
    [SerializeField] private  TextMeshProUGUI highScore;

    [SerializeField] private TextMeshProUGUI currentScore;

    [SerializeField] private float speedUpdatingScore;

    private int _currentDisplayScore = 0;
    private int _targetScore = 0;

    private int _currentDisplayHighScore = 0;

    private int _targetHighScore = 0;

    public void OnEnable()
    {
        EventBus.Instance.Subscribe<OnScoreUpdated>(UpdateScore);
    }
    public void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnScoreUpdated>(UpdateScore);
    }

    public void UpdateScore(OnScoreUpdated onScoreUpdated)
    {
        if(_currentDisplayScore == onScoreUpdated.CurrentScore)
        {
            return;
        }
        _targetScore = onScoreUpdated.CurrentScore;
        _targetHighScore = onScoreUpdated.HighScore;

        DOTween.Kill("ScoreAnimation");
        DOTween.Kill("HighScoreAnimation");
        currentScore.transform.DOScale(1.1f, speedUpdatingScore/2);

        DOTween.To(() => _currentDisplayScore,
        x =>
        {
            _currentDisplayScore = x;
            currentScore.text = _currentDisplayScore.ToString();
        },
        _targetScore, speedUpdatingScore).SetId("ScoreAnimation").SetEase(Ease.OutCubic).OnComplete(() =>
        {
            currentScore.transform.DOScale(1f, speedUpdatingScore/2);
        });
        DOTween.To(() => _currentDisplayHighScore,
        x =>
        {
            _currentDisplayHighScore = x;
            highScore.text = _currentDisplayHighScore.ToString();
        },
        _targetHighScore, speedUpdatingScore).SetId("HighScoreAnimation").SetEase(Ease.OutCubic).OnComplete(() =>
        {
            highScore.transform.DOScale(1f, speedUpdatingScore/2);
        });
    }
}

using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject background;

    [SerializeField] private GameObject baseUI;


    [SerializeField] private TextMeshProUGUI scoreText;

    [SerializeField] private float speedUpdatingScore;

    public void OnEnable()
    {
        EventBus.Instance.Subscribe<OnGameOver>(OnGameOver);
    }
    public void OnDisable()
    {
        EventBus.Instance.UnSubscribe<OnGameOver>(OnGameOver);
    }

    public void OnGameOver(OnGameOver onGameOver)
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }
        baseUI.SetActive(false);
        background.SetActive(true);
        int currentDisplayScore=  0;
        int targetScore = onGameOver.CurrentScore;

        DOTween.Kill("ScoreAnimation");


        DOTween.To(()=> currentDisplayScore,
        x => {
            currentDisplayScore = x;
            scoreText.text = currentDisplayScore.ToString();

        }, targetScore, speedUpdatingScore).SetId("ScoreAnimation").SetEase(Ease.OutCubic);
    }

    public void NewGame()
    {
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }
        baseUI.SetActive(true);
        background.SetActive(false);
        GameManager.Instance.RestartGame();
    }
}

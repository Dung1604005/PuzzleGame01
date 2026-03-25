using System;
using UnityEngine;


public class ScoreModel
{
    private int currentScore;

    public int CurrentScore => currentScore;

    private int highScore;

    public int HighScore => highScore;

     private int comboCount;

    public int ComboCount => comboCount;

    // Tick combo kiem tra xem da bao nhieu lan combo chua duoc kich hoat
    private int tickCombo = 0;

    

    public void AddScore(int lines)
    {
        int scoreToAdd = 0;

        // Diem thuong neu co combo
        if(lines > 0)
        {
            comboCount += lines;

            int lineBonus = 0;

            switch (lines)
            {
                case 1: lineBonus = 100; break;
                case 2: lineBonus = 300; break;
                case 3: lineBonus = 600; break;
                case 4: lineBonus = 1000; break;
                default: lineBonus = 1000 + (lines - 4)*100; break;
                // Neu lines >= 5 thi moi line +100
            }

            int comboBonus = (comboCount > 1) ? (comboCount-1)*67:0;
            // Mot combo duoc +67 (tinh tu combo 2)
            scoreToAdd += lineBonus + comboBonus;
            tickCombo = 0;
        }
        else
        {
            tickCombo+= 1;
            if(tickCombo >= 3)
            {
                ResetCombo();
            }
        }
        currentScore += scoreToAdd;
        highScore = Math.Max(currentScore, highScore);
        EventBus.Instance.Publish(new OnAddScore
        {
            AddedScore = scoreToAdd,
            CurrentCombo = comboCount     
        });
        EventBus.Instance.Publish(new OnScoreUpdated
        {
            CurrentScore = currentScore,
            HighScore = highScore,
            CurrentCombo = comboCount            
        });
    }

    public void Reset()
    {
        ResetCombo();
        currentScore = 0;
        EventBus.Instance.Publish(new OnScoreUpdated
        {
            CurrentScore = currentScore,
            HighScore = highScore
        });
    }

    public void ResetCombo()
    {
        comboCount = 0;
    }
    public void LoadHighScore(int saved)
    {
        highScore = saved;
    }

    public ScoreModel()
    {
        EventBus.Instance.Publish(new OnScoreUpdated
        {
            CurrentScore = currentScore,
            HighScore = highScore
        });
    }

}

public struct OnAddScore: IEvent
{
    public int AddedScore;

    public int CurrentCombo;
}
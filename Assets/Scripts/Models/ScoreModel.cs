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

    // Event thay doi current score va high score
    public event Action<int, int> OnScoreUpdated; 

    public void AddScore(int lines, int tilesCleared)
    {
        int scoreToAdd = 0;
        // Tinh 10 diem voi moi tile bi xoa

        scoreToAdd += tilesCleared*10;

        // Diem thuong neu co xoa hang hoac xoa cot 
        if(lines > 0)
        {
            comboCount ++;

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
        }
        else
        {
            ResetCombo();
        }
        currentScore += scoreToAdd;
        highScore = Math.Max(currentScore, highScore);
        OnScoreUpdated?.Invoke(currentScore,  highScore);
    }

    public void ResetCombo()
    {
        comboCount = 0;
    }
    public void LoadHighScore(int saved)
    {
        highScore = saved;
    }

}

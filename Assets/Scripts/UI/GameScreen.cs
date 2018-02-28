using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class GameScreen : MonoBehaviour
{
    #region Fields

    public static event Action<bool> OnScreenTouch;

    [SerializeField] int scoreTextSize = 30;

    Text scoreText;

    int playerScore;

    bool isGameOver;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        BoardManager.PlayerScore += OnPlayerScore;
        BoardManager.HeroFell += GameOver;
    }


    void Start()
    {
        ScoreTextSettings();
    }


    void Update()
    {
        SetScore();
    }


    void OnMouseDown()
    {
        OnScreenTouch(true);
    }


    void OnMouseUp()
    {
        OnScreenTouch(false);
    }


    void OnDisable()
    {
        BoardManager.PlayerScore -= OnPlayerScore;
        BoardManager.HeroFell -= GameOver;
    }

    #endregion


    #region Private methods

    void ScoreTextSettings()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();

        scoreText.fontSize = scoreTextSize;
        scoreText.color = Color.white;

        scoreText.text = "0";
    }

    void SetScore()
    {
        scoreText.text = " " + playerScore;
    }

    #endregion


    #region Event handlers

    void OnPlayerScore(int getPlayerScore)
    {
        playerScore = getPlayerScore;
    }


    void GameOver(bool gameOver)
    {
        isGameOver = gameOver;

        if (isGameOver)
        {
            playerScore = 0;
        }
    }

    #endregion
}

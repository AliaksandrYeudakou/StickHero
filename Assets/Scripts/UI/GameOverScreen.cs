using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameOverScreen : MonoBehaviour
{
    #region Fields

    [SerializeField] private Button restartButton;
    [SerializeField] private GameObject scoreTitle;
    [SerializeField] private GameObject bestTitle;
    [SerializeField] private GameObject scoreText;
    [SerializeField] private GameObject bestScoreText;
    [SerializeField] int gameOverTextSize = 10;
    [SerializeField] int scoreTitleTextSize = 15;
    [SerializeField] int bestTitleTextSize = 15;

    Text gameOverText;
    Text scoreTitleText;
    Text bestTitleText;
    Text score;
    Text bestScore;

    int playerScore;
    int bestPlayerScore;

    bool isGameRestarted;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        BoardManager.PlayerScore += OnPlayerScore;
        GameManager.GameIsRestarted += GameRestarted;
    }


    void Start()
    {
        GameOverTextSettings();

        Button restartBtn = restartButton.GetComponent<Button>();
        restartBtn.onClick.AddListener(GameManager.instance.RestartGame);
    }


    void Update()
    {
        SetResults();

        if (playerScore > bestPlayerScore)
        {
            bestPlayerScore = playerScore;
        }
    }

    #endregion


    #region Private methods

    void GameOverTextSettings()
    {
        gameOverText = GameObject.Find("GameOverTitle").GetComponent<Text>();
        scoreTitleText = GameObject.Find("ScoreTitle").GetComponent<Text>();
        bestTitleText = GameObject.Find("BestTitle").GetComponent<Text>();
        score = GameObject.Find("ScoreText").GetComponent<Text>();
        bestScore = GameObject.Find("BestScoreText").GetComponent<Text>();

        gameOverText.fontSize = gameOverTextSize;
        scoreTitleText.fontSize = scoreTitleTextSize;
        bestTitleText.fontSize = bestTitleTextSize;

        gameOverText.color = Color.white;
        scoreTitleText.color = Color.black;
        bestTitleText.color = Color.black;
        score.color = Color.black;
        bestScore.color = Color.black;

        gameOverText.text = "Game over!".ToUpper();
        scoreTitleText.text = "Score".ToUpper();
        bestTitleText.text = "Best".ToUpper();
    }


    void SetResults()
    {
        score.text = " " + playerScore;
        bestScore.text = " " + bestPlayerScore;
    }

    #endregion


    #region Event handlers

    void OnPlayerScore(int getPlayerScore)
    {
        playerScore = getPlayerScore;
    }

    void GameRestarted(bool gameRestarder)
    {
        isGameRestarted = gameRestarder;

        if (isGameRestarted)
        {
            playerScore = 0;
        }
    }

    #endregion
}

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
    [SerializeField] int gameOverTextSize = 10;
    [SerializeField] int scoreTitleTextSize = 15;
    [SerializeField] int bestTitleTextSize = 15;

    Text gameOverText;
    Text scoreTitleText;
    Text bestTitleText;

    #endregion


    #region Unity lifecycle

    void Start()
    {
        GameOverTextSettings();

        Button restartBtn = restartButton.GetComponent<Button>();
        restartBtn.onClick.AddListener(GameManager.instance.RestartGame);
    }

    #endregion


    #region Private methods

    void GameOverTextSettings()
    {
        gameOverText = GameObject.Find("GameOverTitle").GetComponent<Text>();
        scoreTitleText = GameObject.Find("ScoreTitle").GetComponent<Text>();
        bestTitleText = GameObject.Find("BestTitle").GetComponent<Text>();

        gameOverText.fontSize = gameOverTextSize;
        scoreTitleText.fontSize = scoreTitleTextSize;
        bestTitleText.fontSize = bestTitleTextSize;

        gameOverText.color = Color.white;
        scoreTitleText.color = Color.black;
        bestTitleText.color = Color.black;

        gameOverText.text = "Game over!".ToUpper();
        scoreTitleText.text = "Score".ToUpper();
        bestTitleText.text = "Best".ToUpper();
    }

    #endregion
}

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
    int playerScore = 0;

    #endregion


    #region Unity lifecycle

    void Start()
    {
        ScoreTextSettings();
    }


    void OnMouseDown()
    {
        OnScreenTouch(true);
    }


    void OnMouseUp()
    {
        OnScreenTouch(false);
    }

    #endregion


    #region Private methods

    void ScoreTextSettings()
    {
        scoreText = GameObject.Find("ScoreText").GetComponent<Text>();

        scoreText.fontSize = scoreTextSize;
        scoreText.color = Color.white;

        scoreText.text = " " + playerScore;
    }

    #endregion
}

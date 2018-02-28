using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class UIManager : MonoBehaviour
{
    #region Fields

    public static UIManager instance = null;

    [SerializeField] GameObject gameScreen;
    [SerializeField] GameObject mainScreen;
    [SerializeField] GameObject gameOverScreen;
    [SerializeField] GameObject stickController;

    bool isGameOver;

    #endregion


    #region Unity Lificycle

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        else if (instance != null)
        {
            Destroy(gameObject);
        }

        InitialUIState();
    }


    void OnEnable()
    {
        BoardManager.HeroFell += GameOver;
    }


    void OnDisable()
    {
        BoardManager.HeroFell -= GameOver;
    }

    #endregion


    #region Public methods

    public void GameScreenState()
    {
        mainScreen.gameObject.SetActive(false);
        gameScreen.gameObject.SetActive(true);
    }


    public void GameScreenRestartState()
    {
        stickController.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(false);
        gameScreen.gameObject.SetActive(true);
    }

    #endregion


    #region Private methods

    private void HideScreens()
    {
        gameScreen.gameObject.SetActive(false);
        gameOverScreen.gameObject.SetActive(false);
    }


    private void InitialUIState()
    {
        mainScreen.gameObject.SetActive(true);
        gameOverScreen.gameObject.SetActive(true);
        HideScreens();
    }

    #endregion


    #region Event handlers

    void GameOver(bool gameOver)
    {
        isGameOver = gameOver;

        if (isGameOver)
        {
            gameScreen.gameObject.SetActive(false);
            gameOverScreen.gameObject.SetActive(true);
        }
    }

    #endregion
}

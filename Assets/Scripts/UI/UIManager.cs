using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    #region Fields

    public static UIManager instance = null;

    [SerializeField] GameObject gameScreen;
    [SerializeField] GameObject mainScreen;

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

    #endregion


    #region Public methods

    public void GameScreenState()
    {
        mainScreen.gameObject.SetActive(false);
        gameScreen.gameObject.SetActive(true);
    }

    #endregion


    #region Private methods

    private void HideGameScreen()
    {
        gameScreen.gameObject.SetActive(false);
    }


    private void InitialUIState()
    {
        HideGameScreen();
    }

    #endregion

}

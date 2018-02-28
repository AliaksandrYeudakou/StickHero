using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class GameManager : MonoBehaviour
{
    #region Fields

    public static event Action<bool> GameIsRestarted;

    public static GameManager instance = null;

    [SerializeField] GameObject stickController;
    [SerializeField] AudioClip soundButton;

    BoardManager boardScript;
    StickController stickScript;

    #endregion


    #region Unity lifecycle

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

        DontDestroyOnLoad(gameObject);

        boardScript = GetComponent<BoardManager>();
        stickScript = GetComponent<StickController>();

        CameraSetting();

        InitialGameState();
    }

    #endregion


    #region Public methods

    public void StartGame()
    {
        SoundManager.instance.PlaySingle(soundButton);

        UIManager.instance.GameScreenState();

        boardScript.ISGameStarted();

        stickController.gameObject.SetActive(true);
        stickController.GetComponent<StickController>().CanGrown = true;
    }


    public void RestartGame()
    {
        SoundManager.instance.PlaySingle(soundButton);

        UIManager.instance.GameScreenRestartState();

        boardScript.SetupGameScene();

        stickController.gameObject.SetActive(true);
        stickController.GetComponent<StickController>().CanGrown = true;
        GameIsRestarted(true);
    }

    #endregion


    #region Private methods

    void InitialGameState()
    {
        stickController.gameObject.SetActive(false);
    }


    void CameraSetting()
    {
        float orthographicSize = Camera.main.pixelHeight / 2;

        if (orthographicSize != Camera.main.orthographicSize)
        {
            Camera.main.orthographicSize = orthographicSize;
        }
    }

    #endregion
}

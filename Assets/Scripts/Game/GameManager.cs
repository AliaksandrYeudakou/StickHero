using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    #region Fields

    public static GameManager instance = null;

    [SerializeField] GameObject stickController;

    BoardManager boardScript;

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

        CameraSetting();

        InitialGameState();
    }

    #endregion


    #region Public methods

    public void StartGame()
    {
        UIManager.instance.GameScreenState();

        boardScript.ISGameStarted();

        stickController.gameObject.SetActive(true);
        stickController.GetComponent<StickController>().CanGrown = true;
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

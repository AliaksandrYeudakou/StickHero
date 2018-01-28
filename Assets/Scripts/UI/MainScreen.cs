using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainScreen : MonoBehaviour
{
    #region Fields

    [SerializeField] private Button playButton;

    #endregion


    #region Unity lifecycle

    void Start()
    {
        Button playBtn = playButton.GetComponent<Button>();
        playBtn.onClick.AddListener(GameManager.instance.StartGame);
    }

    #endregion
}

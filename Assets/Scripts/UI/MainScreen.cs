using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class MainScreen : MonoBehaviour
{
    #region Fields

    [SerializeField] private Button playButton;
    [SerializeField] private Button muteButton;
    [SerializeField] private Button volumeButton;

    #endregion


    #region Unity lifecycle

    void Start()
    {
        Button playBtn = playButton.GetComponent<Button>();
        playBtn.onClick.AddListener(GameManager.instance.StartGame);

        Button muteBtn = muteButton.GetComponent<Button>();
        muteBtn.onClick.AddListener(SoundManager.instance.MuteSound);

        Button volumeBtn = volumeButton.GetComponent<Button>();
        volumeBtn.onClick.AddListener(SoundManager.instance.GetSound);
    }

    #endregion
}

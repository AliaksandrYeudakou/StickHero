using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    #region Fields

    public static SoundManager instance = null;

    [SerializeField] AudioSource efxSource;                 
    [SerializeField] AudioSource backgroundMusic;                 

    #endregion


    #region Unity lifecycle

    void Awake()
    {
        
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        backgroundMusic = GetComponent<AudioSource>();

        DontDestroyOnLoad(gameObject);
    }

    #endregion


    #region Public methods

    public void PlaySingle(AudioClip clip)
    {
        efxSource.clip = clip;

        efxSource.Play();
    }


    public void MuteSound()
    {
        backgroundMusic.mute = true;
        efxSource.mute = true;
    }


    public void GetSound()
    {
        backgroundMusic.mute = false;
        efxSource.mute = false;
    }

    #endregion
}

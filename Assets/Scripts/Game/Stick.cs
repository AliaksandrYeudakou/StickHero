using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Stick : MonoBehaviour
{
    #region Fields

    public static event Action<bool> GrowthOver;
    public static event Action<Vector3> StickScale;

    [SerializeField] GameObject stick;

    GameObject instanceStick;

    bool canGrown;
    bool isClicked;

    #endregion


    #region Unity Lifecycle

    void OnEnable()
    {
        GameScreen.OnScreenTouch += OnGameScreenTouch;
    }


    void Start()
    {
        SetStartScale();
    }


    void Update()
    {
        if (isClicked && canGrown)
        {
            GrawthStick();
        }
    }


    void OnDisable()
    {
        GameScreen.OnScreenTouch -= OnGameScreenTouch;
    }

    #endregion


    #region Properties

    public bool CanGrown
    {
        get
        {
            return canGrown;
        }
        set
        {
            canGrown = value;
        }
    }

    #endregion


    #region Private methods

    void SetStartScale()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }


    void GrawthStick()
    {
        float x = transform.position.x;
        float y = transform.position.y;

        float scaleY = transform.localScale.y;
        transform.localScale = new Vector3(1, scaleY + 1, 1);

        transform.position = new Vector3(x, y + 2, 1);
    }

    #endregion


    #region Event handlers

    void OnGameScreenTouch(bool isTouch)
    {
        isClicked = isTouch;

        if (!isTouch)
        {
            canGrown = false;

            GrowthOver(true);
            StickScale(transform.localScale);
        }
    }

    #endregion
}

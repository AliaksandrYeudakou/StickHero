using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class Stick : MonoBehaviour
{
    #region Fields
   
    const float DISPLACEMENT_TIME = 0.2f;

    public static event Action<bool> GrowthOver;
    public static event Action<Vector3> StickScale;

    [SerializeField] GameObject stick;

    GameObject instanceStick;

    Transform rotateBoard;

    float displacementTime;
    float startX;
    float endX;
    float stickPositionY;

    bool canGrown;
    bool isClicked;
    bool isChangePosition;

    #endregion


    #region Unity Lifecycle

    void Awake()
    {
        stickPositionY = -1 * (Screen.height * 0.2f);
    }


    void OnEnable()
    {
        GameScreen.OnScreenTouch += OnGameScreenTouch;
        BoardManager.DisplacementStick += OnDisplacementStick;
    }


    void Start()
    {
        //test below
        Setup();
        // end test

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
        BoardManager.DisplacementStick -= OnDisplacementStick;
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

    // test below
    void Setup()
    {
        float x = -1 * (Screen.width * 0.25f);
        float y = -1 * (Screen.height * 0.2f);

        rotateBoard = new GameObject("rotateBoard").transform;

        instanceStick = Instantiate(stick, new Vector3(0, 0, 2), Quaternion.identity);
        instanceStick.transform.SetParent(rotateBoard);

        rotateBoard.transform.position = new Vector3(x, y, 1);
    }
    // end test


    void SetStartScale()
    {
        transform.localScale = new Vector3(0, 0, 0);
    }


    void GrawthStick()
    {
        /*float x = transform.position.x;
        float y = transform.position.y;

        float scaleY = transform.localScale.y;
        transform.localScale = new Vector3(1, scaleY + 1, 1);

        transform.position = new Vector3(x, y + 2, 1);*/

        float x = instanceStick.transform.position.x;
        float y = instanceStick.transform.position.y;
        float scaleY = instanceStick.transform.localScale.y;
        instanceStick.transform.localScale = new Vector3(1, scaleY + 1, 1);

        instanceStick.transform.position = new Vector3(x, y + 2, 1);
    }


    void DisplacementStick(float newPosition)
    {
        if (displacementTime <= DISPLACEMENT_TIME)
        {
            displacementTime += Time.deltaTime;

            float t = displacementTime / DISPLACEMENT_TIME;

            float displacementTrajectory = Mathf.Lerp(startX, newPosition, t);

            gameObject.transform.position = new Vector3(displacementTrajectory, stickPositionY, t);

            if (Mathf.Approximately(gameObject.transform.position.x, endX))
            {
                // палочке нужно расти
            }
        }
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

            startX = (-1 * (Screen.width * 0.25f)) + (transform.localScale.y * 2);
        }
    }


    void OnDisplacementStick(bool needChangePosition, float displacementDisctance)
    {
        isChangePosition = needChangePosition;

        endX = startX - displacementDisctance;

        if (isChangePosition)
        {
            DisplacementStick(endX);
        }
    }

    #endregion
}

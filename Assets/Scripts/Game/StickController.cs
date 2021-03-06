﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class StickController : MonoBehaviour
{
    #region Fields

    const float HEIGHT_BLOCK_RATIO = 0.3f;
    const float DISPLACEMENT_TIME = 0.2f;
    const float ROTATION_TIME = 250f;

    public static event Action<bool> BridgeBuilt;
    public static event Action<Vector3> StickScale;

    [SerializeField] GameObject stick;
    [SerializeField] AudioClip soundKick;
    [SerializeField] AudioClip soundKickStick;

    GameObject instanceStick;

    Transform rotateBoard;

    float displacementTime;
    float boardPositionX;
    float boardPositionY;

    float startX;
    float endX;

    bool isClicked;
    bool canGrown;
    bool canPutBridge;
    bool needFallBridge;
    bool isChangePosition;
    bool isGameOver;


    int stickCount;

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


    #region Unity lifecycle

    void OnEnable()
    {
        GameScreen.OnScreenTouch += OnGameScreenTouch;
        BoardManager.DisplacementStick += OnDisplacementStick;
        BoardManager.BridgeFall += OnBridgeFall;
        BoardManager.HeroFell += GameOver;
        GameManager.GameIsRestarted += RestartStickController;
    }


    void Start()
    {
        SetGameObjectPosition();
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
        BoardManager.BridgeFall -= OnBridgeFall;
        BoardManager.HeroFell -= GameOver;
        GameManager.GameIsRestarted -= RestartStickController;
    }

    #endregion


    #region Private methods

    public void SetGameObjectPosition()
    {
        if (stickCount == 1)
        {
            Destroy(rotateBoard.transform.gameObject, 3.5f);
            stickCount = 0;
        }

        float widthRatio = 0.25f;

        boardPositionX = -1 * (Screen.width * (0.5f - widthRatio));
        boardPositionY = -1 * (Screen.height * (0.5f - HEIGHT_BLOCK_RATIO));

        rotateBoard = new GameObject("rotateBoard").transform;

        instanceStick = Instantiate(stick, new Vector3(0, 0, 1), Quaternion.identity);
        instanceStick.transform.SetParent(rotateBoard);

        rotateBoard.transform.position = new Vector3(boardPositionX, boardPositionY, 1);

        stickCount++;
    }

    void GrawthStick()
    {
        float x = instanceStick.transform.position.x;
        float y = instanceStick.transform.position.y;
        float scaleY = instanceStick.transform.localScale.y;
        instanceStick.transform.localScale = new Vector3(1, scaleY + 1, 1);

        instanceStick.transform.position = new Vector3(x, y + 2, 1);
    }


    IEnumerator PutBridge(float euelerAngelZ)
    {
        SoundManager.instance.PlaySingle(soundKickStick);

        float time = 0;
        float step = 0;

        Quaternion from = rotateBoard.transform.rotation;
        Quaternion to = Quaternion.Euler(0, 0, euelerAngelZ);

        while (time < ROTATION_TIME)
        {
            time += 16;

            step -= (euelerAngelZ) / (ROTATION_TIME / 16f);

            rotateBoard.transform.rotation = Quaternion.RotateTowards(from, to, step);

            yield return new WaitForSeconds(0.016f);
        }

        if (to == Quaternion.Euler(0, 0, -90f))
        {
            BridgeBuilt(true);
            SoundManager.instance.PlaySingle(soundKick);
        }
    }


    void DisplacementStick(float newPosition)
    {
        if (displacementTime <= DISPLACEMENT_TIME)
        {
            displacementTime += Time.deltaTime;

            float t = displacementTime / DISPLACEMENT_TIME;

            float changePos = Mathf.Lerp(startX, newPosition, t);

            instanceStick.transform.position = new Vector3(changePos, boardPositionY, 1);

            if (Mathf.Approximately(instanceStick.transform.position.x, newPosition))
            {
                SetGameObjectPosition();
                canGrown = true;

                displacementTime = 0f;
            }
        }
    }

    #endregion


    #region Event handlers

    void OnGameScreenTouch(bool isTouch)
    {
        isClicked = isTouch;

        if (!isClicked)
        {
            canGrown = false;

            canPutBridge = true;

            if (canPutBridge)
            {
                StartCoroutine(PutBridge(-90f));
            }

            StickScale(instanceStick.transform.localScale);

            startX = boardPositionX + instanceStick.transform.localScale.y * 2;
        }
    }


    void OnBridgeFall(bool isNeedFallBridge)
    {
        needFallBridge = isNeedFallBridge;

        if (needFallBridge)
        {
            StartCoroutine(PutBridge(-180f));
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


    void GameOver(bool gameOver)
    {
        isGameOver = gameOver;

        if (isGameOver)
        {
            Destroy(rotateBoard.transform.gameObject, 1f);
            stickCount = 0;
        }
    }


    void RestartStickController(bool needrestart)
    {
        if (needrestart)
        {
            SetGameObjectPosition();   
        }
    }

    #endregion
}

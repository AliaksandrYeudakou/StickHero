using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class StickController : MonoBehaviour
{
    #region Fields

    const float HEIGHT_BLOCK_RATIO = 0.3f;

    public static event Action<bool> BridgeBuilt;

    float endTime = 250f;

    bool canPutBridge;
    bool needFallBridge;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        Stick.GrowthOver += OnGrowthOver;
        BoardManager.BridgeFall += OnBridgeFall;
    }


    void Start()
    {
        SetGameObjectPosition();
    }


    void OnDisable()
    {
        Stick.GrowthOver -= OnGrowthOver;
        BoardManager.BridgeFall -= OnBridgeFall;
    }

    #endregion


    #region Private methods

    void SetGameObjectPosition()
    {
        float widthRatio = 0.25f;

        float x = -1 * (Screen.width * (0.5f - widthRatio));
        float y = -1 * (Screen.height * (0.5f - HEIGHT_BLOCK_RATIO));

        transform.position = new Vector3(x, y, 1);
    }


    IEnumerator PutBridge(float euelerAngelZ)
    {
        float time = 0;
        float step = 0;

        Quaternion from = transform.rotation;
        Quaternion to = Quaternion.Euler(0, 0, euelerAngelZ);

        while (time < endTime)
        {
            time += 16;

            step -= (euelerAngelZ) / (endTime / 16f);

            transform.rotation = Quaternion.RotateTowards(from, to, step);

            yield return new WaitForSeconds(0.016f);
        }

        BridgeBuilt(true);

        canPutBridge = false;
        needFallBridge = false;
    }

    #endregion


    #region Event handlers

    void OnGrowthOver(bool isGrowthOver)
    {
        canPutBridge = isGrowthOver;

        if (canPutBridge)
        {
            StartCoroutine(PutBridge(-90f));
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

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class StickController : MonoBehaviour
{
    #region Fields

    const float HEIGHT_BLOCK_RATIO = 0.3f;

    public static event Action<bool, Vector3> BridgeBuilt;

    float endTime = 250f;

    bool canPutBridge;

    #endregion


    #region Unity lifecycle

    void OnEnable()
    {
        Stick.GrowthOver += OnGrowthOver;
    }


    void Start()
    {
        SetGameObjectPosition();
    }


    void OnDisable()
    {
        Stick.GrowthOver -= OnGrowthOver;
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


    IEnumerator PutBridge()
    {
        float time = 0;

        float startAngelX = transform.rotation.x;
        float startAngelY = transform.rotation.y;
        float startAngelZ = transform.rotation.z;

        Quaternion from = transform.rotation;
        Quaternion to = Quaternion.Euler(startAngelX, startAngelY, startAngelZ - 90f);

        float step = 0;

        while (time < endTime)
        {
            time += 16;

            step -= (startAngelZ - 90f) / (endTime / 16f);

            transform.rotation = Quaternion.RotateTowards(from, to, step);

            yield return new WaitForSeconds(0.016f);
        }

        BridgeBuilt(true, transform.position); 

        canPutBridge = false;
    }

    #endregion


    #region Event handlers

    void OnGrowthOver(bool isGrowthOver)
    {
        canPutBridge = isGrowthOver;

        if (canPutBridge)
        {
            StartCoroutine(PutBridge());
        }
    }

    #endregion
}

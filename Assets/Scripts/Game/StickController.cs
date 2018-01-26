using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class StickController : MonoBehaviour
{
    #region Fields

    const float HEIGHT_BLOCK_RATIO = 0.3f;

    public static event Action<bool> StickFell;

    [SerializeField] GameObject stick;

    GameObject instanceStick;

    bool isClicked;
    bool canGrown;

    bool isRotated;

    float positionX;
    float endTime = 250f;

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

    void Start()
    {
        SetStickPosition();
    }


    void OnEnable()
    {
        GameScreen.OnScreenTouch += OnGameScreenTouch;
    }


    void OnDisable()
    {
        GameScreen.OnScreenTouch -= OnGameScreenTouch;
    }


    void Update()
    {
        if (isClicked && canGrown)
        {
            GrawthStick();
        }
    }

    #endregion


    #region Private methods

    void SetStickPosition()
    {
        float widthRatio = 0.25f;

        float x = -1 * (Screen.width * (0.5f - widthRatio));
        float y = -1 * (Screen.height * (0.5f - HEIGHT_BLOCK_RATIO));

        instanceStick = Instantiate(stick, new Vector3(x, y, 1), Quaternion.identity);
        instanceStick.transform.localScale = new Vector3(0, 0, 0);
    }


    Vector2 GetIconSize(GameObject game_object)
    {
        return game_object.GetComponent<SpriteRenderer>().sprite.rect.size;
    }


    void GrawthStick()
    {
        float x = instanceStick.transform.position.x;
        float y = instanceStick.transform.position.y;

        float scaleY = instanceStick.transform.localScale.y;
        instanceStick.transform.localScale = new Vector3(1, scaleY + 1, 1);

        instanceStick.transform.position = new Vector3(x, y + 2, 1);

        positionX = instanceStick.transform.position.x + (instanceStick.transform.localScale.y * 2);
    }


    IEnumerator TurnStick()
    {
        float time = 0;

        float startX = instanceStick.transform.position.x;
        float endX = positionX;
        
        float startY = instanceStick.transform.position.y;
        float endY = -1 * (Screen.height * (0.5f - HEIGHT_BLOCK_RATIO));

        float startXStep = 1.25f;
        float coefficient = 0.5f / (endTime / 16f);

        float startYStep = 0.75f;

        float startAngelX = instanceStick.transform.rotation.x;
        float startAngelY = instanceStick.transform.rotation.y;
        float startAngelZ = instanceStick.transform.rotation.z;

        Quaternion from = instanceStick.transform.rotation;
        Quaternion to = Quaternion.Euler(startAngelX, startAngelY, startAngelZ - 90f);

        float step = 0;

        while (time < endTime)
        {
            time += 16;

            startXStep -= coefficient;
            startYStep += coefficient;

            float stepX = (endX - startX) / (endTime / 16f) * startXStep;
            float stepY = (endY - startY) / (endTime / 16f) * startYStep;

            step -= (startAngelZ - 90f) / (endTime / 16f);

            instanceStick.transform.rotation = Quaternion.RotateTowards(from, to, step);

            instanceStick.transform.position += new Vector3(stepX, stepY, 0);

            yield return new WaitForSeconds(0.016f);
        }

        instanceStick.transform.rotation = Quaternion.RotateTowards(from, to, -90f);
        instanceStick.transform.position = new Vector3(endX, endY, 1);

        StickFell(true);
    }

    #endregion


    #region Event handlers

    void OnGameScreenTouch(bool isTouch)
    {
        isClicked = isTouch;

        if (!isTouch)
        {
            StartCoroutine(TurnStick());
            canGrown = false;
        }
    }

    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class StickController : MonoBehaviour
{
    #region Fields

    [SerializeField] GameObject stick;

    GameObject instanceStick;

    bool isClicked;
    bool canGrown;

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
        float heightRatio = 0.3f;

        float iconWidth = GetIconSize(stick).y / 2;

        float x = -1 * (Screen.width * (0.5f - widthRatio));
        float y = -1 * (Screen.height * (0.5f - heightRatio));


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
    }

    #endregion


    #region Event handlers

    void OnGameScreenTouch(bool isTouch)
    {
        isClicked = isTouch;

        if (!isTouch)
        {
            canGrown = false;
        }
    }

    #endregion
}

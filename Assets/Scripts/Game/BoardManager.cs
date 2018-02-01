using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;


public class BoardManager : MonoBehaviour
{
    #region Fields

    const float START_BLOCK_WIDTH_RATIO = 0.25f;
    const float BLOCK_UI_HEIGHT_RATIO = 0.2f;
    const float BLOCK_GAME_HEIGHT_RATIO = 0.3f;
    const float HEIGHT_RATIO_HERO = 0.2f;
    const float WIDTH_RATIO_HERO = 0.3f;
    const float TIME_TO_MOVE = 0.2f;
    const float TIME_TO_RUNNING = 0.7f;
    const float TIME_TO_FALL = 0.5f;
    const float TIME_TO_PUSH = 0.6f;

    public static event Action<bool> BridgeFall;

    [SerializeField] GameObject startBlock; 
    [SerializeField] GameObject[] blocks;
    [SerializeField] GameObject hero;
    [SerializeField] GameObject stick;
    [SerializeField] int speed;

    GameObject instanceStartBlock;
    GameObject instanceBlock;
    GameObject instanceHero;

    Vector2 startBlockUISize;
    Vector2 startBlockGameSize;
    Vector2 heroIconSize;

    Vector3 stickScale;

    Transform boardHolder;

    float startCornerX;
    float centrePositionX;
    float startUIBlockPositionY;
    float startGameBlockPositionX;
    float startGameBlockPositionY;
    float startGameHeroPositionX;
    float heroGamePositionY;
    float startUIHeroPositionY;
    float startX_MovingHero;
    float endXBefor_movingHero;
    float endXAfter_movingHero;
    float endX_movingHeroSuccess;
    float stickEndX;
    float endY_fallingHero;
    float additionalBlock_leftCorner;
    float additionalBlock_rightCorner;

    float currentTime;
    float moovingTime;
    float fallingTime;
    float testTime;

    float heroHeight;
    float heroWidth;
    float blockHeight;

    float end;

    bool sartGame;
    bool isBridgeBuilt;

    bool mooving;
    bool needToFall;

    #endregion


    #region Unity lifecycle

    void Awake()
    {
        heroIconSize = GetIconSize(hero);

        startBlockUISize = SetBlockSize(START_BLOCK_WIDTH_RATIO, BLOCK_UI_HEIGHT_RATIO);
        startBlockGameSize = SetBlockSize(START_BLOCK_WIDTH_RATIO, BLOCK_GAME_HEIGHT_RATIO);

        heroWidth = HEIGHT_RATIO_HERO * startBlockUISize.y;
        heroHeight = WIDTH_RATIO_HERO * startBlockUISize.x;

        blockHeight = SetBlockHeight();

        startUIBlockPositionY = SetStartBlockPosition(startBlockUISize).y;

        centrePositionX = 0;
        startGameBlockPositionX = SetStartBlockPosition(startBlockGameSize).x;
        startGameBlockPositionY = SetStartBlockPosition(startBlockGameSize).y;
        
        startGameHeroPositionX = -1 * Screen.width / 2 + startBlockGameSize.x / 2 + heroWidth / 2;
        heroGamePositionY = -1 * (Screen.height / 2 - (startBlockGameSize.y + heroWidth / 2));

        startUIHeroPositionY = -1 * (Screen.height / 2 - (startBlockUISize.y + heroWidth / 2));

        endY_fallingHero = -1 * (Screen.height / 2 + heroHeight);

        startCornerX = -1 * (Screen.width * (0.5f - START_BLOCK_WIDTH_RATIO));

        startX_MovingHero = startGameHeroPositionX;
    }


    void OnEnable()
    {
        StickController.BridgeBuilt += OnBuiltBridge;
        Stick.StickScale += OnStickScale;
    }


    void Start()
    {
        SetupUIScene();

        CreateBlockAndPostions();
    }


    void Update()
    {
        if (sartGame && !isBridgeBuilt)
        {
            SetupStartGameBlockScale();
            SetupStartingGamePositions();

            PushBlock();
        }

        if (isBridgeBuilt)
        {
            MovingHero();
        }
    }


    void OnDisable()
    {
        StickController.BridgeBuilt -= OnBuiltBridge;
        Stick.StickScale -= OnStickScale;
    }

    #endregion


    #region Public methods

    public void ISGameStarted()
    {
        sartGame = true;
    }

    #endregion


    #region Private methods

    void SetupUIScene()
    {
        boardHolder = new GameObject("Board").transform;

        hero.transform.localScale = new Vector2(heroHeight / heroIconSize.x, heroWidth / heroIconSize.y);
        startBlock.transform.localScale = SetStartBlockScale(startBlockUISize, startBlock);

        instanceHero = Instantiate(hero, new Vector3(0, startUIHeroPositionY, 1), Quaternion.identity);
        instanceStartBlock = Instantiate(startBlock, new Vector3(0, startUIBlockPositionY, 1), Quaternion.identity);

        FixObjectBoxCollider2D(instanceHero);
        FixObjectBoxCollider2D(instanceStartBlock);

        instanceStartBlock.transform.SetParent(boardHolder);
        instanceHero.transform.SetParent(boardHolder);
    }



    void SetupStartGameBlockScale()
    {
        instanceStartBlock.transform.localScale = SetStartBlockScale(startBlockGameSize, startBlock);

        FixObjectBoxCollider2D(instanceStartBlock);
    }


    void SetupStartingGamePositions()
    {
        if (currentTime <= TIME_TO_MOVE)
        {
            currentTime += Time.deltaTime;

            float interpolated = currentTime / TIME_TO_MOVE;

            float changeBlockPosition = Mathf.Lerp(centrePositionX, startGameBlockPositionX, interpolated);
            float changeHeroPosition = Mathf.Lerp(centrePositionX, startGameHeroPositionX, interpolated);

            instanceStartBlock.transform.position = new Vector3(changeBlockPosition, startGameBlockPositionY, 1);

            instanceHero.transform.position = new Vector3(changeHeroPosition, heroGamePositionY, 1);
        }
    }


    void CreateBlockAndPostions()
    {
        GameObject block = blocks[Random.Range(0, blocks.Length)];

        float blockWidth = GetIconSize(block).x;

        float y = Screen.height / 2 - blockHeight / 2;
        float x = Screen.width / 2 + blockWidth / 2;

        instanceBlock = Instantiate(block, new Vector3(x, -y, 1), Quaternion.identity);
        instanceBlock.transform.SetParent(boardHolder);

        instanceBlock.transform.localScale = SetBlockScale(block);

        float start = instanceBlock.transform.position.x;
        float pickPosition = startCornerX + blockWidth;
        end = Random.Range(start - blockWidth, pickPosition);

        additionalBlock_leftCorner = end - GetIconSize(instanceBlock).x / 2;
        additionalBlock_rightCorner = end + GetIconSize(instanceBlock).x / 2;

        if (additionalBlock_rightCorner < 0)
        {
            endX_movingHeroSuccess = additionalBlock_rightCorner + heroWidth / 1.5f;
        }

        else if (additionalBlock_rightCorner >= 0)
        {
            endX_movingHeroSuccess = additionalBlock_rightCorner - heroWidth / 1.5f;
        }
    }


    void PushBlock()
    {
        if (testTime <= TIME_TO_PUSH)
        {
            testTime += Time.deltaTime;

            float interpolated = testTime / TIME_TO_PUSH;

            float changePosition = Mathf.Lerp(instanceBlock.transform.position.x, end, interpolated);

            instanceBlock.transform.position = new Vector3(changePosition, instanceBlock.transform.position.y, 1);
        }
    }
    

    void MovingHero()
    {
        if (!mooving)
        {
            if (stickEndX >= additionalBlock_leftCorner && stickEndX <= additionalBlock_rightCorner)
            {
                RunHeroHere(endX_movingHeroSuccess);

                if (instanceHero.transform.position.x == endX_movingHeroSuccess)
                {
                    mooving = true;
                }
            }

            else
            {
                needToFall = true;
            }

            if (needToFall)
            {
                if (stickEndX < additionalBlock_leftCorner)
                {
                    RunHeroHere(endXBefor_movingHero);

                    if (instanceHero.transform.position.x == endXBefor_movingHero)
                    {
                        BridgeFall(true);

                        FallHero(endXBefor_movingHero);
                    }
                }

                
                else if (stickEndX > additionalBlock_rightCorner)
                {
                    RunHeroHere(endXAfter_movingHero);

                    if (instanceHero.transform.position.x == endXAfter_movingHero)
                    {
                        BridgeFall(true);

                        FallHero(endXAfter_movingHero);
                    }
                }
                

                if (instanceHero.transform.position.y == endY_fallingHero)
                {
                    mooving = true;
                }
            }
        }    
    }


    void RunHeroHere(float endPosition)
    {
        if (moovingTime <= TIME_TO_RUNNING)
        {
            moovingTime += Time.deltaTime;

            float interpolated = moovingTime / TIME_TO_RUNNING;

            float changePos = Mathf.Lerp(startGameHeroPositionX, endPosition, interpolated);
            instanceHero.transform.position = new Vector3(changePos, heroGamePositionY, 1);
        }
    }


    void FallHero(float endX)
    {
        if (fallingTime <= TIME_TO_FALL)
        {
            fallingTime += Time.deltaTime;

            float t = fallingTime / TIME_TO_FALL;

            float fallTrajectory = Mathf.Lerp(heroGamePositionY, endY_fallingHero, t);
            instanceHero.transform.position = new Vector3(endX, fallTrajectory, 1);
        }
    }


    Vector2 GetIconSize(GameObject sprite)
    {
        return sprite.GetComponent<SpriteRenderer>().sprite.rect.size;
    }


    Vector2 SetBlockSize(float widthRatioBlock, float heightRatioBlock)
    {
        float startBlockWidth = Screen.width * widthRatioBlock;
        float startBlockHeight = Screen.height * heightRatioBlock;

        return new Vector2(startBlockWidth, startBlockHeight);
    }


    Vector3 SetStartBlockScale(Vector2 blockSize, GameObject block)
    {
        return new Vector3(blockSize.x / GetIconSize(block).x, blockSize.y / GetIconSize(block).y, 0);
    }


    Vector2 SetStartBlockPosition(Vector2 blockSize)
    {
        float xBlockInitPosition = -1 * (Screen.width / 2 - blockSize.x / 2);
        float yBlockInitPosition = -1 * (Screen.height / 2 - blockSize.y / 2);

        return new Vector2(xBlockInitPosition, yBlockInitPosition);
    }


    void FixObjectBoxCollider2D(GameObject instance)
    {
        Vector2 boxCollider2dSize = instance.GetComponent<SpriteRenderer>().sprite.bounds.size;

        instance.GetComponent<BoxCollider2D>().size = boxCollider2dSize;
    }


    Vector2 GetObjectPosition(GameObject instance)
    {
        return new Vector2(instance.transform.position.x, instance.transform.position.y);
    }


    float SetBlockHeight()
    {
        float blockHeight = Screen.height * BLOCK_GAME_HEIGHT_RATIO;

        return blockHeight;
    }
 

    Vector3 SetBlockScale(GameObject instance)
    {
        float blockIconSizeY = GetIconSize(instance).y;

        float blockScaleY = blockHeight / blockIconSizeY;

        return new Vector3(1, blockScaleY, 1);
    }

    #endregion


    #region Event handlers

    void OnBuiltBridge(bool isBridgeBuilt)
    {
        this.isBridgeBuilt = isBridgeBuilt;
    }


    void OnStickScale(Vector3 stickScale)
    {
        this.stickScale = stickScale;

        stickEndX = startCornerX + stickScale.y * 4;
        endXBefor_movingHero = stickEndX - heroWidth / 2;
        endXAfter_movingHero = stickEndX + heroWidth / 2;
    }

    #endregion
}

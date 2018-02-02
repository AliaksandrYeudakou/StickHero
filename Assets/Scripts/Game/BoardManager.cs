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
    const float DISPLACEMENT_TIME = 0.2f;

    public static event Action<bool> BridgeFall;
    public static event Action<bool, float> DisplacementStick;

    [SerializeField] GameObject startBlock; 
    [SerializeField] GameObject[] blocks;
    [SerializeField] GameObject hero;
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
    float startUIBlockPositionY;
    float startGameBlockPosX;
    float startGameBlockPosY;
    float startGameHeroPosX;
    float heroGamePositionY;
    float startUIHeroPositionY;
    float additionalBlockPosY;
    float startX_MovingHero;
    float endXBefor_movingHero;
    float endXAfter_movingHero;
    float endX_movingHeroSuccess;
    float stickEndX;
    float endY_fallingHero;
    float additionalBlock_leftCorner;
    float additionalBlock_rightCorner;
    float displacementDistance;
    float displacementBlockPosX;
    float displacementAddBlockPosX;

    float currentTime;
    float moovingTime;
    float fallingTime;
    float displacementTime;
    float testTime;

    float heroHeight;
    float heroWidth;
    float blockHeight;

    float end;

    bool sartGame;
    bool isBridgeBuilt;

    bool mooving;
    bool needToFall;

    bool needDisplacement;
    bool needPushBlock;

    #endregion


    #region Unity lifecycle

    void Awake()
    {
        heroIconSize = GetIconSize(hero);

        startCornerX = -1 * (Screen.width * (0.5f - START_BLOCK_WIDTH_RATIO));

        startBlockUISize = SetBlockSize(START_BLOCK_WIDTH_RATIO, BLOCK_UI_HEIGHT_RATIO);
        startBlockGameSize = SetBlockSize(START_BLOCK_WIDTH_RATIO, BLOCK_GAME_HEIGHT_RATIO);

        heroWidth = HEIGHT_RATIO_HERO * startBlockUISize.y;
        heroHeight = WIDTH_RATIO_HERO * startBlockUISize.x;

        blockHeight = SetBlockHeight();

        startUIBlockPositionY = SetStartBlockPosition(startBlockUISize).y;

        startGameBlockPosX = SetStartBlockPosition(startBlockGameSize).x;
        startGameBlockPosY = SetStartBlockPosition(startBlockGameSize).y;

        startGameHeroPosX = startCornerX - heroWidth / 1.5f;
        heroGamePositionY = -1 * (Screen.height / 2 - (startBlockGameSize.y + heroWidth / 2));

        startUIHeroPositionY = -1 * (Screen.height / 2 - (startBlockUISize.y + heroWidth / 2));

        endY_fallingHero = -1 * (Screen.height / 2 + heroHeight);

        startX_MovingHero = startGameHeroPosX;
    }


    void OnEnable()
    {
        StickController.BridgeBuilt += OnBuiltBridge;
        StickController.StickScale += OnStickScale;
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
            MovingObjects();
        }

        if (needPushBlock)
        {
            PushBlock();

            if (instanceBlock.transform.position.y == end)
            {
                needPushBlock = false;
            }
        }
    }


    void OnDisable()
    {
        StickController.BridgeBuilt -= OnBuiltBridge;
        StickController.StickScale -= OnStickScale;
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

            float changeBlockPosition = Mathf.Lerp(0, startGameBlockPosX, interpolated);
            float changeHeroPosition = Mathf.Lerp(0, startGameHeroPosX, interpolated);

            instanceStartBlock.transform.position = new Vector3(changeBlockPosition, startGameBlockPosY, 1);

            instanceHero.transform.position = new Vector3(changeHeroPosition, heroGamePositionY, 1);
        }
    }


    void CreateBlockAndPostions()
    {
        GameObject block = blocks[Random.Range(0, blocks.Length)];

        float blockWidth = GetIconSize(block).x;

        additionalBlockPosY = -1 * (Screen.height / 2 - blockHeight / 2);
        float x = Screen.width / 2 + blockWidth / 2;

        instanceBlock = Instantiate(block, new Vector3(x, additionalBlockPosY, 1), Quaternion.identity);
        instanceBlock.transform.SetParent(boardHolder);

        instanceBlock.transform.localScale = SetBlockScale(block);

        float start = instanceBlock.transform.position.x;
        float pickPosition = startCornerX + blockWidth;
        end = Random.Range(start - blockWidth, pickPosition);

        additionalBlock_leftCorner = end - GetIconSize(instanceBlock).x / 2;
        additionalBlock_rightCorner = end + GetIconSize(instanceBlock).x / 2;

        endX_movingHeroSuccess = additionalBlock_rightCorner - heroWidth / 1.5f;

        displacementDistance = endX_movingHeroSuccess - startGameHeroPosX;
        displacementBlockPosX = startGameBlockPosX - displacementDistance;
        displacementAddBlockPosX = end - displacementDistance;
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

        if (instanceBlock.transform.position.x == end)
        {
            testTime = 0f;
        }
    }
    

    void MovingObjects()
    {
        if (!mooving)
        {
            if (stickEndX >= additionalBlock_leftCorner && stickEndX <= additionalBlock_rightCorner)
            {
                RunHero(endX_movingHeroSuccess);

                if (instanceHero.transform.position.x == endX_movingHeroSuccess)
                {
                    needDisplacement = true;
                }
            }

            if (needDisplacement)
            {
                DisplacementObjects();

                DisplacementStick(true, displacementDistance); // at this moment

                if (instanceHero.transform.position.x == startGameHeroPosX)
                {
                    CreateBlockAndPostions();

                    mooving = true;
                    needPushBlock = true;
                    needDisplacement = false;
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
                    RunHero(endXBefor_movingHero);

                    if (instanceHero.transform.position.x == endXBefor_movingHero)
                    {
                        BridgeFall(true);

                        FallHero(endXBefor_movingHero);
                    }
                }

                
                else if (stickEndX > additionalBlock_rightCorner)
                {
                    RunHero(endXAfter_movingHero);

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


    void RunHero(float endPosition)
    {
        if (moovingTime <= TIME_TO_RUNNING)
        {
            moovingTime += Time.deltaTime;

            float interpolated = moovingTime / TIME_TO_RUNNING;

            float changePos = Mathf.Lerp(startGameHeroPosX, endPosition, interpolated);
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


    void DisplacementObjects()
    {
        if (displacementTime <= DISPLACEMENT_TIME)
        {
            displacementTime += Time.deltaTime;

            float t = displacementTime / DISPLACEMENT_TIME;

            float heroTrajectory = Mathf.Lerp(endX_movingHeroSuccess, startGameHeroPosX, t);
            float startBlockTrajectory = Mathf.Lerp(startGameBlockPosX, displacementBlockPosX, t);
            float additionalBlockTrajectory = Mathf.Lerp(end, displacementAddBlockPosX, t);

            instanceHero.transform.position = new Vector3(heroTrajectory, heroGamePositionY, 1);
            instanceStartBlock.transform.position = new Vector3(startBlockTrajectory, startGameBlockPosY, 1);
            instanceBlock.transform.position = new Vector3(additionalBlockTrajectory, additionalBlockPosY, 1);
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

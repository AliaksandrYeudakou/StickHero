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
    public static event Action<bool> HeroFell;
    public static event Action<int> PlayerScore;

    [SerializeField] GameObject startBlock; 
    [SerializeField] GameObject[] blocks;
    [SerializeField] GameObject hero;
    [SerializeField] int speed;
    [SerializeField] AudioClip soundScore;
    [SerializeField] AudioClip soundVictory;
    [SerializeField] AudioClip soundDeath;

    GameObject instanceStartBlock;
    GameObject instanceBlock;
    GameObject additionalBlock;
    GameObject instanceHero;

    GameObject instance;

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
    float additionalScoreLeft;
    float additionalScoreRight;

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
    bool startBlockRuined;
    bool needAdditionalPush;

    int offsetsCount;
    int score;

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

        additionalBlockPosY = -1 * (Screen.height / 2 - blockHeight / 2);
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
            if (!startBlockRuined)
            {
                SetupStartGameBlockScale();
            }
            
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

            if (instanceBlock.transform.position.x == end)
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


    public void SetupGameScene()
    {
        DestroyObject(additionalBlock);
        DestroyObject(instanceBlock);
        DestroyObject(instanceStartBlock);

        instanceHero.transform.position = new Vector3(startGameHeroPosX, heroGamePositionY, 1);

        instanceStartBlock = Instantiate(startBlock, new Vector3(startGameBlockPosX, startGameBlockPosY, 1), Quaternion.identity);
        instanceStartBlock.transform.localScale = SetStartBlockScale(startBlockGameSize, startBlock);

        instanceStartBlock.transform.SetParent(boardHolder);

        MyReset();
        CreateBlockAndPostions();
    }

    #endregion


    #region Private methods

    void MyReset()
    {
        isBridgeBuilt = false;

        mooving = true;
        needToFall = false;

        needDisplacement = false;
        needPushBlock = true;
        startBlockRuined = false;
        needAdditionalPush = false;
        sartGame = false;

        offsetsCount = 0;
        currentTime = 0;
        moovingTime = 0;
        fallingTime = 0;
        displacementTime = 0;
        testTime = 0;
        score = 0;
    }


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
        if (offsetsCount == 2)
        {
            offsetsCount = 0;
            DestroyObject(instanceBlock);
        }

        GameObject block = blocks[Random.Range(0, blocks.Length)];

        float blockWidth = GetIconSize(block).x;

        float x = Screen.width / 2 + blockWidth / 2;
        float pickPosition = startCornerX + blockWidth;

        end = Random.Range(x - blockWidth, pickPosition);
        additionalBlock_leftCorner = end - GetIconSize(block).x / 2;
        additionalBlock_rightCorner = end + GetIconSize(block).x / 2;
        endX_movingHeroSuccess = additionalBlock_rightCorner - heroWidth / 1.5f;

        additionalScoreLeft = end - 4;
        additionalScoreRight = end + 4;

        if (offsetsCount == 0)
        {
            instanceBlock = Instantiate(block, new Vector3(x, additionalBlockPosY, 1), Quaternion.identity);
            instanceBlock.transform.SetParent(boardHolder);

            instanceBlock.transform.localScale = SetBlockScale(block);
        }

        if (offsetsCount == 1)
        {
            additionalBlock = Instantiate(block, new Vector3(x, additionalBlockPosY, 1), Quaternion.identity);
            additionalBlock.transform.SetParent(boardHolder);

            additionalBlock.transform.localScale = SetBlockScale(block);
        }

        offsetsCount++;

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

            if (offsetsCount == 1)
            {
                float changePosition = Mathf.Lerp(instanceBlock.transform.position.x, end, interpolated);

                instanceBlock.transform.position = new Vector3(changePosition, instanceBlock.transform.position.y, 1);
            }

            if (offsetsCount == 2)
            {
                float changePosition = Mathf.Lerp(additionalBlock.transform.position.x, end, interpolated);

                additionalBlock.transform.position = new Vector3(changePosition, instanceBlock.transform.position.y, 1);
            }
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
                    if (stickEndX >= additionalScoreLeft && stickEndX <= additionalScoreRight)
                    {
                        score++;
                        SoundManager.instance.PlaySingle(soundVictory);
                    }

                    else
                    {
                        SoundManager.instance.PlaySingle(soundScore);
                    }

                    score++;
                    PlayerScore(score);
                    needDisplacement = true;
                }
            }

            if (needDisplacement)
            {
                DisplacementObjects();

                DisplacementStick(true, displacementDistance);

                if (instanceHero.transform.position.x == startGameHeroPosX)
                {
                    CreateBlockAndPostions();

                    moovingTime = 0f;
                    testTime = 0f;
                    displacementTime = 0f;

                    mooving = true;
                    isBridgeBuilt = false;  
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

                if (Mathf.Approximately(instanceHero.transform.position.y, endY_fallingHero))
                {
                    mooving = true;

                    isBridgeBuilt = false;
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

        if (Mathf.Approximately(instanceHero.transform.position.y, endY_fallingHero))
        {
            SoundManager.instance.PlaySingle(soundDeath);
            mooving = true;
            instanceHero.transform.position = new Vector3(endX, (-1 * (Screen.height)), 1);
            HeroFell(true);
        }
    }


    void DisplacementObjects()
    {
        if (displacementTime <= DISPLACEMENT_TIME)
        {
            displacementTime += Time.deltaTime;

            float t = displacementTime / DISPLACEMENT_TIME;
            
            float heroTrajectory = Mathf.Lerp(endX_movingHeroSuccess, startGameHeroPosX, t);

            instanceHero.transform.position = new Vector3(heroTrajectory, heroGamePositionY, 1);
               
            if (offsetsCount == 1)
            {  
                float additionalBlockTrajectory = Mathf.Lerp(end, displacementAddBlockPosX, t);
                float startBlockTrajectory = Mathf.Lerp(startGameBlockPosX, (-1 * Screen.width), t);

                instanceBlock.transform.position = new Vector3(additionalBlockTrajectory, additionalBlockPosY, 1);

                if (!startBlockRuined)
                {
                    instanceStartBlock.transform.position = new Vector3(startBlockTrajectory, startGameBlockPosY, 1);

                    if (instanceStartBlock.transform.position.x == (-1 * Screen.width))
                    {
                        startBlockRuined = true;
                        DestroyObject(instanceStartBlock);
                    }
                }
                
                if (needAdditionalPush)
                {
                    float additionalPushTrajectory = Mathf.Lerp(displacementAddBlockPosX, (-1 * Screen.width), t);
                    additionalBlock.transform.position = new Vector3(additionalPushTrajectory, additionalBlockPosY, 1);

                    if (additionalBlock.transform.position.x == (-1 * Screen.width))
                    {
                        needAdditionalPush = false;
                        DestroyObject(additionalBlock);
                    }
                }
            }

            if (offsetsCount == 2)
            {
                float additionalBlockTrajectory = Mathf.Lerp(end, displacementAddBlockPosX, t);
                additionalBlock.transform.position = new Vector3(additionalBlockTrajectory, additionalBlockPosY, 1);

                float newTrajectory = Mathf.Lerp(displacementAddBlockPosX, (-1 * Screen.width), t);
                instanceBlock.transform.position = new Vector3(newTrajectory, additionalBlockPosY, 1);

                if (additionalBlock.transform.position.x == displacementAddBlockPosX)
                {
                    needAdditionalPush = true;
                }
            }
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

        mooving = false;
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

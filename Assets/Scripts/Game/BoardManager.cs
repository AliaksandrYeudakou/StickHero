using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class BoardManager : MonoBehaviour
{
    #region Fields

    const float START_BLOCK_WIDTH_RATIO = 0.25f;
    const float BLOCK_UI_HEIGHT_RATIO = 0.2f;
    const float BLOCK_GAME_HEIGHT_RATIO = 0.3f;

    [SerializeField] GameObject startBlock; 
    [SerializeField] GameObject[] blocks;
    [SerializeField] GameObject hero;
    [SerializeField] int speed;

    GameObject instanceStartBlock;
    GameObject instanceBlock;
    GameObject instanceHero;

    Vector2 startBlockUISize;
    Vector2 startBlockGameSize;

    Transform boardHolder;

    float startBlockHeight;
    float startBlockWidth;
    float heroWidth;
    float end;

    bool sartGame;
    bool startGamePosition;

    #endregion


    #region Unity lifecycle

    void Start()
    {
        CreateBlockAndPostion();
    }


    void Update()
    {
        if (sartGame && !startGamePosition)
        {
            SetStartGamePosition();
            PushBlock(instanceBlock);
        }
    }

    #endregion


    #region Public methods

    public void SetStartUIPosition()
    {
        boardHolder = new GameObject("Board").transform;

        startBlockUISize = SetBlockSize(START_BLOCK_WIDTH_RATIO, BLOCK_UI_HEIGHT_RATIO);

        float y = SetStartBlockPosition(startBlockUISize).y;

        startBlock.transform.localScale = SetStartBlockScale(startBlockUISize, startBlock);
        instanceStartBlock = Instantiate(startBlock, new Vector3(0, y, 1), Quaternion.identity);

        FixObjectBoxCollider2D(instanceStartBlock);

        SetStartUIHeroPosition();

        instanceStartBlock.transform.SetParent(boardHolder);
        instanceHero.transform.SetParent(boardHolder);
    }


    public void ISGameStarted()
    {
        sartGame = true;
    }

    #endregion


    #region Private methods

    void SetStartGamePosition()
    {
        SetStartBlockGamePosition();
        SetStartGameHeroPosition();

        float currentX = instanceStartBlock.transform.position.x;
        float endX = SetStartBlockPosition(startBlockGameSize).x;

        if (currentX == endX)
        {
            startGamePosition = true;
        }
    }


    void SetStartBlockGamePosition()
    {
        startBlockGameSize = SetBlockSize(START_BLOCK_WIDTH_RATIO, BLOCK_GAME_HEIGHT_RATIO);
        instanceStartBlock.transform.localScale = SetStartBlockScale(startBlockGameSize, startBlock);

        FixObjectBoxCollider2D(instanceStartBlock);

        float start = instanceStartBlock.transform.position.x;
        float end = SetStartBlockPosition(startBlockGameSize).x;
        float interpolated = speed * Time.deltaTime;

        float changeBlockPosition = Mathf.Lerp(start, end, interpolated);

        float y = SetStartBlockPosition(startBlockGameSize).y;

        instanceStartBlock.transform.position = new Vector3(changeBlockPosition, y, 1);
    }


    void SetStartUIHeroPosition()
    {
        float widthRatioHero = 0.35f;
        float heightRatioHero = 0.25f;

        float heroHeight = widthRatioHero * startBlockUISize.x;
        heroWidth = heightRatioHero * startBlockUISize.y;

        Vector2 heroIconSize = GetIconSize(hero);

        hero.transform.localScale = new Vector2(heroHeight / heroIconSize.x, heroWidth / heroIconSize.y);

        float y = Screen.height / 2 - (startBlockHeight + heroWidth / 2);

        instanceHero = Instantiate(hero, new Vector3(0, -y, 1), Quaternion.identity);

        FixObjectBoxCollider2D(instanceHero);
    }


    void SetStartGameHeroPosition()
    {
        float interpolated = speed * Time.deltaTime;

        float y = Screen.height / 2 - (startBlockHeight + heroWidth / 2);

        float start = instanceHero.transform.position.x;
        float end = -1 * Screen.width / 2 + startBlockWidth / 2 + heroWidth / 2;

        instanceHero.transform.position = new Vector3(Mathf.Lerp(start, end, interpolated), -y, 1);
    }


    Vector2 GetIconSize(GameObject sprite)
    {
        return sprite.GetComponent<SpriteRenderer>().sprite.rect.size;
    }


    Vector2 SetBlockSize(float widthRatioBlock, float heightRatioBlock)
    {
        startBlockWidth = Screen.width * widthRatioBlock;
        startBlockHeight = Screen.height * heightRatioBlock;

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
        float blockHeight = SetBlockHeight();

        float blockIconSizeY = GetIconSize(instance).y;

        float blockScaleY = blockHeight / blockIconSizeY;

        return new Vector3(1, blockScaleY, 1);
    }


    void CreateBlockAndPostion()
    {
        GameObject block = blocks[Random.Range(0, blocks.Length)];

        float blockHeight = SetBlockHeight();
        float iconWidth = GetIconSize(block).x;

        float y = Screen.height / 2 - blockHeight / 2;
        float x = Screen.width / 2 + iconWidth / 2;

        instanceBlock = Instantiate(block, new Vector3(x, -y, 1), Quaternion.identity);
        instanceBlock.transform.SetParent(boardHolder);

        instanceBlock.transform.localScale = SetBlockScale(block);

        float corner = -1 * Screen.width / 2 + startBlockWidth;

        float start = instanceBlock.transform.position.x;
        float pickPosition = corner + iconWidth;
        end = Random.Range(start - iconWidth, pickPosition);
    }


    void PushBlock(GameObject instance)
    {
        float slowly = 10;
        float y = instanceBlock.transform.position.y;

        float start = instanceBlock.transform.position.x;

        float interpolated = slowly * Time.deltaTime;

        float changePosition = Mathf.Lerp(start, end, interpolated);
        
        instanceBlock.transform.position = new Vector3(changePosition, y, 1);   
    }

    #endregion
}

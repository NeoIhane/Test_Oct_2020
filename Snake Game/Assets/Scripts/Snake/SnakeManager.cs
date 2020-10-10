using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class SnakeManager : MonoBehaviour
{
    [SerializeField]
    Controller controller;

    public enum GameState { Play, EndGame }
    GameState gameState = GameState.Play;

    void Start()
    {

        //SetSnake
        InitSnake();

        //SetApple
        apple.InitApple();

        //Set UI
        TopBar.Instance.SetNumApple(totalApple);
        TopBar.Instance.SetNumThophy(maxApple);

        Popup.Instance.Init(OnPlayAgain);

        //Set Controller
        controller.onTouchDown = () => { if (direction != GetDirectUp()) direction = GetDirectDown(); };
        controller.onTouchRight = () => { if (direction != GetDirectLeft()) direction = GetDirectRight(); };
        controller.onTouchLeft = () => { if (direction != GetDirectRight() && direction != Vector3.zero) direction = GetDirectLeft(); };
        controller.onTouchUP = () => { if (direction != GetDirectDown()) direction = GetDirectUp(); };

    }
    void Update()
    {
        //map.DebugMap();
        controller.EnableControll = gameState == GameState.Play;
        switch (gameState)
        {
            case GameState.Play:
                SnakeMoving();
                break;
            case GameState.EndGame:

                break;
        }
    }
    void OnPlayAgain()
    {
        Popup.Instance.Hide();
        RestartSnake();
        apple.PlaceAppleAtStartPoint();
        gameState = GameState.Play;
    }

    #region Map
    [SerializeField]
    float blockSize = 1;
    [SerializeField]
    Map map = new Map(10, 10, 0, 0, 1);
    #endregion


    #region Score
    [SerializeField]
    Apple apple;
    int totalApple = 0;
    int maxApple = 0;
    void AddScore()
    {
        totalApple++;
        if (maxApple < totalApple) maxApple = totalApple;
        UpdateScore();
    }
    void ResetScore()
    {
        totalApple = 0;
        UpdateScore();
    }
    void UpdateScore()
    {
        TopBar.Instance.SetNumApple(totalApple);
        TopBar.Instance.SetNumThophy(maxApple);
    }
    IEnumerator RandomApple()
    {
        int randX = Random.Range(0, 10);
        int randY = Random.Range(0, 10);
        while (!CanPlaceApple(randX, randY))
        {
            randX = Random.Range(0, 10);
            randY = Random.Range(0, 10);
            yield return null;
        }
        apple.Place(new Vector3(randX, randY));
    }
    bool CanPlaceApple(int x, int y)
    {
        if (x == (int)snakeHead.currentPosition.x && y == (int)snakeHead.currentPosition.y) return false;
        if (x == (int)snakeTail.currentPosition.x && y == (int)snakeTail.currentPosition.y) return false;
        foreach (SnakeNode n in snakeNodes)
        {
            if (x == (int)n.currentPosition.x && y == (int)n.currentPosition.y) return false;

        }
        return true;
    }
    #endregion

    #region Snake
    #region SnakeParameter
    [Header("SnakeParameter")]
    [SerializeField]
    SnakeHeadNode snakeHead;
    [SerializeField]
    SnakeTailNode snakeTail;
    [SerializeField]
    SnakeBodyNode snakeBody;
    [SerializeField]
    Transform snakeBodyContains;
    [SerializeField]
    List<SnakeNode> snakeNodes = new List<SnakeNode>();
    bool isMoveNext = false;
    Vector3 direction;
    Vector3[] startPositon = new Vector3[3];
    bool isSnakeDie = false;
    #endregion
    #region SnakeManage
    public void InitSnake()
    {
        startPositon[0] = snakeHead.transform.position;//head
        startPositon[1] = snakeBody.transform.position;//body
        startPositon[2] = snakeTail.transform.position;//tail

        //Set Event
        snakeHead.onEatApple = OnEatApple;
        snakeHead.onEatSelf = OnEatSelf;
        snakeHead.onHitWall = OnHitWall;

        RestartSnake();

        snakeNodes.Add(snakeBody);
    }
    public void RestartSnake()
    {
        int nBody = snakeNodes.Count;
        for (int i = 1; i < nBody; i++)
        {
            Destroy(snakeNodes[snakeNodes.Count - 1].gameObject);
            snakeNodes.Remove(snakeNodes[snakeNodes.Count - 1]);
        }

        snakeHead.Set(startPositon[0], Vector3.zero, blockSize);
        snakeBody.Set(startPositon[1], (startPositon[0] - startPositon[1]).normalized, blockSize);
        snakeTail.Set(startPositon[2], (startPositon[1] - startPositon[2]).normalized, blockSize);

        snakeBody.tag = "Neck";

        direction = Vector3.zero;
        snakeHead.SetDirection(new Vector3(1, 0, 0), blockSize);
        snakeHead.SetToNormal();
        isSnakeDie = false;

        ResetScore();
        
    }
    public void AddSnakeBodyNode()
    {
        SnakeBodyNode node = (SnakeBodyNode)Instantiate<SnakeBodyNode>(snakeBody, snakeHead.transform.position, Quaternion.identity, snakeBodyContains);
        node.Set(snakeHead.currentPosition, snakeHead.direction, blockSize);
        node.name = "Boby_" + snakeNodes.Count.ToString();
        snakeNodes[snakeNodes.Count - 1].tag = "SnakeBody";
        snakeNodes.Add(node);
    }
    IEnumerator MoveTo(Vector3 direction)
    {
        float timecount = 0;
        float time = 0.3f;
        snakeHead.SetDirection(direction, blockSize);

        while (timecount < time)
        {
            timecount += Time.deltaTime;
            float value = timecount / time;
            snakeHead.MoveNext(value);
            foreach (SnakeNode node in snakeNodes)
                node.MoveNext(value);
            snakeTail.MoveNext(value);

            if (!isSnakeDie)
                yield return null;
        }
        
        snakeTail.Set(snakeNodes[0].currentPosition, snakeNodes[0].direction, blockSize);
        for (int i = 0; i < snakeNodes.Count; i++)
        {
            if (i == snakeNodes.Count - 1)
            {
                snakeNodes[i].SetToNextNode(snakeHead, blockSize);
            }
            else
            {
                snakeNodes[i].SetToNextNode(snakeNodes[i + 1], blockSize);
            }
        }
        snakeHead.Set(snakeHead.nextPosition, direction, blockSize);

        isMoveNext = false;
    }
    void SnakeMoving()
    {
        if (direction != Vector3.zero)
        {
            if (!isMoveNext)
            {
                StartCoroutine(MoveTo(direction));
                isMoveNext = true;
            }
            //CheckHitBound();
        }
    }
    #endregion
    #region SnakeEvents
    void OnEatSelf()
    {
        if (!isSnakeDie)
        {
            gameState = GameState.EndGame;
            snakeHead.SetToStun();
            StartCoroutine(PlayAnimStun(0.3f));
            isSnakeDie = true;
        }
    }
    void OnHitWall()
    {
        if (!isSnakeDie)
        {
            gameState = GameState.EndGame;
            snakeHead.SetToStun();
            StartCoroutine(PlayAnimStun(0.3f));
            isSnakeDie = true;
        }
    }
    void OnEatApple()
    {
        if (!isSnakeDie)
        {
            apple.Eat();
            AddSnakeBodyNode();

            AddScore();

            StartCoroutine(PlayAnimEatAppleToBody(0.3f));
            StartCoroutine(RandomApple());
        }
    }
    #endregion
    #region SnakeAnim
    IEnumerator PlayAnimEatAppleToBody(float time)
    {
        float timeCount = 0;
        Vector3 tmpScale = new Vector3(1, 1, 1);
        snakeHead.SetToEating();
        yield return new WaitForSeconds(0.3f);

        while (timeCount < time)
        {
            timeCount += Time.deltaTime;
            float value = timeCount / time;

            for (int i = 0; i < snakeNodes.Count; i++)
            {
                float addScale = 1f;
                float bias = (float)i / (float)snakeNodes.Count;
                float n = (float)(snakeNodes.Count - i) / (float)snakeNodes.Count;
                float distance = Mathf.Abs(value - n);

                snakeNodes[i].SetSize((addScale * bias) * (1 - distance));
            }

            yield return null;
        }
        snakeHead.SetToNormal();
        for (int i = 0; i < snakeNodes.Count; i++)
        {
            snakeNodes[i].transform.localScale = tmpScale;
        }
    }
    IEnumerator PlayAnimStun(float time)
    {
        float timeCount = 0;
        while (timeCount < time)
        {
            timeCount += Time.deltaTime;
            float value = punch(3, timeCount / time);
            Vector3 newPos = Vector3.Lerp(Vector3.zero, direction * 2, value);
            snakeHead.transform.parent.position = newPos;
            yield return null;
        }
        Popup.Instance.Show(totalApple);
    }
    public float punch(float amplitude, float value)
    {
        float s = 9;
        if (value == 0)
        {
            return 0;
        }
        else if (value == 1)
        {
            return 0;
        }
        float period = 1 * 0.3f;
        s = period / (2 * Mathf.PI) * Mathf.Asin(0);
        return (amplitude * Mathf.Pow(2, -10 * value) * Mathf.Sin((value * 1 - s) * (2 * Mathf.PI) / period));
    }
    #endregion
    #endregion

    #region Helper
    public static Vector3 GetDirectUp()
    {
        return new Vector3(0, 1, 0);
    }
    public static Vector3 GetDirectDown()
    {
        return new Vector3(0, -1, 0);
    }
    public static Vector3 GetDirectRight()
    {
        return new Vector3(1, 0, 0);
    }
    public static Vector3 GetDirectLeft()
    {
        return new Vector3(-1, 0, 0);
    }
    #endregion
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class BoardManager : MonoBehaviour
{
    const int MODE_OFFLINE = 0;
    const int MODE_ONLINE = 1;

    const int STATE_SLEEP = 0;
    const int STATE_PLAYER_TURN = 1;
    const int STATE_OPPONENT_TURN = 2;
    const int STATE_STANDBY = 3;

    const float ROUND_RESULT_TIME = 5.0f;

    MainManager mainManager;

    [SerializeField] Sprite[] spritesNumbersPlayerTime = new Sprite[10];
    [SerializeField] Sprite[] spritesNumbersOpponentTime = new Sprite[10];

    [SerializeField] GameObject[] gameObjectsImagesPlayerScore = new GameObject[5];
    [SerializeField] GameObject[] gameObjectsImagesOpopnentScore = new GameObject[5];

    [SerializeField] Image imageNumberPlayerTimeBig;
    [SerializeField] Image imageNumberPlayerTimeSmall1;
    [SerializeField] Image imageNumberPlayerTimeSmall2;

    [SerializeField] Image imageNumberOpponentTimeBig;
    [SerializeField] Image imageNumberOpponentTimeSmall1;
    [SerializeField] Image imageNumberOpponentTimeSmall2;

    [SerializeField] GameObject[] gameObjectImageBoard = new GameObject[10];

    [SerializeField] GameObject[] prefabsBoardAIs = new GameObject[4];

    BoardAI currentBoardAI;

    Animator[] animatorsBoard = new Animator[10];
    Animator[] animatorsPlayerScore = new Animator[5];
    Animator[] animatorsOpponentScore = new Animator[5];

    int playMode = MODE_OFFLINE;
    int boardState = STATE_SLEEP;


    int[] initialTimes = { 350, 325, 300, 275, 250, 230, 210, 190, 170, 150, 140, 130, 120, 110, 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 50 };
    int[] mainBoard = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    int timePlayer = 350;
    int timeOpponent = 350;

    int opponentThinkingCount;

    int playerScore = 0;
    int opponentScore = 0;

    int drawCount = 0;
    int continuousDrawCount = 0;

    int assignedPosition = 0;

    // Start is called before the first frame update
    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        for (int i = 0; i < 10; i++) animatorsBoard[i] = gameObjectImageBoard[i].GetComponent<Animator>();
        for (int i = 0; i < 5; i++) animatorsPlayerScore[i] = gameObjectsImagesPlayerScore[i].GetComponent<Animator>();
        for (int i = 0; i < 5; i++) animatorsOpponentScore[i] = gameObjectsImagesOpopnentScore[i].GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        imageNumberPlayerTimeBig.sprite = spritesNumbersPlayerTime[timePlayer / 50 % 10];
        imageNumberPlayerTimeSmall1.sprite = spritesNumbersPlayerTime[timePlayer / 5 % 10];
        imageNumberPlayerTimeSmall2.sprite = spritesNumbersPlayerTime[timePlayer % 5 * 2];

        imageNumberOpponentTimeBig.sprite = spritesNumbersOpponentTime[timeOpponent / 50 % 10];
        imageNumberOpponentTimeSmall1.sprite = spritesNumbersOpponentTime[timeOpponent / 5 % 10];
        imageNumberOpponentTimeSmall2.sprite = spritesNumbersOpponentTime[timeOpponent % 5 * 2];
    }

    private void FixedUpdate()
    {
        switch (playMode)
        {
            case MODE_OFFLINE:
                switch (boardState)
                {
                    case STATE_PLAYER_TURN:
                        if(timePlayer > 0)
                        {
                            timePlayer--;
                        }
                        else
                        {
                            StartCoroutine(PlayerTimeUp());
                        }
                        break;
                    case STATE_OPPONENT_TURN:
                        if(timeOpponent > 0)
                        {
                            timeOpponent--;
                        }
                        else
                        {
                            StartCoroutine(OpponentTimeUp());
                        }
                        
                        if (opponentThinkingCount <= 0)
                        {
                            int thinkingAnswer = currentBoardAI.FetchThinkingAnswer();
                            Debug.Log(thinkingAnswer);
                            switch (PutStampForOpponent(thinkingAnswer))
                            {
                                case 0: //試合続行
                                    boardState = STATE_PLAYER_TURN;
                                    animatorsBoard[0].SetTrigger("PlayerTurn");
                                    for (int i = 1; i < 10; i++)
                                    {
                                        string triggerName = i == thinkingAnswer ? "FadingBatsu" : "Invisible";
                                        animatorsBoard[i].SetTrigger(triggerName);
                                    }
                                    break;
                                case 2: //対戦相手の勝ち
                                    StartCoroutine(OpponentWin());
                                    break;
                                case 3: //引き分け
                                    StartCoroutine(DrawGame());
                                    break;
                                case 4: //対戦相手の負け（コマを重ねる）
                                    StartCoroutine(OpponentPilingViolatation());
                                    break;
                                case 5: //対戦相手の負け（指定違反）
                                    StartCoroutine(OpponentAssignedViolatation());
                                    break;
                            }
                        }
                        else
                        {
                            opponentThinkingCount--;
                        }
                        
                        break;
                }
                break;
            case MODE_ONLINE:
                break;
        }
    }

    public void PrepareForOffline(int boardAILevel)
    {
        currentBoardAI = prefabsBoardAIs[boardAILevel].GetComponent<BoardAI>();
    }

    public void PrepareForOnline()
    {
        
    }

    public void Initialize()
    {
        boardState = STATE_STANDBY;
        StartCoroutine(RoundCall());
    }

    public void Conclude()
    {
        boardState = STATE_SLEEP;
    }

    public void OnPointerDownButtonControl(int s)
    {
        if(boardState == STATE_PLAYER_TURN)
        {
            switch (PutStampForPlayer(s))
            {
                case 0: //試合続行
                    boardState = STATE_OPPONENT_TURN;
                    animatorsBoard[0].SetTrigger("OpponentTurn");
                    for(int i = 1; i < 10; i++)
                    {
                        string triggerName = i == s ? "FadingMaru" : "Invisible";
                        animatorsBoard[i].SetTrigger(triggerName);
                    }
                    currentBoardAI.SendBoardInformations(mainBoard, timeOpponent);
                    currentBoardAI.BeginThinking();
                    opponentThinkingCount = currentBoardAI.FetchThinkingCount();
                    break;
                case 1: //プレイヤーの勝ち
                    StartCoroutine(PlayerWin());
                    break;
                case 3: //引き分け
                    StartCoroutine(DrawGame());
                    break;
                case 4: //プレイヤーの負け（コマを重ねる）
                    StartCoroutine(PlayerPilingViolatation(s));
                    break;
                case 5: //プレイヤーの負け（指定違反）
                    StartCoroutine(PlayerAssignedViolatation());
                    break;
            }
        }
    }

    

    int PutStampForPlayer(int s)
    {
        if(mainBoard[s] == 0)
        {
            mainBoard[s] = 1;
            return ConfirmBoardStatus();
        }
        else
        {
            return 4;
        }
    }

    int PutStampForOpponent(int s)
    {
        if(mainBoard[s] == 0)
        {
            mainBoard[s] = 2;
            return ConfirmBoardStatus();
        }
        else
        {
            return 4;
        }
    }

    int ConfirmBoardStatus()
    {
        int[] marked1 = { 1, 1, 1, 2, 3, 3, 4, 7 };
        int[] marked2 = { 2, 4, 5, 5, 5, 6, 5, 8 };
        int[] marked3 = { 3, 7, 9, 8, 7, 9, 6, 9 };
        for (int p = 1; p <= 2; p++) 
        {
            for (int i = 0; i < 8; i++) 
            {
                if (mainBoard[marked1[i]] == p && mainBoard[marked2[i]] == p && mainBoard[marked3[i]] == p)
                {
                    return p;
                }
            }
        }
        for (int i = 1; i < 10; i++) 
        {
            if (mainBoard[i] == 0) return 0;
        }
        return 3;
    }

    void AddPlayerPoint()
    {
        animatorsPlayerScore[playerScore].SetTrigger("PlayerPoint");
        playerScore++;
        continuousDrawCount = 0;
    }

    void AddOpponentPoint()
    {
        animatorsOpponentScore[opponentScore].SetTrigger("OpponentPoint");
        opponentScore++;
        continuousDrawCount = 0;
    }

    IEnumerator PlayerWin()
    {
        int[] contempolaryBoard = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] marked1 = { 1, 1, 1, 2, 3, 3, 4, 7 };
        int[] marked2 = { 2, 4, 5, 5, 5, 6, 5, 8 };
        int[] marked3 = { 3, 7, 9, 8, 7, 9, 6, 9 };
        for (int i = 0; i < 8; i++)
        {
            if (mainBoard[marked1[i]] == 1 && mainBoard[marked2[i]] == 1 && mainBoard[marked3[i]] == 1)
            {
                contempolaryBoard[marked1[i]] = 1;
                contempolaryBoard[marked2[i]] = 1;
                contempolaryBoard[marked3[i]] = 1;
            }
        }
        for (int i = 1; i < 10; i++)
        {
            switch (contempolaryBoard[i])
            {
                case 0:
                    animatorsBoard[i].SetTrigger("Invisible");
                    break;
                case 1:
                    animatorsBoard[i].SetTrigger("Maru");
                    break;
            }
        }
        boardState = STATE_STANDBY;
        animatorsBoard[0].SetTrigger("PlayerWin");


        AddPlayerPoint();

        if(playerScore >= 5)
        {
            boardState = STATE_SLEEP;
        }
        else
        {
            yield return new WaitForSeconds(ROUND_RESULT_TIME);
            StartCoroutine(RoundCall());
        }
        
    }

    IEnumerator OpponentWin()
    {
        int[] contempolaryBoard = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
        int[] marked1 = { 1, 1, 1, 2, 3, 3, 4, 7 };
        int[] marked2 = { 2, 4, 5, 5, 5, 6, 5, 8 };
        int[] marked3 = { 3, 7, 9, 8, 7, 9, 6, 9 };
        for (int i = 0; i < 8; i++)
        {
            if (mainBoard[marked1[i]] == 2 && mainBoard[marked2[i]] == 2 && mainBoard[marked3[i]] == 2)
            {
                contempolaryBoard[marked1[i]] = 1;
                contempolaryBoard[marked2[i]] = 1;
                contempolaryBoard[marked3[i]] = 1;
            }
        }
        for (int i = 1; i < 10; i++)
        {
            switch (contempolaryBoard[i])
            {
                case 0:
                    animatorsBoard[i].SetTrigger("Invisible");
                    break;
                case 1:
                    animatorsBoard[i].SetTrigger("Batsu");
                    break;
            }
        }
        boardState = STATE_STANDBY;
        animatorsBoard[0].SetTrigger("OpponentWin");

        AddOpponentPoint();

        if(opponentScore >= 5)
        {
            boardState = STATE_SLEEP;
        }
        else
        {
            yield return new WaitForSeconds(ROUND_RESULT_TIME);
            StartCoroutine(RoundCall());
        }


    }

    IEnumerator PlayerTimeUp()
    {
        boardState = STATE_STANDBY;
        animatorsBoard[0].SetTrigger("OpponentWin");
        AddOpponentPoint();

        if (opponentScore >= 5)
        {
            boardState = STATE_SLEEP;
            yield return new WaitForSeconds(ROUND_RESULT_TIME);
            mainManager.MoveToResultPanel();
        }
        else
        {
            yield return new WaitForSeconds(ROUND_RESULT_TIME);
            StartCoroutine(RoundCall());
        }
    }

    IEnumerator OpponentTimeUp()
    {
        boardState = STATE_STANDBY;
        animatorsBoard[0].SetTrigger("PlayerWin");
        AddPlayerPoint();

        if (playerScore >= 5)
        {
            yield return new WaitForSeconds(ROUND_RESULT_TIME);
            mainManager.MoveToResultPanel();
            boardState = STATE_SLEEP;
        }
        else
        {
            yield return new WaitForSeconds(ROUND_RESULT_TIME);
            StartCoroutine(RoundCall());
        }
    }

    IEnumerator PlayerPilingViolatation(int s)
    {
        boardState = STATE_STANDBY;
        animatorsBoard[0].SetTrigger("OpponentWin");
        if (mainBoard[s] == 1) animatorsBoard[s].SetTrigger("ViolationMaru");
        if (mainBoard[s] == 2) animatorsBoard[s].SetTrigger("ViolationBatsu");

        AddOpponentPoint();

        if (opponentScore >= 5)
        {
            boardState = STATE_SLEEP;
            yield return new WaitForSeconds(ROUND_RESULT_TIME);
            mainManager.MoveToResultPanel();
        }
        else
        {
            yield return new WaitForSeconds(ROUND_RESULT_TIME);
            StartCoroutine(RoundCall());
        }
    }

    IEnumerator OpponentPilingViolatation()
    {
        boardState = STATE_STANDBY;
        animatorsBoard[0].SetTrigger("PlayerWin");
        AddPlayerPoint();

        if (playerScore >= 5)
        {
            boardState = STATE_SLEEP;
            yield return new WaitForSeconds(ROUND_RESULT_TIME);
            mainManager.MoveToResultPanel();
        }
        else
        {
            yield return new WaitForSeconds(ROUND_RESULT_TIME);
            StartCoroutine(RoundCall());
        }
    }

    IEnumerator PlayerAssignedViolatation()
    {
        boardState = STATE_STANDBY;
        yield return new WaitForSeconds(ROUND_RESULT_TIME);
    }

    IEnumerator OpponentAssignedViolatation()
    {
        boardState = STATE_STANDBY;
        yield return new WaitForSeconds(ROUND_RESULT_TIME);
    }

    IEnumerator DrawGame()
    {
        boardState = STATE_STANDBY;
        drawCount++;
        if (continuousDrawCount < 24) continuousDrawCount++;

        animatorsBoard[0].SetTrigger("Draw");
        for (int i = 1; i < 10; i++)
        {
            string[] triggerNames = { "Invisible", "Maru", "Batsu" };
            animatorsBoard[i].SetTrigger(triggerNames[mainBoard[i]]);
        }
        yield return new WaitForSeconds(3.0f);
        StartCoroutine(RoundCall());
    }

    IEnumerator RoundCall()
    {
        animatorsBoard[0].SetTrigger("Idle");
        for (int i = 1; i < 10; i++)
        {
            animatorsBoard[i].SetTrigger("Invisible");
            mainBoard[i] = 0;
        }
        timePlayer = initialTimes[continuousDrawCount];
        timeOpponent = initialTimes[continuousDrawCount];

        yield return new WaitForSeconds(2.0f);
        Debug.Log("FIGHT");
        if((playerScore + opponentScore + drawCount) % 2 == 0)
        {
            boardState = STATE_PLAYER_TURN;
            animatorsBoard[0].SetTrigger("PlayerTurn");
        }
        else
        {
            boardState = STATE_OPPONENT_TURN;
            animatorsBoard[0].SetTrigger("OpponentTurn");
        }

    }
}

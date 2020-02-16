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

    MainManager mainManager;

    [SerializeField] Sprite[] spritesNumbersPlayerTime = new Sprite[10];
    [SerializeField] Sprite[] spritesNumbersOpponentTime = new Sprite[10];

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

    int playMode = MODE_OFFLINE;
    int boardState = STATE_SLEEP;


    int[] initialTimes = { 350, 325, 300, 275, 250, 230, 210, 190, 170, 150, 140, 130, 120, 110, 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 50 };
    int[] mainBoard = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    int timePlayer = 350;
    int timeOpponent = 350;

    int drawCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        for (int i = 0; i < 10; i++) animatorsBoard[i] = gameObjectImageBoard[i].GetComponent<Animator>();
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

    }

    public void OnPointerDownButtonControl(int s)
    {
        if(boardState == STATE_PLAYER_TURN)
        {
            switch (PutStampForPlayer(s))
            {
                case 0: //試合続行
                    boardState = STATE_OPPONENT_TURN;
                    for(int i = 1; i < 10; i++)
                    {
                        string triggerName = i == s ? "FadingMaru" : "Invisible";
                        animatorsBoard[i].SetTrigger(triggerName);
                    }
                    currentBoardAI.BeginThinking();
                    break;
                case 1: //プレイヤーの勝ち
                    StartCoroutine(PlayerWin());
                    break;
                case 3: //引き分け
                    StartCoroutine(DrawGame());
                    break;
                case 4: //プレイヤーの負け（コマを重ねる）
                    StartCoroutine(PlayerPilingViolatation());
                    break;
                case 5: //プレイヤーの負け（指定違反）
                    StartCoroutine(PlayerAssignedViolatation());
                    break;
            }
        }
        if(boardState == STATE_OPPONENT_TURN)
        {
            switch (PutStampForOpponent(s))
            {
                case 0: //試合続行
                    boardState = STATE_PLAYER_TURN;
                    for (int i = 1; i < 10; i++)
                    {
                        string triggerName = i == s ? "FadingBatsu" : "Invisible";
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
            return 2;
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
            return 1;
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

    }

    void AddOpponentPoint()
    {

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
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(RoundCall());
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
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(RoundCall());
    }

    IEnumerator PlayerTimeUp()
    {
        boardState = STATE_STANDBY;
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(RoundCall());
    }

    IEnumerator OpponentTimeUp()
    {
        boardState = STATE_STANDBY;
        yield return new WaitForSeconds(1.0f);
        StartCoroutine(RoundCall());
    }

    IEnumerator PlayerPilingViolatation()
    {
        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator OpponentPilingViolatation()
    {
        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator PlayerAssignedViolatation()
    {
        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator OpponentAssignedViolatation()
    {
        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator DrawGame()
    {
        boardState = STATE_STANDBY;
        if (drawCount < 24) drawCount++;

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
        for (int i = 1; i < 10; i++)
        {
            animatorsBoard[i].SetTrigger("Invisible");
            mainBoard[i] = 0;
        }
        timePlayer = initialTimes[drawCount];
        timeOpponent = initialTimes[drawCount];

        yield return new WaitForSeconds(2.0f);
        Debug.Log("FIGHT");
        boardState = STATE_PLAYER_TURN;
    }
}

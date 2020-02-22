using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAIVeryStrong : BoardAI
{
    int[] encodeBoard = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    int[] decodeBoard = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };

    int[] mainBoard = { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
    int timerCount = 0;
    int thinkingAnswer = 0;
    int thinkingCount = 0;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        if (thinkingCount > 0) thinkingCount--;
    }

    public override void Initialize()
	{
		base.Initialize();
	}

	public override void SendBoardInformations(int[] mainBoard, int opponentCount)
	{
        this.mainBoard = mainBoard;
        timerCount = opponentCount;
	}

	public override void BeginThinking()
    {
        thinkingAnswer = 0;
        do
        {
            thinkingAnswer = Random.Range(1, 10);
        } while (mainBoard[thinkingAnswer] != 0);

        int count = 0;
        for (int i = 1; i < 10; i++) if (mainBoard[i] != 0) count++;

        switch (count)
        {
            case 0: // 先攻第一手
                if (Random.Range(0, 100) < 70) 
                {
                    thinkingAnswer = 1;
                }
                else if (Random.Range(0, 100) < 70) 
                {
                    thinkingAnswer = 5;
                }
                else
                {
                    thinkingAnswer = 2;
                }
                break;
            case 1: // 後攻第一手
                if (mainBoard[5] == 0)
                {
                    thinkingAnswer = 5;
                }
                else
                {
                    thinkingAnswer = 1;
                }
                break;
            case 2: // 先攻第二手
                int[] frontSecondOpponent = { 1, 1, 1, 1, 1, 1, 1, 1, 5, 5, 5, 5, 5, 5, 5, 5, 2, 2, 2, 2, 2, 2, 2, 2 };
                int[] frontSecondPlayer = { 2, 3, 4, 5, 6, 7, 8, 9, 1, 2, 3, 4, 6, 7, 8, 9, 1, 3, 4, 5, 6, 7, 8, 9 };
                int[] frontSecondAnswer = { 5, 7, 2, 8, 5, 2, 5, 3, 2, 1, 7, 2, 1, 8, 1, 3, 5, 6, 1, 4, 3, 1, 7, 3 };
                for (int i = 0; i < 24; i++)
                {
                    if (mainBoard[frontSecondOpponent[i]] == 2 && mainBoard[frontSecondPlayer[i]] == 1)
                    {
                        if (i == 3 && Random.Range(0, 100) < 50) 
                        {
                            thinkingAnswer = 9;
                        }
                        else
                        {
                            thinkingAnswer = frontSecondAnswer[1];
                        }
                    }
                }
                break;
            case 3: // 後攻第二手
                int[] backSecondOpponent = { 5, 5, 1, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5 };
                int[] backSecondPlayer1 = { 1, 3, 5, 1, 3, 3, 4, 2, 2, 1, 6, 2, 2, 6, 4, 2, 4 };
                int[] backSecondPlayer2 = { 9, 7, 9, 8, 8, 4, 9, 7, 9, 6, 7, 4, 6, 8, 8, 8, 6 };
                int[] backSecondAnswer = { 2, 2, 3, 4, 6, 1, 7, 6, 4, 9, 3, 1, 1, 9, 1, 1, 1 };
                for (int i = 0; i < 17; i++)
                {
                    if(mainBoard[backSecondOpponent[i]]==2 && mainBoard[backSecondPlayer1[i]]==1 && mainBoard[backSecondPlayer2[i]] == 1)
                    {
                        thinkingAnswer = backSecondAnswer[i];
                    }
                }
                break;
            case 4: // 先攻第三手
                int[] frontThirdOpponent1 = { 1, 1, 1, 1, 1, 1, 2, 5, 5, 2, 5, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2, 2 };
                int[] frontThirdOpponent2 = { 5, 7, 2, 8, 8, 3, 5, 7, 7, 5, 8, 6, 6, 6, 6, 4, 4, 3, 7, 7, 7, 7 };
                int[] frontThirdPlayer1 = { 2, 3, 3, 2, 5, 2, 1, 3, 3, 4, 2, 1, 3, 3, 3, 5, 5, 1, 4, 5, 6, 8 };
                int[] frontThirdPlayer2 = { 9, 4, 4, 5, 9, 9, 8, 4, 8, 8, 7, 3, 4, 8, 9, 6, 8, 6, 8, 8, 8, 9 };
                int[] frontThirdAnswer = { 4, 9, 5, 7, 4, 7, 7, 8, 1, 1, 1, 5, 5, 5, 5, 1, 1, 5, 3, 1, 1, 1 };
                for (int i = 0; i < 22; i++)
                {
                    if (mainBoard[frontThirdOpponent1[i]] == 2 && mainBoard[frontThirdOpponent2[i]] == 2 && mainBoard[frontThirdPlayer1[i]] == 1 && mainBoard[frontThirdPlayer2[i]] == 1)
                    {
                        thinkingAnswer = frontThirdAnswer[i];
                    }
                }
                break;
            case 5: // 後攻第三手
                if (mainBoard[4] == 2 && mainBoard[5] == 2 && mainBoard[1] == 1 & mainBoard[6] == 1 && mainBoard[8] == 1) 
                {
                    if (Random.Range(0, 100) < 65)
                    {
                        thinkingAnswer = 9;
                    }
                    else
                    {
                        thinkingAnswer = 7;
                    }
                }
                if (mainBoard[5] == 2 && mainBoard[6] == 2 && mainBoard[3] == 1 && mainBoard[4] == 1 && mainBoard[8] == 1)
                {
                    if (Random.Range(0, 100) < 65)
                    {
                        thinkingAnswer = 1;
                    }
                    else
                    {
                        thinkingAnswer = 2;
                    }
                }
                break;
        }

        int playerReached = checkReached(1);
        if (playerReached != 0) thinkingAnswer = playerReached;

        int opponentReached = checkReached(2);
        if (opponentReached != 0) thinkingAnswer = opponentReached;

        thinkingCount = Random.Range(5, 20);
    }

	public override int FetchThinkingAnswer()
	{
        return thinkingAnswer;
	}

    public override int FetchThinkingCount()
    {
        return thinkingCount;
    }

    int checkReached(int s)
	{
        int[] occupied1 = { 1, 1, 1, 1, 1, 1, 2, 2, 2, 3, 3, 3, 3, 4, 4, 4, 5, 5, 5, 5, 6, 7, 7, 8 };
        int[] occupied2 = { 2, 3, 4, 5, 7, 9, 3, 5, 8, 5, 6, 7, 9, 5, 6, 7, 6, 7, 8, 9, 9, 8, 9, 9 };
        int[] vacant = { 3, 2, 7, 9, 4, 5, 1, 8, 5, 7, 9, 5, 6, 6, 5, 1, 4, 3, 2, 1, 3, 9, 8, 7 };
        for (int i = 0; i < 24; i++)
		{
            if (mainBoard[occupied1[i]] == s && mainBoard[occupied2[i]] == s && mainBoard[vacant[i]] == 0) 
			{
                return vacant[i];
			}
		}
        return 0;
	}
}

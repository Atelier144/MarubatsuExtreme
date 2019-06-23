using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour {
    MainManager mainManager;

    [SerializeField] GameObject[] gameObjectsMasu = new GameObject[9];
    [SerializeField] GameObject[] gameObjectsBo = new GameObject[16];
    [SerializeField] GameObject gameObjectPanelMain;
    [SerializeField] GameObject[] gameObjectsKomaMaru = new GameObject[9];
    [SerializeField] GameObject[] gameObjectsKomaBatsu = new GameObject[9];

    SpriteRenderer[] spriteRenderersMasu = new SpriteRenderer[9];
    SpriteRenderer[] spriteRenderersBo = new SpriteRenderer[16];

    [SerializeField] GameObject prefabFadingMaru;
    [SerializeField] GameObject prefabFadingBatsu;

    [SerializeField] Text textPlayerGameTime;
    [SerializeField] Text textComputerGameTime;

    Animator animator;

    const int NOTHING = 0;
    const int PLAYER = 1;
    const int COMPUTER = 2;

    Color colorWhite = new Color(1.0f, 1.0f, 1.0f);
    Color colorGray = new Color(0.5f, 0.5f, 0.5f);
    Color colorLightBlue = new Color(0.5f, 1.0f, 1.0f);
    Color colorLightYellow = new Color(1.0f, 1.0f, 0.5f);
    Color colorDarkGray = new Color(0.2f, 0.2f, 0.2f);
    Color colorRed = new Color(1.0f, 0.0f, 0.0f);

    Vector3[] positionsKoma =
    {
        new Vector3(-360.0f, -260.0f, 0.0f),
        new Vector3(0.0f, -260.0f, 0.0f),
        new Vector3(360.0f, -260.0f, 0.0f),
        new Vector3(-360.0f, 100.0f, 0.0f),
        new Vector3(0.0f, 100.0f, 0.0f),
        new Vector3(360.0f, 100.0f, 0.0f),
        new Vector3(-360.0f, 460.0f, 0.0f),
        new Vector3(0.0f, 460.0f, 0.0f),
        new Vector3(360.0f, 460.0f, 0.0f)

    };

    int[] board = { 0, 0, 0, 0, 0, 0, 0, 0, 0 };

    int currentTurn = NOTHING;

    int playerGameTime = 350;
    int computerGameTime = 350;

    int playerPoint = 0;
    int computerPoint = 0;

    int computerPutTime = 40;

    int resetTime;

    int overIndex = -1;
	// Use this for initialization
	void Start () {
        mainManager = GameObject.Find("MainManager").GetComponent<MainManager>();

        for (int i = 0; i < 9; i++) spriteRenderersMasu[i] = gameObjectsMasu[i].GetComponent<SpriteRenderer>();
        for (int i = 0; i < 16; i++) spriteRenderersBo[i] = gameObjectsBo[i].GetComponent<SpriteRenderer>();

        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        int playerTimeBig = playerGameTime / 50;
        int playerTimeSmall = playerGameTime % 50 * 2;
        int computerTimeBig = computerGameTime / 50;
        int computerTimeSmall = computerGameTime % 50 * 2;
        string playerText = playerTimeBig.ToString() + "." + playerTimeSmall.ToString() + '"';
        string computerText = computerTimeBig.ToString() + "." + computerTimeSmall.ToString() + '"';
        textPlayerGameTime.text = playerText;
        textComputerGameTime.text = computerText;
    }

    private void FixedUpdate()
    {
        if(currentTurn == PLAYER)
        {
            if (playerGameTime >= 0) playerGameTime--;
            else GiveComputerPoint();
        }
        if(currentTurn == COMPUTER)
        {
            if (playerGameTime >= 0) computerGameTime--;
            else GivePlayerPoint();

            if (computerPutTime > 0) computerPutTime--;
            else PutKomaComputer(Random.Range(0, 9));
        }
        if(currentTurn == 3)
        {
            resetTime--;
            if (resetTime <= 0) Reset();
        }
    }

    public void PushMasuButton(int s)
    {
        PutKomaPlayer(s);
    }

    public int ConfirmGameSet(int s)
    {
        int[] inspection1 = { 0, 0, 0, 1, 2, 2, 3, 6 };
        int[] inspection2 = { 1, 3, 4, 4, 4, 5, 4, 7 };
        int[] inspection3 = { 2, 6, 8, 7, 6, 8, 5, 8 };
        for (int i = 0; i < 8; i++) if (board[inspection1[i]] == s && board[inspection2[i]] == s && board[inspection3[i]] == s) return 1;
        for (int i = 0; i < 9; i++) if (board[i] == NOTHING) return 0;
        return 2;
    }

    public void PutKomaPlayer(int s)
    {
        if(currentTurn == PLAYER)
        {
            if (board[s] == NOTHING)
            {
                board[s] = PLAYER;
                DestroyAllKoma();
                switch (ConfirmGameSet(PLAYER))
                {
                    case 0:
                        ChangeTurnToComputer();
                        GenerateFadingMaru(s);
                        break;
                    case 1:
                        GivePlayerPoint();
                        break;
                    case 2:
                        GiveNoPoint();
                        break;
                }
            }
        }
    }

    public void PutKomaComputer(int s)
    {
        if(currentTurn == COMPUTER)
        {
            if (board[s] == NOTHING)
            {
                board[s] = COMPUTER;
                DestroyAllKoma();

                switch (ConfirmGameSet(COMPUTER))
                {
                    case 0:
                        ChangeTurnToPlayer();
                        GenerateFadingBatsu(s);
                        break;
                    case 1:
                        GiveComputerPoint();
                        break;
                    case 2:
                        GiveNoPoint();
                        break;
                }
            }
            else
            {
                overIndex = s;
                GivePlayerPoint();
            }
        }
    }

    public void ChangeTurnToPlayer()
    {
        currentTurn = PLAYER;
        gameObjectPanelMain.SetActive(true);
        for (int i = 0; i < 9; i++) spriteRenderersMasu[i].color = colorWhite;
        for (int i = 0; i < 16; i++) spriteRenderersBo[i].color = colorWhite;
    }

    public void ChangeTurnToComputer()
    {
        currentTurn = COMPUTER;
        gameObjectPanelMain.SetActive(false);
        for (int i = 0; i < 9; i++) spriteRenderersMasu[i].color = colorGray;
        for (int i = 0; i < 16; i++) spriteRenderersBo[i].color = colorGray;

        computerPutTime = Random.Range(30, 70);
    }

    public void GivePlayerPoint()
    {
        currentTurn = 3;
        resetTime = 250;
        gameObjectPanelMain.SetActive(false);
        DrawLinePlayer();
    }

    public void GiveComputerPoint()
    {
        currentTurn = 3;
        resetTime = 250;
        gameObjectPanelMain.SetActive(false);
        for (int i = 0; i < 9; i++) spriteRenderersMasu[i].color = colorLightYellow;
        for (int i = 0; i < 16; i++) spriteRenderersBo[i].color = colorLightYellow;
    }

    public void GiveNoPoint()
    {
        currentTurn = 3;
        resetTime = 250;
        gameObjectPanelMain.SetActive(false);
        for (int i = 0; i < 9; i++) spriteRenderersMasu[i].color = colorGray;
        for (int i = 0; i < 16; i++) spriteRenderersBo[i].color = colorGray;
    }

    public void DrawLinePlayer()
    {
        for (int i = 0; i < 9; i++) spriteRenderersMasu[i].color = colorDarkGray;
        for (int i = 0; i < 16; i++) spriteRenderersBo[i].color = colorDarkGray;
        if(board[0] == PLAYER && board[1] == PLAYER && board[2] == PLAYER)
        {
            gameObjectsKomaMaru[0].SetActive(true);
            gameObjectsKomaMaru[1].SetActive(true);
            gameObjectsKomaMaru[2].SetActive(true);
            spriteRenderersMasu[0].color = colorLightBlue;
            spriteRenderersMasu[1].color = colorLightBlue;
            spriteRenderersMasu[2].color = colorLightBlue;
            spriteRenderersBo[6].color = colorLightBlue;
            spriteRenderersBo[9].color = colorLightBlue;
        }
        if (board[0] == PLAYER && board[3] == PLAYER && board[6] == PLAYER)
        {
            gameObjectsKomaMaru[0].SetActive(true);
            gameObjectsKomaMaru[3].SetActive(true);
            gameObjectsKomaMaru[6].SetActive(true);
            spriteRenderersMasu[0].color = colorLightBlue;
            spriteRenderersMasu[3].color = colorLightBlue;
            spriteRenderersMasu[6].color = colorLightBlue;
            spriteRenderersBo[0].color = colorLightBlue;
            spriteRenderersBo[3].color = colorLightBlue;
        }
        if (board[0] == PLAYER && board[4] == PLAYER && board[8] == PLAYER)
        {
            gameObjectsKomaMaru[0].SetActive(true);
            gameObjectsKomaMaru[4].SetActive(true);
            gameObjectsKomaMaru[8].SetActive(true);
            spriteRenderersMasu[0].color = colorLightBlue;
            spriteRenderersMasu[4].color = colorLightBlue;
            spriteRenderersMasu[8].color = colorLightBlue;
            spriteRenderersBo[14].color = colorLightBlue;
            spriteRenderersBo[15].color = colorLightBlue;
        }
        if (board[1] == PLAYER && board[4] == PLAYER && board[7] == PLAYER)
        {
            gameObjectsKomaMaru[1].SetActive(true);
            gameObjectsKomaMaru[4].SetActive(true);
            gameObjectsKomaMaru[7].SetActive(true);
            spriteRenderersMasu[1].color = colorLightBlue;
            spriteRenderersMasu[4].color = colorLightBlue;
            spriteRenderersMasu[7].color = colorLightBlue;
            spriteRenderersBo[1].color = colorLightBlue;
            spriteRenderersBo[4].color = colorLightBlue;
        }
        if (board[2] == PLAYER && board[4] == PLAYER && board[6] == PLAYER)
        {
            gameObjectsKomaMaru[2].SetActive(true);
            gameObjectsKomaMaru[4].SetActive(true);
            gameObjectsKomaMaru[6].SetActive(true);
            spriteRenderersMasu[2].color = colorLightBlue;
            spriteRenderersMasu[4].color = colorLightBlue;
            spriteRenderersMasu[6].color = colorLightBlue;
            spriteRenderersBo[12].color = colorLightBlue;
            spriteRenderersBo[13].color = colorLightBlue;
        }
        if (board[2] == PLAYER && board[5] == PLAYER && board[8] == PLAYER)
        {
            gameObjectsKomaMaru[2].SetActive(true);
            gameObjectsKomaMaru[5].SetActive(true);
            gameObjectsKomaMaru[8].SetActive(true);
            spriteRenderersMasu[2].color = colorLightBlue;
            spriteRenderersMasu[5].color = colorLightBlue;
            spriteRenderersMasu[8].color = colorLightBlue;
            spriteRenderersBo[2].color = colorLightBlue;
            spriteRenderersBo[5].color = colorLightBlue;
        }
        if (board[3] == PLAYER && board[4] == PLAYER && board[5] == PLAYER)
        {
            gameObjectsKomaMaru[3].SetActive(true);
            gameObjectsKomaMaru[4].SetActive(true);
            gameObjectsKomaMaru[5].SetActive(true);
            spriteRenderersMasu[3].color = colorLightBlue;
            spriteRenderersMasu[4].color = colorLightBlue;
            spriteRenderersMasu[5].color = colorLightBlue;
            spriteRenderersBo[7].color = colorLightBlue;
            spriteRenderersBo[10].color = colorLightBlue;
        }
        if (board[6] == PLAYER && board[7] == PLAYER && board[8] == PLAYER)
        {
            gameObjectsKomaMaru[6].SetActive(true);
            gameObjectsKomaMaru[7].SetActive(true);
            gameObjectsKomaMaru[8].SetActive(true);
            spriteRenderersMasu[6].color = colorLightBlue;
            spriteRenderersMasu[7].color = colorLightBlue;
            spriteRenderersMasu[8].color = colorLightBlue;
            spriteRenderersBo[8].color = colorLightBlue;
            spriteRenderersBo[11].color = colorLightBlue;
        }
        if(overIndex != -1)
        {
            spriteRenderersMasu[overIndex].color = colorRed;
        }
    }
        
        public void DestroyAllKoma()
    {
        GameObject[] gameObjectsKoma = GameObject.FindGameObjectsWithTag("Koma");
        foreach (GameObject gameObjectKoma in gameObjectsKoma) Destroy(gameObjectKoma);
    }

    public void GenerateFadingMaru(int s)
    {
        Instantiate(prefabFadingMaru, positionsKoma[s], new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
    }

    public void GenerateFadingBatsu(int s)
    {
        Instantiate(prefabFadingBatsu, positionsKoma[s], new Quaternion(0.0f, 0.0f, 0.0f, 0.0f));
    }

    public void Initialize()
    {
        ChangeTurnToPlayer();
    }

    public void Reset()
    {
        for (int i = 0; i < 9; i++)
        {
            board[i] = NOTHING;
            gameObjectsKomaMaru[i].SetActive(false);
            gameObjectsKomaBatsu[i].SetActive(false);
        }
        ChangeTurnToPlayer();
        overIndex = -1;
        playerGameTime = 350;
        computerGameTime = 350;
    }
}

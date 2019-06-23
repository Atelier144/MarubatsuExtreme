using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

    BoardManager boardManager;

    [SerializeField] GameObject mainCamera;

    [SerializeField] GameObject gameObjectPanelTitle;
    [SerializeField] GameObject gameObjectPanelSelectLevel;

    [SerializeField] GameObject gameObjectTitles;

	// Use this for initialization
	void Start () {
        if (Screen.height * 9 > Screen.width * 16)
        {
            int size = Screen.height * 540 / Screen.width;
            mainCamera.GetComponent<Camera>().orthographicSize = size;
        }

        boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetActivePanelTitle()
    {
        gameObjectPanelTitle.SetActive(true);
    }

    public void MoveToMain()
    {
        boardManager.Initialize();
    }

    public void PushLoginButton()
    {

    }

    public void PushOfflineStartButton()
    {
        gameObjectPanelTitle.SetActive(false);
        gameObjectPanelSelectLevel.SetActive(true);
    }

    public void PushOnlineStartButton()
    {

    }

    public void PushOptionButton()
    {

    }

    public void PushEasyButton()
    {
        gameObjectPanelSelectLevel.SetActive(false);
        gameObjectTitles.GetComponent<Titles>().SetTrigger("MoveOut");
    }

    public void PushNormalButton()
    {

    }

    public void PushHardButton()
    {

    }
}

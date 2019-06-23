using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour {

    [SerializeField] GameObject mainCamera;

    [SerializeField] GameObject gameObjectPanelTitle;
    [SerializeField] GameObject gameObjectPanelSelectLevel;

	// Use this for initialization
	void Start () {
        if (Screen.height * 9 > Screen.width * 16)
        {
            int size = Screen.height * 540 / Screen.width;
            mainCamera.GetComponent<Camera>().orthographicSize = size;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SetActivePanelTitle()
    {
        gameObjectPanelTitle.SetActive(true);
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

    }

    public void PushNormalButton()
    {

    }

    public void PushHardButton()
    {

    }
}

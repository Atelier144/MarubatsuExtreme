using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainManager : MonoBehaviour {


    [SerializeField] GameObject gameObjectPanelMain;

    [SerializeField] GameObject gameObjectPanelIntroduction;
    [SerializeField] GameObject gameObjectPanelTitle;
    [SerializeField] GameObject gameObjectPanelSkipIntroduction;

    [SerializeField] GameObject gameObjectPanelSelectMode;
    [SerializeField] GameObject gameObjectPanelLoginForm;

    [SerializeField] GameObject gameObjectButtonStart;
    [SerializeField] GameObject gameObjectButtonLogin;
    [SerializeField] GameObject gameObjectButtonSettings;

    [SerializeField] Image imageIntroduction;
    [SerializeField] Image imageTitle;

    BoardManager boardManager;

    Sequence sequenceImageIntroduction;
    Sequence sequenceImageTitle;

    // Use this for initialization
    void Start () {
        boardManager = GameObject.Find("BoardManager").GetComponent<BoardManager>();

        StartCoroutine(CoroutineIntroduction());
    }

    // Update is called once per frame
    void Update () {
    
    }

    public void OnPointerDownButtonSkipIntroduction()
    {
        OnFinishIntroduction();
    }

    public void OnClickButtonStart()
    {
        StartCoroutine(CoroutineOnClickButtonStart());
    }

    public void OnClickButtonLogin()
    {
        StartCoroutine(CoroutineOnClickButtonLogin());
    }

    public void OnClickButtonSettings()
    {

    }

    public void OnClickButtonBackToTitle()
    {
        gameObjectPanelMain.transform.DOLocalMove(new Vector3(1620.0f, 0.0f, 0.0f), 0.4f);
    }

    public void OnClickButtonOffline()
    {
        StartCoroutine(CoroutineOnClickButtonOffline());
    }

    public void OnClickButtonOnline()
    {
        StartCoroutine(CoroutineOnClickButtonOnline());
    }

    public void OnClickButtonGameStart(int level)
    {
        boardManager.PrepareForOffline(level);
        StartCoroutine(CoroutineOnClickButtonGameStart());
    }

    public void OnClickButtonLoginSubmit()
    {
        StartCoroutine(CoroutineOnClickButtonLoginSubmit());
    }

    public void OnClickButtonAtelier144Signup()
    {
        StartCoroutine(CoroutineOnClickButtonAtelier144Signup());
    }

    void OnFinishIntroduction()
    {

        gameObjectPanelIntroduction.SetActive(false);
        gameObjectPanelTitle.SetActive(true);
        gameObjectPanelSkipIntroduction.SetActive(false);

        sequenceImageIntroduction.Kill();
        sequenceImageTitle.Kill();

        imageIntroduction.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
        imageTitle.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);

        gameObjectButtonStart.SetActive(true);
        gameObjectButtonLogin.SetActive(true);
        gameObjectButtonSettings.SetActive(true);
    }

    IEnumerator CoroutineIntroduction()
    {
        yield return new WaitForSeconds(1.0f);

        // イントロダクションを徐々に表示させて、徐々に消滅させるDOTween Sequense
        sequenceImageIntroduction = DOTween.Sequence();
        sequenceImageIntroduction.Append(imageIntroduction.DOColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), 3.0f));
        sequenceImageIntroduction.Append(imageIntroduction.DOColor(new Color(1.0f, 1.0f, 1.0f, 0.0f), 3.0f));

        yield return new WaitForSeconds(6.0f);

        gameObjectPanelIntroduction.SetActive(false);
        gameObjectPanelTitle.SetActive(true);

        // タイトルを徐々に表示させるDOTween Sequense
        sequenceImageTitle = DOTween.Sequence();
        sequenceImageTitle.Append(imageTitle.DOColor(new Color(1.0f, 1.0f, 1.0f, 1.0f), 4.0f));


        yield return new WaitForSeconds(4.0f);

        OnFinishIntroduction();

    }

    IEnumerator CoroutineOnClickButtonStart()
    {
        gameObjectPanelSelectMode.SetActive(true);
        gameObjectPanelLoginForm.SetActive(false);
        yield return new WaitForSeconds(1.0f);
        gameObjectPanelMain.transform.DOLocalMove(new Vector3(810.0f, 0.0f, 0.0f), 0.5f);
    }

    IEnumerator CoroutineOnClickButtonLogin()
    {
        gameObjectPanelSelectMode.SetActive(false);
        gameObjectPanelLoginForm.SetActive(true);
        yield return new WaitForSeconds(1.0f);
        gameObjectPanelMain.transform.DOLocalMove(new Vector3(810.0f, 0.0f, 0.0f), 0.5f);
    }

    IEnumerator CoroutineOnClickButtonOffline()
    {
        yield return new WaitForSeconds(1.0f);
        gameObjectPanelMain.transform.DOLocalMove(new Vector3(0.0f, 0.0f, 0.0f), 0.5f);

    }

    IEnumerator CoroutineOnClickButtonOnline()
    {
        yield return new WaitForSeconds(1.0f);
    }

    IEnumerator CoroutineOnClickButtonLoginSubmit()
    {
        SceneManager.LoadScene("LoadingScene", LoadSceneMode.Additive);
        yield return new WaitForSeconds(3.0f);
        SceneManager.UnloadSceneAsync("LoadingScene");
    }

    IEnumerator CoroutineOnClickButtonAtelier144Signup()
    {
        yield return new WaitForSeconds(1.0f);
        Application.OpenURL("https://gameatelier144.com/signup");
    }

    IEnumerator CoroutineOnClickButtonGameStart()
    {
        yield return new WaitForSeconds(1.0f);
        gameObjectPanelMain.transform.DOLocalMove(new Vector3(-810.0f, 0.0f, 0.0f), 0.5f).OnComplete(()=> { boardManager.Initialize(); });
    }
}

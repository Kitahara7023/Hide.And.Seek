using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("ゲームシーン名")]
    public string gameSceneName = "GameScene";

    [Header("HOW TO PLAY")]
    public GameObject howToPlayPanel;
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;

    // STARTボタンを押した時
    public void StartGame()
    {
        Debug.Log("ゲームを開始します");

        SceneManager.LoadScene(gameSceneName);
    }

    // HOW TO PLAYを開いた時
    public void OpenHowToPlay()
    {
        // HOW TO PLAY画面を表示
        howToPlayPanel.SetActive(true);

        // 最初はPage1を表示
        page1.SetActive(true);

        // Page2、Page3は非表示
        page2.SetActive(false);
        page3.SetActive(false);

        // タイトル画面を非表示
        GameObject titlePanel = GameObject.Find("TitlePanel");

        if (titlePanel != null)
        {
            titlePanel.SetActive(false);
        }

        Debug.Log("HOW TO PLAYを開きました");
    }

    // Page1 → Page2
    public void NextPage()
    {
        page1.SetActive(false);
        page2.SetActive(true);
    }

    // Page2 → Page3
    public void NextPage2()
    {
        page2.SetActive(false);
        page3.SetActive(true);
    }

    // Page2 → Page1
    public void BackPage1()
    {
        page1.SetActive(true);
        page2.SetActive(false);
    }

    // Page3 → Page2
    public void BackPage2()
    {
        page2.SetActive(true);
        page3.SetActive(false);
    }

    // EXITボタンを押した時
    public void ExitGame()
    {
        Debug.Log("ゲームを終了します");

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
    Application.Quit();
#endif
    }
}
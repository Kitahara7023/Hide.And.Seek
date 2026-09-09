using TMPro;
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

    [Header("ゲーム設定")]
    public GameObject settingPanel;

    [Header("ゲーム設定Dropdown")]
    public TMP_Dropdown panelSizeDropdown;
    public TMP_Dropdown bombCountDropdown;
    public TMP_Dropdown turnCountDropdown;
    public TMP_Dropdown dwarfCountDropdown;

    // STARTボタンを押した時
    public void StartGame()
    {
        Debug.Log("ゲーム設定画面を開きます");

        // タイトル画面を非表示
        GameObject titlePanel = GameObject.Find("TitlePanel");

        if (titlePanel != null)
        {
            titlePanel.SetActive(false);
        }

        // 設定画面を表示
        settingPanel.SetActive(true);
    }

    // GAME STARTボタンを押した時
    public void StartGameWithSettings()
    {
        // パネルサイズを取得
        switch (panelSizeDropdown.value)
        {
            case 0:
                GameSettings.Instance.panelWidth = 5;
                GameSettings.Instance.panelHeight = 5;
                break;

            case 1:
                GameSettings.Instance.panelWidth = 7;
                GameSettings.Instance.panelHeight = 7;
                break;

            case 2:
                GameSettings.Instance.panelWidth = 9;
                GameSettings.Instance.panelHeight = 9;
                break;
        }


        // 爆弾の数を取得
        GameSettings.Instance.bombCount =
            bombCountDropdown.value + 1;


        // ターン数を取得
        int[] turnValues =
        {
            5,
            10,
            15,
            20,
            25,
            30,
            35,
            40,
            45,
            50
        };

        GameSettings.Instance.startTurn =
            turnValues[turnCountDropdown.value];


        // 小人の数を取得
        GameSettings.Instance.dwarfCount =
            dwarfCountDropdown.value + 1;


        // Consoleに設定内容を表示
        Debug.Log("========== ゲーム設定 ==========");
        Debug.Log(
            "パネルサイズ : " +
            GameSettings.Instance.panelWidth +
            " × " +
            GameSettings.Instance.panelHeight
        );

        Debug.Log(
            "爆弾の数 : " +
            GameSettings.Instance.bombCount
        );

        Debug.Log(
            "残りターン : " +
            GameSettings.Instance.startTurn
        );

        Debug.Log(
            "小人の数 : " +
            GameSettings.Instance.dwarfCount
        );

        Debug.Log("================================");


        // GameSceneへ移動
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
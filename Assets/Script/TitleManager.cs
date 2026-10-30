using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour
{
    [Header("タイトル画面")]
    public GameObject titlePanel;

    [Header("ゲームシーン名")]
    public string gameSceneName = "GameScene";

    [Header("HOW TO PLAY")]
    public GameObject howToPlayPanel;
    public GameObject page1;
    public GameObject page2;
    public GameObject page3;
    public GameObject page4;

    [Header("ゲーム設定")]
    public GameObject settingPanel;

    [Header("小人の移動設定")]
    public Toggle dwarfMoveToggle;

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

        // HOW TO PLAYを非表示
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }

        // 設定画面を表示
        if (settingPanel != null)
        {
            settingPanel.SetActive(true);
        }

        // 保存されている移動設定をToggleに反映
        if (GameSettings.Instance != null &&
            dwarfMoveToggle != null)
        {
            dwarfMoveToggle.isOn =
                GameSettings.Instance.dwarfsCanMove;
        }
    }

    // GAME STARTボタンを押した時
    public void StartGameWithSettings()
    {
        // GameSettingsが存在するか確認
        if (GameSettings.Instance == null)
        {
            Debug.LogError(
                "GameSettings.Instanceが見つかりません。"
            );
            return;
        }

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

            case 3:
                GameSettings.Instance.panelWidth = 11;
                GameSettings.Instance.panelHeight = 11;
                break;

            case 4:
                GameSettings.Instance.panelWidth = 13;
                GameSettings.Instance.panelHeight = 13;
                break;

            case 5:
                GameSettings.Instance.panelWidth = 15;
                GameSettings.Instance.panelHeight = 15;
                break;
        }

        // 爆弾の数を取得
        GameSettings.Instance.bombCount =
            bombCountDropdown.value + 1;

        // ターン数を取得
        int[] turnValues =
        {
            5, 10, 15, 20, 25,
            30, 35, 40, 45, 50
        };

        int turnIndex = Mathf.Clamp(
            turnCountDropdown.value,
            0,
            turnValues.Length - 1
        );

        GameSettings.Instance.startTurn =
            turnValues[turnIndex];

        // 小人の数を取得
        GameSettings.Instance.dwarfCount =
            dwarfCountDropdown.value + 1;

        // 小人の移動設定を保存
        if (dwarfMoveToggle != null)
        {
            GameSettings.Instance.dwarfsCanMove =
                dwarfMoveToggle.isOn;
        }

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

        Debug.Log(
            "小人の移動 : " +
            (GameSettings.Instance.dwarfsCanMove
                ? "ON"
                : "OFF")
        );

        Debug.Log("================================");

        // GameSceneへ移動
        SceneManager.LoadScene(gameSceneName);
    }

    // HOW TO PLAYを開いた時
    public void OpenHowToPlay()
    {
        Debug.Log("HOW TO PLAYを開きました");

        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }

        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(true);
        }

        if (page1 != null) page1.SetActive(true);
        if (page2 != null) page2.SetActive(false);
        if (page3 != null) page3.SetActive(false);
        if (page4 != null) page4.SetActive(false);

        GameObject titlePanel =
            GameObject.Find("TitlePanel");

        if (titlePanel != null)
        {
            titlePanel.SetActive(false);
        }
    }

    public void CloseHowToPlay()
    {
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }

        GameObject titlePanel =
            GameObject.Find("TitlePanel");

        if (titlePanel != null)
        {
            titlePanel.SetActive(true);
        }
    }

    // Page1 → Page2
    public void NextPage2()
    {
        page1.SetActive(false);
        page2.SetActive(true);
    }

    // Page2 → Page3
    public void NextPage3()
    {
        page2.SetActive(false);
        page3.SetActive(true);
    }

    // Page3 → Page4
    public void NextPage4()
    {
        page3.SetActive(false);
        page4.SetActive(true);
    }
    
    public void BackTitle()
    {
        Debug.Log("タイトルに戻る処理を開始");

        if (titlePanel == null)
        {
            Debug.LogError("TitlePanelが登録されていません");
            return;
        }

        // タイトル画面を表示
        titlePanel.SetActive(true);

        // 説明画面を閉じる
        if (howToPlayPanel != null)
        {
            howToPlayPanel.SetActive(false);
        }

        // 設定画面を閉じる
        if (settingPanel != null)
        {
            settingPanel.SetActive(false);
        }

        Debug.Log("TitlePanel activeSelf = " + titlePanel.activeSelf);
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

    // Page4 → Page3
    public void BackPage3()
    {
        page3.SetActive(true);
        page4.SetActive(false);
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
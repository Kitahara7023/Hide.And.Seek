using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("ゲーム設定")]
    public int startTurn = 15;

    [Header("現在の状態")]
    public int currentTurn;

    [Header("盤面管理")]
    [SerializeField] private GridManager gridManager;

    [Header("クリア時に非表示にするボタン")]
    public GameObject bombButton;
    public GameObject hintButton;

    public bool isGameClear;
    public bool isGameOver;

    // 見つけた小人の数
    public int foundDwarfCount = 0;

    // 必要な小人の数
    public int requiredDwarfCount = 1;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        isGameClear = false;
        isGameOver = false;

        // GameSettingsからターン数を取得

        if (GameSettings.Instance != null)
        {
            startTurn = GameSettings.Instance.startTurn;

            requiredDwarfCount =
                GameSettings.Instance.dwarfCount;

            Debug.Log(
                "GameSettingsからターン数を取得 : "
                + startTurn
            );

            Debug.Log(
                "必要な小人の数 : "
                + requiredDwarfCount
            );
        }
        else
        {
            Debug.LogWarning(
                "GameSettings.Instanceが見つかりません。"
                + "Inspectorの設定値を使用します。"
            );
        }

        // 最初のターン数を設定
        currentTurn = startTurn;

        // UIにターン数を表示
        UIManager.Instance.UpdateTurn(currentTurn);

        // 小人の発見数を初期化
        foundDwarfCount = 0;

        Debug.Log("ゲーム開始");
        Debug.Log("残りターン : " + currentTurn);
    }

    // パネルを割った時に呼ぶ    ターンを1消費する
    // allowAfterClearは、最後のターンで小人を発見した場合に使用する
    public void UseTurn(bool allowAfterClear = false)
    {
        // ゲームオーバー後は何もしない
        if (isGameOver)
            return;

        // クリア後の通常操作は受け付けない
        if (isGameClear && !allowAfterClear)
            return;

        currentTurn--;

        if (currentTurn < 0)
            currentTurn = 0;

        UIManager.Instance.UpdateTurn(currentTurn);

        Debug.Log("残りターン : " + currentTurn);

        if (currentTurn <= 0)
        {
            currentTurn = 0;

            UIManager.Instance.UpdateTurn(currentTurn);

            GameOver();
        }
    }

    public void FoundDwarf()
    {
        if (isGameClear || isGameOver)
            return;

        foundDwarfCount++;

        Debug.Log(
            "小人を発見！ " +
            foundDwarfCount +
            " / " +
            requiredDwarfCount
        );

        UIManager.Instance.UpdateDwarfCount(
        foundDwarfCount,
        requiredDwarfCount
    );


        // 全員見つけたか確認
        if (foundDwarfCount >= requiredDwarfCount)
        {
            GameClear();
        }
    }

    // 小人を見つけた時   ゲームクリア
    public void GameClear()
    {
        // すでにゲームオーバーまたはクリアなら何もしない
        if (isGameClear || isGameOver)
            return;

        // クリア状態にする
        isGameClear = true;

        // 爆弾設置ボタンとヒントボタンを非表示
        HideActionButtons();

        Debug.Log("================================");
        Debug.Log("GAME CLEAR");
        Debug.Log("================================");

        // クリアUIを表示
        UIManager.Instance.ShowClear();
    }

    // ターンが0になった時  ゲームオーバー
    public void GameOver()
    {
        // すでにクリアまたはゲームオーバーなら何もしない
        if (isGameClear || isGameOver)
            return;

        // ゲームオーバー状態にする
        isGameOver = true;

        // 爆弾設置ボタンとヒントボタンを非表示
        HideActionButtons();

        Debug.Log("================================");
        Debug.Log("GAME OVER");
        Debug.Log("================================");

        // 小人の隠れていた場所を公開
        if (gridManager != null)
        {
            DwarfManager.Instance.RevealAllDwarfs();
        }
        else
        {
            Debug.LogError("GridManagerが設定されていません！");
        }

        // ゲームオーバーUIを表示
        UIManager.Instance.ShowGameOver();
    }

    // ゲームクリア時にボタンを非表示にする
    public void HideActionButtons()
    {
        if (bombButton != null)
            bombButton.SetActive(false);

        if (hintButton != null)
            hintButton.SetActive(false);

        Debug.Log("爆弾設置ボタンとヒントボタンを非表示にしました");
    }

    // リトライ
    public void Retry()
    {
        Debug.Log("リトライします");

        // 現在のシーンをもう一度読み込む
        SceneManager.LoadScene(
            SceneManager.GetActiveScene().name
        );
    }

    public void Title()
    {
        SceneManager.LoadScene("Title");
    }
}

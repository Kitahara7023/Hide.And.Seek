using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("ゲーム設定")]
    public int startTurn = 15;

    [Header("現在の状態")]
    public int currentTurn;

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
    public void UseTurn()
    {
        // クリアまたはゲームオーバー後は何もしない
        if (isGameClear || isGameOver)
            return;

        // ターンを1減らす
        currentTurn--;

        // 0未満にならないようにする
        if (currentTurn < 0)
        {
            currentTurn = 0;
        }

        // UI更新
        UIManager.Instance.UpdateTurn(currentTurn);

        Debug.Log("残りターン : " + currentTurn);

        // ターンが0以下になったらゲームオーバー
        if (currentTurn <= 0)
        {
            currentTurn = 0;

            // UI上も0にする
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

        Debug.Log("================================");
        Debug.Log("GAME OVER");
        Debug.Log("================================");

        // ゲームオーバーUIを表示
        UIManager.Instance.ShowGameOver();
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

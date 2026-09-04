using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("ゲーム設定")]
    public int startTurn = 15;

    [Header("現在の状態")]
    public int currentTurn;

    public bool isGameClear;
    public bool isGameOver;

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
        // ゲーム開始時に状態を初期化
        isGameClear = false;
        isGameOver = false;

        // ターン数を初期化
        currentTurn = startTurn;

        // UIにターン数を表示
        UIManager.Instance.UpdateTurn(currentTurn);

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

    // 小人を見つけた時   ゲームクリア
    public void GameClear()
    {
        // すでにゲームオーバーまたはクリアなら何もしない
        if (isGameClear || isGameOver)
            return;

        // クリア状態にする
        isGameClear = true;

        Debug.Log("GAME CLEAR");

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

        Debug.Log("GAME OVER");

        // ゲームオーバーUIを表示
        UIManager.Instance.ShowGameOver();
    }
}

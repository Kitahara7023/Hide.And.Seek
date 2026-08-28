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
        currentTurn = startTurn;

        // 最初のターン数を表示
        UIManager.Instance.UpdateTurn(currentTurn);
    }

    // パネルを割った時に呼ぶ
    public void UseTurn()
    {
        if (isGameClear || isGameOver)
            return;

        currentTurn--;

        // UI更新
        UIManager.Instance.UpdateTurn(currentTurn);

        Debug.Log("残りターン : " + currentTurn);

        if (currentTurn <= 0)
        {
            GameOver();
        }
    }

    // 小人を見つけた時
    public void GameClear()
    {
        if (isGameOver)
            return;

        isGameClear = true;

        UIManager.Instance.ShowClear();

        Debug.Log("GAME CLEAR");
    }

    // ターンが0になった時
    public void GameOver()
    {
        if (isGameClear)
            return;

        isGameOver = true;

        UIManager.Instance.ShowGameOver();

        Debug.Log("GAME OVER");
    }
}

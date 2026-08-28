using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // シングルトン
    public static UIManager Instance;

    [Header("テキスト")]
    // 残りターン表示
    public TMP_Text turnText;

    // ゲームクリア表示
    public GameObject clearPanel;

    // ゲームオーバー表示
    public GameObject gameOverPanel;

    void Awake()
    {
        // シングルトン化
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
        // 開始時はパネルを非表示
        clearPanel.SetActive(false);
        gameOverPanel.SetActive(false);

        // ターン数を表示
        UpdateTurn(GameManager.Instance.currentTurn);
    }

    
    /// ターン数を更新する
    
    public void UpdateTurn(int turn)
    {
        turnText.text = "TURN : " + turn;
    }

   
    /// ゲームクリア表示
    
    public void ShowClear()
    {
        clearPanel.SetActive(true);
    }

    
    /// ゲームオーバー表示
    
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}

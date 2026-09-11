using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    // シングルトン
    public static UIManager Instance;

    [Header("テキスト")]
    // 残りターン表示
    public TMP_Text turnText;

    // 小人の発見数を表示するテキスト
    public TMP_Text dwarfText;

    // クリア時の残りターン表示
    public TMP_Text turnResultText;

    [Header("パネル")]
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

        // 小人の発見数を表示
        UpdateDwarfCount(
            GameManager.Instance.foundDwarfCount,
            GameManager.Instance.requiredDwarfCount
        );
    }

    
    /// ターン数を更新する
    
    public void UpdateTurn(int turn)
    {
        turnText.text = "TURN : " + turn;
    }

    // 小人の発見数を更新
    public void UpdateDwarfCount(int found,int required)
    {
        dwarfText.text ="小人 : " +found +" / " +required;
    }

    /// ゲームクリア表示

    public void ShowClear()
    {
        clearPanel.SetActive(true);

        // クリア時の残りターンを表示
        turnResultText.text =
            "残りターン : " + GameManager.Instance.currentTurn;
    }

    
    /// ゲームオーバー表示
    
    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
    }
}

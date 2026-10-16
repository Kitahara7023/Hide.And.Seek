using UnityEngine;

public class GameSettings : MonoBehaviour
{
    // ゲーム設定をどこからでも取得できるようにする
    public static GameSettings Instance;

    [Header("ゲーム設定")]

    [Header("パネルの横幅")] 
    public int panelWidth = 7;

    [Header("パネルの縦幅")]
    public int panelHeight = 7;

    [Header("爆弾の数")]// 
    public int bombCount = 5;

    [Header("残りターン")] 
    public int startTurn = 15;

    [Header("小人の数")]
    public int dwarfCount = 1;

    [Header("小人の移動設定")]
    public bool dwarfsCanMove = false;

    private void Awake()
    {
        // GameSettingsを1つだけ残す
        if (Instance == null)
        {
            Instance = this;

            // シーンを移動しても残す
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}

using UnityEngine;

public class GameSettings : MonoBehaviour
{
    // ゲーム設定をどこからでも取得できるようにする
    public static GameSettings Instance;

    [Header("ゲーム設定")]
    // パネルの横幅
    public int panelWidth = 7;

    // パネルの縦幅
    public int panelHeight = 7;

    // 爆弾の数
    public int bombCount = 5;

    // 残りターン
    public int startTurn = 15;

    // 小人の数
    public int dwarfCount = 1;

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

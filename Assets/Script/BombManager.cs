using UnityEngine;

public class BombManager : MonoBehaviour
{

    public static BombManager Instance;

    [Header("GridManager")]
    public GridManager gridManager;

    [Header("Bomb Prefab")]
    public Bomb bombPrefab;

    [Header("爆弾の数")]
    public int bombCount = 3;

    private void Awake()
    {
        Instance = this;
    }

    // 爆弾を複数配置
    public void SpawnBomb()
    {
        Debug.Log("爆弾配置開始");

        Panel[,] panels = gridManager.GetPanels();

        int spawnedCount = 0;

        // 指定した数だけ爆弾を配置する
        for (int i = 0; i < bombCount; i++)
        {
            bool spawned = false;

            // 1個につき最大100回探す
            for (int j = 0; j < 100; j++)
            {
                int x = Random.Range(0, gridManager.Width);
                int y = Random.Range(0, gridManager.Height);

                Panel panel = panels[x, y];

                if (panel == null)
                    continue;

                // 小人がいるパネルには置かない
                if (panel.dwarf != null)
                    continue;

                // すでに爆弾があるパネルには置かない
                if (panel.hasBomb)
                    continue;

                // 爆弾を生成
                Bomb bomb = Instantiate(bombPrefab);

                // パネルに配置
                bomb.SetPanel(panel);

                Debug.Log($"爆弾配置 ({x},{y})");

                spawned = true;
                spawnedCount++;

                break;
            }

            // 100回探しても配置できなかった
            if (!spawned)
            {
                Debug.LogError(
                    $"爆弾を配置できませんでした ({i + 1}個目)"
                );
            }
        }

        Debug.Log($"爆弾配置完了：{spawnedCount}/{bombCount}");
    }

    // 爆発
    public void Explode(Panel center)
    {
        Debug.Log(
            $"爆発! 中心 = ({center.x},{center.y})"
        );

        Panel[,] panels = gridManager.GetPanels();

        // 周囲3×3を調べる
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = center.x + dx;
                int ny = center.y + dy;

                // 盤面外なら無視
                if (nx < 0 || nx >= gridManager.Width)
                    continue;

                if (ny < 0 || ny >= gridManager.Height)
                    continue;

                Panel panel = panels[nx, ny];

                if (panel == null)
                    continue;

                // すでに壊れていたら無視
                if (panel.IsBroken())
                    continue;

                Debug.Log(
                    $"爆発範囲 ({nx},{ny})"
                );

                panel.BreakPanel();
            }
        }
    }
}


using UnityEngine;

public class BombManager : MonoBehaviour
{
    public static BombManager Instance;

    [Header("GridManager")]
    public GridManager gridManager;

    [Header("Bomb Prefab")]
    public Bomb bombPrefab;

    private void Awake()
    {
        Instance = this;
    }

    // 最初から爆弾を配置

    public void SpawnBomb()
    {
        Panel[,] panels = gridManager.GetPanels();

        while (true)
        {
            int x = Random.Range(0, gridManager.Width);
            int y = Random.Range(0, gridManager.Height);

            Panel panel = panels[x, y];

            if (panel == null)
                continue;

            // 小人がいる場所には置かない
            if (panel.dwarf != null)
                continue;

            // すでに爆弾がある場所には置かない
            if (panel.hasBomb)
                continue;

            // 最初から配置する爆弾
            Bomb bomb = Instantiate(bombPrefab);

            // false → 見えない
            bomb.SetPanel(panel, false);

            Debug.Log($"爆弾配置 ({x},{y})");

            return;
        }
    }

    // 指定したパネルに爆弾を置く

    public void PlaceBomb(Panel panel)
    {
        if (panel == null)
            return;

        if (panel.hasBomb)
            return;

        // 爆弾を生成
        Bomb bomb = Instantiate(bombPrefab);

        // true → 見える
        bomb.SetPanel(panel, true);

        Debug.Log(
            $"爆弾を設置 ({panel.x},{panel.y})"
        );
    }

    // 爆発

    public void Explode(Panel center)
    {
        Debug.Log(
        $"爆発開始！ 中心 ({center.x},{center.y})"
        );

        // 中心パネルにある爆弾を爆発させる
        if (center.bomb != null)
        {
            center.bomb.Explode();
        }

        Panel[,] panels = gridManager.GetPanels();

        // 3×3の範囲
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = center.x + dx;
                int ny = center.y + dy;

                // 盤面外
                if (nx < 0 || nx >= gridManager.Width)
                    continue;

                if (ny < 0 || ny >= gridManager.Height)
                    continue;

                Panel panel = panels[nx, ny];

                if (panel == null)
                    continue;

                // すでに壊れている場合
                if (panel.IsBroken())
                    continue;

                Debug.Log(
                    $"爆発で破壊 ({nx},{ny})"
                );

                // 爆発による破壊なのでターンを減らさない
                panel.BreakPanel(false);
            }
        }

        Debug.Log("爆発終了");
    }
}


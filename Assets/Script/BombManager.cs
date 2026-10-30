using System.Collections.Generic;
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
        if (gridManager == null || bombPrefab == null)
        {
            Debug.LogError("GridManagerまたはBomb Prefabが未設定です");
            return;
        }

        Panel[,] panels = gridManager.GetPanels();

        if (panels == null)
            return;

        List<Panel> candidates = new List<Panel>();

        // 配置可能なパネルを取得
        for (int x = 0; x < gridManager.Width; x++)
        {
            for (int y = 0; y < gridManager.Height; y++)
            {
                Panel panel = panels[x, y];

                // 小人や爆弾があっても配置可能
                if (panel != null && !panel.IsBroken())
                {
                    candidates.Add(panel);
                }
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogError("爆弾を配置できるパネルがありません");
            return;
        }

        Panel selected =
            candidates[Random.Range(0, candidates.Count)];

        Bomb bomb = Instantiate(bombPrefab);

        bomb.SetPanel(selected, false);

        Debug.Log($"爆弾配置 ({selected.x},{selected.y})");
    }

    // 指定したパネルに爆弾を置く
    public void PlaceBomb(Panel panel)
    {
        if (panel == null || bombPrefab == null)
            return;

        if (panel.IsBroken())
            return;

        // 爆弾があっても小人がいても設置可能
        Bomb bomb = Instantiate(bombPrefab);

        bomb.SetPanel(panel, true);

        Debug.Log($"爆弾を設置 ({panel.x},{panel.y})");
    }

    // 爆発
    public void Explode(Panel center)
    {
        if (center == null || gridManager == null)
            return;

        Debug.Log(
            $"爆発開始！ 中心 ({center.x},{center.y})"
        );

        // 中心パネルにあるすべての爆弾を処理
        List<Bomb> centerBombs =
            new List<Bomb>(center.bombs);

        foreach (Bomb bomb in centerBombs)
        {
            if (bomb != null)
            {
                bomb.Explode();
            }
        }

        Panel[,] panels = gridManager.GetPanels();

        if (panels == null)
            return;

        // 中心から上下左右1マスずつ、合計3×3を破壊
        for (int x = center.x - 1; x <= center.x + 1; x++)
        {
            for (int y = center.y - 1; y <= center.y + 1; y++)
            {
                // 盤面の外なら無視
                if (x < 0 || x >= gridManager.Width ||
                    y < 0 || y >= gridManager.Height)
                {
                    continue;
                }

                Panel panel = panels[x, y];

                if (panel == null || panel.IsBroken())
                    continue;

                // 爆発の中心かどうか
                bool isCenter = panel == center;

                // 爆発による破壊なのでターンは消費しない
                panel.BreakPanel(false, isCenter);
            }
        }

        Debug.Log("爆発終了");
    }
}


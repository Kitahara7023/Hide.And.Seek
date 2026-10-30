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
        Panel[,] panels = gridManager.GetPanels();

        List<Panel> candidates = new List<Panel>();

        // 配置可能なパネルを取得
        for (int x = 0; x < gridManager.Width; x++)
        {
            for (int y = 0; y < gridManager.Height; y++)
            {
                Panel panel = panels[x, y];

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
        if (panel == null)
            return;

        if (panel.IsBroken())
            return;

        // 爆弾があっても小人がいても設置可能
        Bomb bomb = Instantiate(bombPrefab);

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

        // 中心パネルにあるすべての爆弾を処理
        foreach (Bomb bomb in center.bombs.ToArray())
        {
            if (bomb != null)
            {
                bomb.Explode();
            }
        }

        Panel[,] panels = gridManager.GetPanels();

        // 3×3の範囲
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int nx = center.x + dx;
                int ny = center.y + dy;

                if (nx < 0 || nx >= gridManager.Width)
                    continue;

                if (ny < 0 || ny >= gridManager.Height)
                    continue;

                Panel panel = panels[nx, ny];

                if (panel == null)
                    continue;

                if (panel.IsBroken())
                    continue;

                Debug.Log($"爆発で破壊 ({nx},{ny})");

                panel.BreakPanel(false);
            }
        }

        Debug.Log("爆発終了");
    }
}


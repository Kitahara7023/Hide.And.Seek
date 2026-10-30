using UnityEngine;
using System.Collections.Generic;

public class DwarfManager : MonoBehaviour
{
    public static DwarfManager Instance;

    [Header("盤面")]
    public GridManager gridManager;

    [Header("小人のPrefab")]
    public Dwarf dwarfPrefab;

    [Header("生成した小人")]
    public List<Dwarf> dwarfs = new List<Dwarf>();

    private void Awake()
    {
        Instance = this;
    }

    // 小人を生成する
    public void SpawnDwarf(DwarfType type)
    {
        if (gridManager == null || dwarfPrefab == null)
        {
            Debug.LogError(
                "DwarfManagerのGridManagerまたはPrefabが未設定です。");
            return;
        }

        Panel[,] panels = gridManager.GetPanels();

        if (panels == null)
        {
            return;
        }

        List<Panel> candidates = new List<Panel>();

        // 小人を配置できるパネルを探す
        for (int x = 0; x < panels.GetLength(0); x++)
        {
            for (int y = 0; y < panels.GetLength(1); y++)
            {
                Panel panel = panels[x, y];

                if (CanMoveTo(panel))
                {
                    candidates.Add(panel);
                }
            }
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("小人を配置できるパネルがありません。");
            return;
        }

        // 候補からランダムに選ぶ
        Panel selectedPanel =
            candidates[Random.Range(0, candidates.Count)];

        // ゲーム設定に合わせて移動タイプを決定
        if (GameSettings.Instance != null)
        {
            if (GameSettings.Instance.dwarfsCanMove)
            {
                type = DwarfType.Move;
            }
            else
            {
                type = DwarfType.Stay;
            }
        }

        // 小人を生成
        Dwarf dwarf = Instantiate(dwarfPrefab);

        dwarf.dwarfType = type;
        dwarf.SetPanel(selectedPanel);

        dwarfs.Add(dwarf);

        Debug.Log("生成した小人のタイプ：" + dwarf.dwarfType);
    }

    // 指定パネルに移動できるか確認する
    private bool CanMoveTo(Panel panel)
    {
        if (panel == null)
        {
            return false;
        }

        // 壊れたパネルには移動できない
        if (panel.IsBroken())
        {
            return false;
        }

        // ほかの小人がいるパネルには移動できない
        if (panel.dwarf != null)
        {
            return false;
        }

        // 爆弾があっても移動できる

        return true;
    }

    // Moveタイプの小人を1マス移動させる
    public void MoveDwarfs()
    {
        // 設定で小人の移動が禁止されている場合
        if (GameSettings.Instance != null &&
            !GameSettings.Instance.dwarfsCanMove)
        {
            return;
        }

        // ゲーム終了後は移動しない
        if (GameManager.Instance != null &&
            (GameManager.Instance.isGameOver ||
             GameManager.Instance.isGameClear))
        {
            return;
        }

        if (gridManager == null)
        {
            return;
        }

        Panel[,] panels = gridManager.GetPanels();

        if (panels == null)
        {
            return;
        }

        int width = panels.GetLength(0);
        int height = panels.GetLength(1);

        // 上・下・左・右
        Vector2Int[] directions =
        {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
        };

        // すべての小人を確認
        foreach (Dwarf dwarf in dwarfs)
        {
            if (dwarf == null || dwarf.isFound)
            {
                continue;
            }

            // Stayタイプは移動しない
            if (dwarf.dwarfType != DwarfType.Move)
            {
                continue;
            }

            Panel current = dwarf.currentPanel;

            if (current == null)
            {
                continue;
            }

            List<Panel> candidates = new List<Panel>();

            // 上下左右の隣のパネルを調べる
            foreach (Vector2Int direction in directions)
            {
                int nextX = current.x + direction.x;
                int nextY = current.y + direction.y;

                // 盤面の外なら無視
                if (nextX < 0 || nextX >= width ||
                    nextY < 0 || nextY >= height)
                {
                    continue;
                }

                Panel nextPanel = panels[nextX, nextY];

                // 移動可能なパネルだけ候補に入れる
                if (CanMoveTo(nextPanel))
                {
                    candidates.Add(nextPanel);
                }
            }

            // 移動先がなければ、その場にとどまる
            if (candidates.Count == 0)
            {
                continue;
            }

            // 隣接パネルからランダムに選択
            Panel destination =
                candidates[Random.Range(0, candidates.Count)];

            // 小人を移動
            dwarf.SetPanel(destination);
        }
    }
}

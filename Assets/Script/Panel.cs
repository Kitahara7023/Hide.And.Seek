using UnityEngine;
using System.Collections.Generic;
public class Panel : MonoBehaviour
{
    private bool isBroken = false;

    public Dwarf dwarf;

    public int x;
    public int y;

    public bool hasBomb = false;
    public bool hasItem = false;

    // このパネルにある爆弾を複数管理
    public List<Bomb> bombs = new List<Bomb>();

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public bool IsBroken()
    {
        return isBroken;
    }

    // 爆弾を登録
    public void AddBomb(Bomb bomb)
    {
        if (bomb == null)
            return;

        if (!bombs.Contains(bomb))
        {
            bombs.Add(bomb);
        }

        hasBomb = bombs.Count > 0;

        // 登録した爆弾の位置を更新
        UpdateBombPositions();
    }

    // 爆弾を解除
    public void RemoveBomb(Bomb bomb)
    {
        if (bomb == null)
            return;

        bombs.Remove(bomb);

        hasBomb = bombs.Count > 0;

        UpdateBombPositions();
    }

    // 複数の爆弾の表示位置を調整
    public void UpdateBombPositions()
    {
        // nullを除去
        bombs.RemoveAll(b => b == null);

        hasBomb = bombs.Count > 0;

        for (int i = 0; i < bombs.Count; i++)
        {
            Bomb bomb = bombs[i];

            // すべての爆弾をパネル中央に配置
            bomb.SetVisualPosition(
                transform.position,
                i
            );
        }
    }

    public void BreakPanel()
    {
        BreakPanel(true);
    }

    public void BreakPanel(bool useTurn)
    {
        // すでに壊れているなら何もしない
        if (isBroken)
            return;

        isBroken = true;

        Debug.Log($"Break ({x},{y})");

        // 直接クリックされた場合だけターンを減らす
        if (useTurn)
        {
            Debug.Log("通常クリック → ターンを1減らす");

            if (GameManager.Instance != null)
            {
                GameManager.Instance.UseTurn();
            }
        }
        else
        {
            Debug.Log("爆発による破壊 → ターンを減らさない");
        }

        // 小人
        if (dwarf != null)
        {
            Debug.Log($"小人発見！ Panel ({x},{y})");

            // 爆発によって壊された場合だけ位置をずらす
            if (!useTurn)
            {
                dwarf.MoveAside();
            }

            SpriteRenderer dwarfRenderer =
                dwarf.GetComponent<SpriteRenderer>();

            if (dwarfRenderer != null)
            {
                dwarfRenderer.sortingOrder = 1;
            }

            dwarf.Found();
        }

        // 爆弾
        if (bombs.Count > 0)
        {
            Debug.Log($"爆弾発見！ Panel ({x},{y})");

            BombManager.Instance.Explode(this);
        }

        // パネルを消す
        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (col != null)
            col.enabled = false;
    }
}

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
            bombs.Add(bomb);

        hasBomb = bombs.Count > 0;

        // 爆弾が置かれたら小人の位置を更新
        if (dwarf != null)
        {
            dwarf.UpdatePositionOnPanel();
        }

        UpdateBombPositions();
    }

    // 爆弾を解除
    public void RemoveBomb(Bomb bomb)
    {
        if (bomb == null)
            return;

        bombs.Remove(bomb);

        hasBomb = bombs.Count > 0;

        // 爆弾がなくなった場合、小人の位置も更新
        if (dwarf != null)
        {
            dwarf.UpdatePositionOnPanel();
        }

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
        BreakPanel(true, false);
    }

    public void BreakPanel(bool useTurn)
    {
        BreakPanel(useTurn, false);
    }

    public void BreakPanel(bool useTurn, bool moveDwarfAside)
    {
        if (isBroken)
            return;

        isBroken = true;

        Debug.Log($"Break ({x},{y})");

        // このパネルに小人がいるか確認
        bool hasDwarf = dwarf != null;

        // 小人がいる場合
        if (hasDwarf)
        {
            Debug.Log($"小人発見！ Panel ({x},{y})");

            // 爆発の中心にいる場合だけ位置をずらす
            if (moveDwarfAside)
            {
                dwarf.MoveAside();
            }

            SpriteRenderer dwarfRenderer =
                dwarf.GetComponent<SpriteRenderer>();

            if (dwarfRenderer != null)
                dwarfRenderer.sortingOrder = 1;

            // 小人を発見
            dwarf.Found();
        }

        // 爆弾がある場合は先に爆発させる
        if (bombs.Count > 0)
        {
            Debug.Log($"爆弾発見！ Panel ({x},{y})");

            BombManager.Instance.Explode(this);
        }

        // 爆発範囲の小人の発見処理が終わってからターンを消費
        if (useTurn)
        {
            if (GameManager.Instance != null)
            {
                bool clearByExplosion =
                    GameManager.Instance.isGameClear;

                GameManager.Instance.UseTurn(clearByExplosion);
            }
        }

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (col != null)
            col.enabled = false;
    }
}

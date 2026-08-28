using UnityEngine;

public class Panel : MonoBehaviour
{
    private bool isBroken = false;

    public Dwarf dwarf;

    public int x;
    public int y;

    public bool hasBomb = false;
    public bool hasItem = false;

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

        Debug.Log($"Panel ({x},{y}) Position = {transform.position}");

        Debug.Log($"Break ({x},{y})");

        // 直接クリックされた場合だけターンを減らす
        if (useTurn)
        {
            GameManager.Instance.UseTurn();
        }

        // 小人

        if (dwarf != null)
        {
            Debug.Log($"小人発見！ Panel ({x},{y})");

            SpriteRenderer dwarfRenderer =
                dwarf.GetComponent<SpriteRenderer>();

            if (dwarfRenderer != null)
            {
                dwarfRenderer.sortingOrder = 1;
            }

            dwarf.Found();
        }


        // 爆弾

        if (hasBomb)
        {
            Debug.Log($"爆弾発見！ Panel ({x},{y})");

            // 爆弾があることを解除
            hasBomb = false;

            // 爆発
            BombManager.Instance.Explode(this);
        }


        // パネルを消す

        spriteRenderer.enabled = false;
        col.enabled = false;
    }
}

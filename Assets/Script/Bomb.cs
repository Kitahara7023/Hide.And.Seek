using UnityEngine;

public class Bomb : MonoBehaviour
{
    // 今いるパネル
    public Panel currentPanel;

    // すでに爆発したか
    private bool exploded = false;

    public void SetPanel(Panel panel)
    {
        currentPanel = panel;

        // パネルに爆弾があることを登録
        panel.hasBomb = true;

        // パネルの位置に移動
        transform.position = panel.transform.position;

        // パネルより下に表示
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr != null)
        {
            sr.sortingOrder = -1;
        }

        Debug.Log(
            $"Bomb Position = {transform.position}"
        );
    }

    public void Explode()
    {
        // すでに爆発していたら何もしない
        if (exploded)
            return;

        exploded = true;

        Debug.Log("爆弾が爆発！");

        // 爆弾を消す
        Destroy(gameObject);
    }
}

using UnityEngine;

public class Bomb : MonoBehaviour
{
    // 今いるパネル
    public Panel currentPanel;

    public void SetPanel(Panel panel)
    {
        currentPanel = panel;

        // パネルに爆弾があることを登録
        panel.hasBomb = true;

        // パネルの位置に移動
        transform.position = panel.transform.position;

        // パネルより下に表示
        SpriteRenderer renderer =
            GetComponent<SpriteRenderer>();

        if (renderer != null)
        {
            renderer.sortingOrder = -1;
        }

        Debug.Log(
            $"Bomb Position = {transform.position}"
        );
    }

    public void Explode()
    {
        Debug.Log("爆弾発見！");

        Destroy(gameObject);
    }
}

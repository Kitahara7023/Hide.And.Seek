using UnityEngine;

public class Bomb : MonoBehaviour
{
    // 今いるパネル
    public Panel currentPanel;

    // 最初から隠されていた爆弾かどうか
    private bool isHiddenBomb;

    public void SetPanel( Panel panel, bool visible = false)
    {
        currentPanel = panel;

        // パネルに爆弾があることを設定
        panel.hasBomb = true;

        // このパネルにあるBomb自身を登録する
        panel.bomb = this;

        // パネルの位置に移動
        transform.position = panel.transform.position;

        // SpriteRendererを取得
        SpriteRenderer spriteRenderer =
            GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("BombにSpriteRendererがありません！");
            return;
        }

        // 隠し爆弾かどうかを記録
        isHiddenBomb = !visible;

        if (visible)
        {
            // 自分で設置した爆弾
            spriteRenderer.enabled = true;
            spriteRenderer.sortingOrder = 10;

            Debug.Log("自分で設置した爆弾を表示します");
        }
        else
        {
            // 最初から配置されている隠し爆弾
            spriteRenderer.enabled = false;

            Debug.Log("隠し爆弾を非表示");
        }
    }

    // 爆発
    public void Explode()
    {
        Debug.Log("爆弾が爆発！");

        SpriteRenderer spriteRenderer =
            GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("BombにSpriteRendererがありません！");
            return;
        }

        if (isHiddenBomb)
        {
            // 最初から隠されていた爆弾
            spriteRenderer.enabled = true;

            // パネルより手前に表示
            spriteRenderer.sortingOrder = 10;

            Debug.Log("隠されていた爆弾が姿を現した！");
        }
        else
        {
            
            // 自分で設置した爆弾
            // 何もしない
            // すでに表示されているので、
            // そのまま表示しておく

            Debug.Log("自分で設置した爆弾が爆発！");
        }
    }
}

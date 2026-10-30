using UnityEngine;

/// 小人の種類

public enum DwarfType
{
    Stay,   // 動かない小人
    Move    // 動く小人
}

public class Dwarf : MonoBehaviour
{
    [Header("小人の設定")]
    public DwarfType dwarfType;

    [Header("現在いるパネル")]
    public Panel currentPanel;

    [Header("発見されたか")]
    public bool isFound = false;

    // 小人を指定したパネルに配置する
    public void SetPanel(Panel panel)
    {
        if (panel == null)
        {
            return;
        }

        // 移動前のパネルから小人の登録を解除
        if (currentPanel != null &&
            currentPanel.dwarf == this)
        {
            currentPanel.dwarf = null;
        }

        // 新しいパネルを現在地にする
        currentPanel = panel;

        // 新しいパネルに小人を登録
        currentPanel.dwarf = this;

        // 通常時はパネル中央
        transform.position = panel.transform.position;
    }

    // 小人が発見されたときの処理
    public void Found()
    {
        if (isFound)
        {
            return;
        }

        isFound = true;

        Debug.Log("小人を発見！");

        // この小人のヒント枠を削除
        HintManager hintManager =
            FindFirstObjectByType<HintManager>();

        if (hintManager != null)
        {
            hintManager.RemoveHintFrame(this);
        }

        // 発見数を更新
        if (GameManager.Instance != null)
        {
            GameManager.Instance.FoundDwarf();
        }
    }

    public void UpdatePositionOnPanel()
    {
        if (currentPanel == null)
            return;

        if (currentPanel.hasBomb)
        {
            // 爆弾から少し顔を出す
            transform.position =
                currentPanel.transform.position +
                new Vector3(-0.22f, 0f, 0f);
        }
        else
        {
            // 爆弾がなければ中央に戻る
            transform.position =
                currentPanel.transform.position;
        }
    }

    // Sceneビューで小人の位置を確認する
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

    // 爆発したときだけ小人の位置をずらす
    public void MoveAside()
    {
        if (currentPanel == null)
            return;

        transform.position =
            currentPanel.transform.position +
            new Vector3(-0.22f, 0f, 0f);
    }
}

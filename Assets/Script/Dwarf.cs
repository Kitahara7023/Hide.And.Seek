using UnityEngine;

/// 小人の種類

public enum DwarfType
{
    Stay,   // 動かない小人
    Move    // 動く小人
}

public class Dwarf : MonoBehaviour
{
    [Header("小人の種類")]
    public DwarfType dwarfType;

    // 現在いるパネル
    public Panel currentPanel;

    
    /// 小人を配置する
    
    public void SetPanel(Panel panel)
    {
        currentPanel = panel;
        currentPanel.dwarf = this;

        Debug.Log("Panel Position = " + panel.transform.position);

        transform.position = panel.transform.position;

        Debug.Log("Dwarf Position = " + transform.position);

        Debug.Log("Parent = " + transform.parent);
    }

    /// この小人を見つけた
    
    public void Found()
    {
        Debug.Log("小人を発見！");

        GameManager.Instance.GameClear();

        //Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }

}

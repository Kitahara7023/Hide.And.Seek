using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Header("Œ»İ‚¢‚éƒpƒlƒ‹")]
    public Panel currentPanel;

    // true‚È‚ç‰B‚³‚ê‚½”š’e
    private bool isHiddenBomb;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // ”š’e‚ğƒpƒlƒ‹‚É”z’u‚·‚é
    // visible‚ªfalse‚È‚ç‰B‚³‚ê‚½”š’e
    // visible‚ªtrue‚È‚çÅ‰‚©‚çŒ©‚¦‚é”š’e
    public void SetPanel(Panel panel, bool visible = false)
    {
        if (panel == null)
            return;

        // ˆÈ‘O‚Ìƒpƒlƒ‹‚©‚ç“o˜^‚ğ‰ğœ
        if (currentPanel != null)
        {
            currentPanel.RemoveBomb(this);
        }

        currentPanel = panel;

        isHiddenBomb = !visible;

        // V‚µ‚¢ƒpƒlƒ‹‚É”š’e‚ğ“o˜^
        currentPanel.AddBomb(this);

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = visible;
        }
    }

    // ƒpƒlƒ‹ã‚Å”š’e‚Ì•\¦ˆÊ’u‚ğXV
    public void SetVisualPosition(Vector3 position, int index)
    {
        transform.position = position;

        if (spriteRenderer != null && !isHiddenBomb)
        {
            spriteRenderer.sortingOrder = 10 + index;
        }
    }

    // ”š”­
    public void Explode()
    {
        Debug.Log("”š’e‚ª”š”­I");

        if (spriteRenderer == null)
            return;

        if (isHiddenBomb)
        {
            spriteRenderer.enabled = true;
            spriteRenderer.sortingOrder = 10;

            isHiddenBomb = false;

            Debug.Log("‰B‚³‚ê‚Ä‚¢‚½”š’e‚ªp‚ğŒ»‚µ‚½I");
        }
        else
        {
            Debug.Log("©•ª‚Åİ’u‚µ‚½”š’e‚ª”š”­I");
        }
    }
}

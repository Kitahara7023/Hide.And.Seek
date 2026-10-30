using UnityEngine;

public class Bomb : MonoBehaviour
{
    public Panel currentPanel;

    private bool isHiddenBomb;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetPanel(Panel panel, bool visible = false)
    {
        if (panel == null)
            return;

        // ‘O‚Ìƒpƒlƒ‹‚©‚ç“o˜^‰ğœ
        if (currentPanel != null)
        {
            currentPanel.RemoveBomb(this);
        }

        currentPanel = panel;

        // V‚µ‚¢ƒpƒlƒ‹‚É“o˜^
        panel.AddBomb(this);

        isHiddenBomb = !visible;

        if (spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        if (spriteRenderer == null)
        {
            Debug.LogError("Bomb‚ÉSpriteRenderer‚ª‚ ‚è‚Ü‚¹‚ñI");
            return;
        }

        spriteRenderer.enabled = visible;

        if (visible)
        {
            spriteRenderer.sortingOrder = 10;
        }

        Debug.Log(
            visible ? "İ’u‚µ‚½”š’e‚ğ•\¦" : "‰B‚µ”š’e‚ğ”ñ•\¦"
        );
    }

    // Panel‚©‚çŒÄ‚Ño‚µ‚Ä•\¦ˆÊ’u‚ğ•ÏX
    public void SetVisualPosition(
    Vector3 position,
    int index)
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

            Debug.Log("‰B‚³‚ê‚Ä‚¢‚½”š’e‚ªp‚ğŒ»‚µ‚½I");
        }
        else
        {
            Debug.Log("©•ª‚Åİ’u‚µ‚½”š’e‚ª”š”­I");
        }
    }
}

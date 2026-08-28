using UnityEngine;
using UnityEngine.InputSystem;

public class GridManager : MonoBehaviour
{
    [Header("生成するパネル")]
    public GameObject panelPrefab;

    [Header("盤面サイズ")]
    public int width = 7;
    public int height = 7;

    [Header("パネル間隔")]
    public float spacing = 1.1f;

    private Panel[,] panels;
    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        Debug.Log("① Start");

        // パネル配列を作成
        panels = new Panel[width, height];

        // 盤面生成
        Debug.Log("② CreateGrid");
        CreateGrid();

        // 小人を1体生成
        Debug.Log("③ SpawnDwarf");
        DwarfManager.Instance.SpawnDwarf(DwarfType.Stay);

        Debug.Log("④ SpawnBomb");
        BombManager.Instance.SpawnBomb();

        Debug.Log("⑤ End");
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 screenPos = Mouse.current.position.ReadValue();
            screenPos.z = -mainCamera.transform.position.z;

            Vector3 worldPos = mainCamera.ScreenToWorldPoint(screenPos);
            worldPos.z = 0;

            Debug.Log(worldPos);

            Collider2D hit = Physics2D.OverlapPoint(worldPos);

            if (hit != null)
            {
                Debug.Log(hit.name);

                Debug.Log($"Hit Position = {hit.transform.position}");

                Panel panel = hit.GetComponent<Panel>();

                if (panel != null)
                {
                    Debug.Log(panel.transform.position);

                    panel.BreakPanel();
                }
            }
        }
    }

    void CreateGrid()
    {
        float startX = -(width - 1) * spacing / 2f;
        float startY = (height - 1) * spacing / 2f;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(
                    startX + x * spacing,
                    startY - y * spacing,
                    0);

                Debug.Log($"Panel ({x},{y}) = {pos}");

                GameObject obj = Instantiate(panelPrefab, pos, Quaternion.identity, transform);
                panels[x, y] = obj.GetComponent<Panel>();

                Panel panel = obj.GetComponent<Panel>();

                panel.x = x;
                panel.y = y;

                panels[x, y] = panel;
            }
        }

        Debug.Log(panels[0, 0]);
    }

    public Panel[,] GetPanels()
    {
        return panels;
    }

    public int Width => width;

    public int Height => height;
}

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

    [Header("爆弾の数")]
    public int bombCount = 5;

    private Panel[,] panels;
    private Camera mainCamera;

    // 爆弾設置モードかどうか
    private bool bombMode = false;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        // パネル配列を作成
        panels = new Panel[width, height];

        // 盤面生成
        CreateGrid();

        // 小人を1体生成
        DwarfManager.Instance.SpawnDwarf(DwarfType.Stay);

        // 爆弾を指定した数だけ配置
        for (int i = 0; i < bombCount; i++)
        {
            BombManager.Instance.SpawnBomb();
        }
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
                Panel panel = hit.GetComponent<Panel>();

                if (panel != null)
                {
                    // 爆弾設置モード
                    if (bombMode)
                    {
                        PlaceBomb(panel);

                        return;
                    }

                    // 通常モード
                    panel.BreakPanel();
                }
            }
        }
    }

    // 爆弾設置モードを開始

    public void StartBombMode()
    {
        bombMode = true;

        Debug.Log("爆弾設置モード開始");
    }

    // 爆弾を設置
    
    void PlaceBomb(Panel panel)
    {
        // すでに壊れているパネルには置けない
        if (panel.IsBroken())
        {
            Debug.Log("壊れたパネルには爆弾を置けません");

            return;
        }

        // 小人がいるパネルには置けない
        if (panel.dwarf != null)
        {
            Debug.Log("小人がいるパネルには爆弾を置けません");

            return;
        }

        // すでに爆弾がある
        if (panel.hasBomb)
        {
            Debug.Log("このパネルにはすでに爆弾があります");

            return;
        }

        // 爆弾を設置
        BombManager.Instance.PlaceBomb(panel);

        // 1ターン消費
        GameManager.Instance.UseTurn();

        Debug.Log($"爆弾を設置 ({panel.x},{panel.y})");

        // 通常モードに戻る
        bombMode = false;

        Debug.Log("爆弾設置モード終了");
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

                GameObject obj =
                    Instantiate(
                        panelPrefab,
                        pos,
                        Quaternion.identity,
                        transform

                    );


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

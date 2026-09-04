using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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

    [Header("爆弾設置ボタン")]
    public Button bombButton;

    private Panel[,] panels;
    private Camera mainCamera;

    // 爆弾設置モードかどうか
    private bool bombMode = false;

    // 爆弾設置後、何ターン経過したか
    private int bombCooldown = 0;

    // 爆弾を再び使えるまでのターン数
    private const int BombCooldownMax = 3;

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

        // 最初は爆弾設置可能
        bombCooldown = BombCooldownMax;

        // ボタンの表示を更新
        UpdateBombButton();
    }

    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 screenPos =
                Mouse.current.position.ReadValue();

            screenPos.z =
                -mainCamera.transform.position.z;

            Vector3 worldPos =
                mainCamera.ScreenToWorldPoint(screenPos);

            worldPos.z = 0;

            Debug.Log(worldPos);

            Collider2D hit =
                Physics2D.OverlapPoint(worldPos);

            if (hit != null)
            {
                Panel panel =
                    hit.GetComponent<Panel>();

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

                    // 1ターン経過
                    AddBombCooldownTurn();
                }
            }
        }
    }

    // 爆弾設置モードを開始

    public void StartBombMode()
    {
        // まだ使えない場合
        if (bombCooldown < BombCooldownMax)
        {
            Debug.Log(
                $"まだ爆弾を使えません。" +
                $"あと {BombCooldownMax - bombCooldown} ターン"
            );

            return;
        }

        bombMode = true;

        Debug.Log("爆弾設置モード開始");

        UpdateBombButton();
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

        // 爆弾を使ったのでクールダウン開始
        bombCooldown = 0;

        // 爆弾設置モード終了
        bombMode = false;

        // ボタンの色を更新
        UpdateBombButton();

        Debug.Log("爆弾設置モード終了");
    }

    // ターン経過
    
    void AddBombCooldownTurn()
    {
        // すでに最大なら何もしない
        if (bombCooldown >= BombCooldownMax)
            return;

        bombCooldown++;

        Debug.Log(
            $"爆弾クールダウン：" +
            $"{bombCooldown}/{BombCooldownMax}"
        );

        // ボタンの表示を更新
        UpdateBombButton();

        if (bombCooldown >= BombCooldownMax)
        {
            Debug.Log(
                "爆弾設置が再び使用可能になりました！"
            );
        }
    }

    // 爆弾ボタンの表示更新
    
    void UpdateBombButton()
    {
        if (bombButton == null)
            return;

        // 使用可能
        if (bombCooldown >= BombCooldownMax)
        {
            bombButton.interactable = true;

            ColorBlock colors = bombButton.colors;

            colors.normalColor = Color.white;

            bombButton.colors = colors;
        }
        // 使用不可
        else
        {
            bombButton.interactable = false;

            ColorBlock colors = bombButton.colors;

            colors.normalColor = Color.gray;

            bombButton.colors = colors;
        }
    }

    // 盤面生成

    void CreateGrid()
    {
        float startX =
            -(width - 1) * spacing / 2f;

        float startY =
            (height - 1) * spacing / 2f;

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


                Panel panel =
                    obj.GetComponent<Panel>();

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

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

    [Header("カメラ設定")]
    public Camera targetCamera;

    // 盤面の周囲に少し余白を作る
    public float cameraMargin = 1.0f;

    private Panel[,] panels;
    private Camera mainCamera;

    private bool bombMode = false;

    private int bombCooldown = 0;
    private const int BombCooldownMax = 3;


    void Awake()
    {
        // Inspectorでカメラを設定していればそれを使用
        if (targetCamera != null)
        {
            mainCamera = targetCamera;
        }
        else
        {
            // 設定されていなければMain Cameraを取得
            mainCamera = Camera.main;
        }
    }


    void Start()
    {
        // ゲーム設定を読み込む

        if (GameSettings.Instance != null)
        {
            // パネルサイズ
            width = GameSettings.Instance.panelWidth;
            height = GameSettings.Instance.panelHeight;

            // 爆弾の数
            bombCount = GameSettings.Instance.bombCount;

            Debug.Log("========== GridManager設定 ==========");
            Debug.Log(
                "パネルサイズ : " +
                width +
                " × " +
                height
            );

            Debug.Log(
                "爆弾の数 : " +
                bombCount
            );

            Debug.Log("====================================");
        }
        else
        {
            Debug.LogWarning(
                "GameSettings.Instanceが見つかりません。"
                + "Inspectorの設定値を使用します。"
            );
        }

        // パネル配列を作成

        panels = new Panel[width, height];

        // パネルを生成

        CreateGrid();

        // カメラを盤面サイズに合わせる

        AdjustCamera();

        // 小人を配置

        int dwarfCount = 1;

        if (GameSettings.Instance != null)
        {
            dwarfCount = GameSettings.Instance.dwarfCount;

            Debug.Log("小人の数 : " + dwarfCount);
        }

        for (int i = 0; i < dwarfCount; i++)
        {
            DwarfManager.Instance.SpawnDwarf(DwarfType.Stay);
        }

        // 爆弾を配置

        for (int i = 0; i < bombCount; i++)
        {
            BombManager.Instance.SpawnBomb();
        }

        // 爆弾クールダウンを初期化

        bombCooldown = BombCooldownMax;

        UpdateBombButton();
    }


    void Update()
    {
        // クリアまたはゲームオーバーなら操作しない
        if (GameManager.Instance.isGameClear ||
            GameManager.Instance.isGameOver)
        {
            return;
        }


        // マウス左クリック
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector3 screenPos =
                Mouse.current.position.ReadValue();

            screenPos.z =
                -mainCamera.transform.position.z;

            Vector3 worldPos =
                mainCamera.ScreenToWorldPoint(screenPos);

            worldPos.z = 0;


            // クリックした場所にあるColliderを取得
            Collider2D hit =
                Physics2D.OverlapPoint(worldPos);


            if (hit != null)
            {
                Panel panel =
                    hit.GetComponent<Panel>();


                if (panel != null)
                {
                    // 爆弾設置モードの場合
                    if (bombMode)
                    {
                        PlaceBomb(panel);
                        return;
                    }


                    // 通常のパネル破壊
                    panel.BreakPanel();


                    // 爆弾クールダウンを1ターン進める
                    AddBombCooldownTurn();
                }
            }
        }
    }

    // カメラサイズを盤面に合わせる

    void AdjustCamera()
    {
        if (mainCamera == null)
        {
            Debug.LogWarning(
                "Main Cameraが見つかりません。"
            );

            return;
        }


        // 盤面の横幅
        float boardWidth =
            (width - 1) * spacing;


        // 盤面の縦幅
        float boardHeight =
            (height - 1) * spacing;


        // カメラの縦方向に必要なサイズ
        float verticalSize =
            (boardHeight / 2f)
            + cameraMargin;


        // 画面のアスペクト比
        float aspect =
            (float)Screen.width /
            Screen.height;


        // 横幅から必要なカメラサイズを計算
        float horizontalSize =
            (boardWidth / aspect / 2f)
            + cameraMargin;


        // 縦・横のうち大きい方を使用
        float cameraSize =
            Mathf.Max(
                verticalSize,
                horizontalSize
            );


        mainCamera.orthographicSize =
            cameraSize;


        Debug.Log(
            "カメラサイズを調整 : "
            + cameraSize
        );
    }

    // 爆弾設置モード開始

    public void StartBombMode()
    {
        if (bombCooldown < BombCooldownMax)
        {
            Debug.Log(
                "まだ爆弾を使えません。" +
                "あと " +
                (BombCooldownMax - bombCooldown) +
                " ターン"
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
        // 壊れたパネルには置けない
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


        // すでに爆弾がある場合
        if (panel.hasBomb)
        {
            Debug.Log("このパネルにはすでに爆弾があります");
            return;
        }


        // 爆弾を設置
        BombManager.Instance.PlaceBomb(panel);


        // ターンを1消費
        GameManager.Instance.UseTurn();


        Debug.Log(
            $"爆弾を設置 ({panel.x},{panel.y})"
        );


        // クールダウン開始
        bombCooldown = 0;

        // 爆弾モード終了
        bombMode = false;

        UpdateBombButton();


        Debug.Log(
            "爆弾設置モード終了。" +
            "3ターン後に再使用可能"
        );
    }

    // 爆弾クールダウン

    void AddBombCooldownTurn()
    {
        if (bombCooldown >= BombCooldownMax)
        {
            return;
        }


        bombCooldown++;


        Debug.Log(
            "爆弾クールダウン：" +
            bombCooldown +
            "/" +
            BombCooldownMax
        );


        UpdateBombButton();


        if (bombCooldown >= BombCooldownMax)
        {
            Debug.Log(
                "爆弾設置が再び使用可能になりました！"
            );
        }
    }

    // 爆弾ボタンの表示を更新

    void UpdateBombButton()
    {
        if (bombButton == null)
        {
            return;
        }


        if (bombCooldown >= BombCooldownMax)
        {
            bombButton.interactable = true;

            ColorBlock colors =
                bombButton.colors;

            colors.normalColor =
                Color.white;

            bombButton.colors =
                colors;
        }
        else
        {
            bombButton.interactable = false;

            ColorBlock colors =
                bombButton.colors;

            colors.normalColor =
                Color.gray;

            bombButton.colors =
                colors;
        }
    }

    // パネルを生成

    void CreateGrid()
    {
        // 盤面を中央に配置するための開始位置
        float startX =
            -(width - 1) * spacing / 2f;

        float startY =
            (height - 1) * spacing / 2f;


        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 pos =
                    new Vector3(
                        startX + x * spacing,
                        startY - y * spacing,
                        0
                    );


                Debug.Log(
                    $"Panel ({x},{y}) = {pos}"
                );


                // パネルを生成
                GameObject obj =
                    Instantiate(
                        panelPrefab,
                        pos,
                        Quaternion.identity,
                        transform
                    );


                // Panelコンポーネントを取得
                Panel panel =
                    obj.GetComponent<Panel>();


                // 座標を設定
                panel.x = x;
                panel.y = y;


                // 配列に保存
                panels[x, y] = panel;
            }
        }

        Debug.Log(
            $"パネル生成完了 : {width} × {height}"
        );
    }
    
    // Panel配列を取得

    public Panel[,] GetPanels()
    {
        return panels;
    }

    // 盤面の横幅

    public int Width => width;

    // 盤面の高さ

    public int Height => height;
}

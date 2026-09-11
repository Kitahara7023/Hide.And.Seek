using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [Header("GridManager")]
    public GridManager gridManager;

    [Header("HINTボタン")]
    public Button hintButton;

    [Header("ヒント枠")]
    public float frameWidth = 0.08f;
    public float framePadding = 0.05f;

    [Header("ヒント枠カラー")]
    public Color[] hintColors =
    {
        Color.green,
        Color.blue,
        Color.yellow,
        Color.magenta,
        Color.cyan
    };

    [Header("ヒント設定")]
    public int hintInterval = 4;
    public int maxHintCount = 2;

    private int usedHintCount = 0;

    // 小人ごとのヒント枠を記録
    private Dictionary<Dwarf, GameObject> dwarfFrames =
        new Dictionary<Dwarf, GameObject>();

    // 現在表示されているヒント枠
    private List<GameObject> currentHintFrames =
        new List<GameObject>();

    void Update()
    {
        UpdateHintButton();
    }

    public void UseHint()
    {
        if (!CanUseHint())
        {
            Debug.Log("まだHINTは使用できません");
            return;
        }

        // すでに表示されているヒント枠を削除
        ClearHintFrames();

        ShowHint();

        usedHintCount++;

        UpdateHintButton();

        Debug.Log(
            "HINTを使用しました : "
            + usedHintCount
            + " / "
            + maxHintCount
        );
    }

    bool CanUseHint()
    {
        if (GameManager.Instance == null)
            return false;

        if (usedHintCount >= maxHintCount)
            return false;

        int usedTurn =
            GameManager.Instance.startTurn -
            GameManager.Instance.currentTurn;

        int requiredTurn =
            (usedHintCount + 1) * hintInterval;

        return usedTurn >= requiredTurn;
    }

    void UpdateHintButton()
    {
        if (hintButton == null)
            return;

        if (CanUseHint())
        {
            hintButton.interactable = true;

            ColorBlock colors = hintButton.colors;
            colors.normalColor = Color.white;
            hintButton.colors = colors;
        }
        else
        {
            hintButton.interactable = false;

            ColorBlock colors = hintButton.colors;
            colors.normalColor = Color.gray;
            hintButton.colors = colors;
        }
    }

    void ShowHint()
    {
        Panel[,] panels = gridManager.GetPanels();

        int dwarfIndex = 0;

        for (int x = 0; x < gridManager.Width; x++)
        {
            for (int y = 0; y < gridManager.Height; y++)
            {
                Panel panel = panels[x, y];

                if (panel == null)
                    continue;

                if (panel.dwarf == null)
                    continue;

                Dwarf dwarf = panel.dwarf;

                // すでに発見済みの小人は無視
                if (dwarf.isFound)
                    continue;

                int hintSize =
                    GetHintSize();

                Color hintColor =
                    GetHintColor(dwarfIndex);

                CreateHintFrame(
                    panel.x,
                    panel.y,
                    hintSize,
                    panels,
                    hintColor,
                    dwarf
                );

                dwarfIndex++;
            }
        }
    }

    int GetHintSize()
    {
        if (gridManager.Width == 5)
        {
            if (usedHintCount == 0)
                return 3;

            return 1;
        }

        if (gridManager.Width == 7)
        {
            if (usedHintCount == 0)
                return 5;

            return 3;
        }

        if (gridManager.Width == 9)
        {
            if (usedHintCount == 0)
                return 7;

            return 5;
        }

        return 3;
    }

    Color GetHintColor(int index)
    {
        if (hintColors == null ||
            hintColors.Length == 0)
        {
            return Color.green;
        }

        return hintColors[index % hintColors.Length];
    }

    void CreateHintFrame(
        int dwarfX,
        int dwarfY,
        int size,
        Panel[,] panels,
        Color hintColor,
        Dwarf dwarf)
    {
        int minStartX =
            Mathf.Max(0, dwarfX - size + 1);

        int maxStartX =
            Mathf.Min(
                dwarfX,
                gridManager.Width - size
            );

        int minStartY =
            Mathf.Max(0, dwarfY - size + 1);

        int maxStartY =
            Mathf.Min(
                dwarfY,
                gridManager.Height - size
            );

        int startX =
            Random.Range(
                minStartX,
                maxStartX + 1
            );

        int startY =
            Random.Range(
                minStartY,
                maxStartY + 1
            );

        int endX =
            startX + size - 1;

        int endY =
            startY + size - 1;

        Panel bottomLeftPanel =
            panels[startX, endY];

        Panel topRightPanel =
            panels[endX, startY];

        Vector3 bottomLeft =
            bottomLeftPanel.transform.position;

        Vector3 topRight =
            topRightPanel.transform.position;

        float halfSpacing =
            gridManager.spacing / 2f;

        bottomLeft -=
            new Vector3(
                halfSpacing,
                halfSpacing,
                0
            );

        topRight +=
            new Vector3(
                halfSpacing,
                halfSpacing,
                0
            );

        bottomLeft -=
            new Vector3(
                framePadding,
                framePadding,
                0
            );

        topRight +=
            new Vector3(
                framePadding,
                framePadding,
                0
            );

        // ヒント枠用オブジェクト
        GameObject frame =
            new GameObject(
                "HintFrame"
            );

        LineRenderer line =
            frame.AddComponent<LineRenderer>();

        line.positionCount = 5;
        line.loop = false;

        line.startWidth = frameWidth;
        line.endWidth = frameWidth;

        line.startColor = hintColor;
        line.endColor = hintColor;

        line.material =
            new Material(
                Shader.Find("Sprites/Default")
            );

        line.useWorldSpace = true;

        Vector3 bottomRight =
            new Vector3(
                topRight.x,
                bottomLeft.y,
                0
            );

        Vector3 topLeft =
            new Vector3(
                bottomLeft.x,
                topRight.y,
                0
            );

        line.SetPosition(0, bottomLeft);
        line.SetPosition(1, bottomRight);
        line.SetPosition(2, topRight);
        line.SetPosition(3, topLeft);
        line.SetPosition(4, bottomLeft);

        // この枠はこの小人用
        dwarfFrames[dwarf] = frame;

        // 作成したヒント枠をリストに追加
        currentHintFrames.Add(frame);

        Debug.Log(
            $"小人 ({dwarfX},{dwarfY}) のヒント枠を作成"
        );
    }

    void ClearHintFrames()
    {
        foreach (GameObject frame in currentHintFrames)
        {
            if (frame != null)
            {
                Destroy(frame);
            }
        }

        currentHintFrames.Clear();

        Debug.Log("前回のヒント枠をすべて削除しました");
    }

    // 小人が発見されたときに呼ぶ
    public void RemoveHintFrame(Dwarf dwarf)
    {
        if (dwarf == null)
            return;

        if (dwarfFrames.TryGetValue(
            dwarf,
            out GameObject frame))
        {
            if (frame != null)
            {
                Destroy(frame);
            }

            dwarfFrames.Remove(dwarf);

            Debug.Log(
                "発見された小人のヒント枠を削除しました"
            );
        }
    }
}

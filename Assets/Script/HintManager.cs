using System.Collections;
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

    [Header("ヒント自動切り替え")]
    public float hintSwitchInterval = 2f;

    // ヒント枠の位置情報
    private List<HintFrameInfo> hintFrameInfos =
        new List<HintFrameInfo>();

    // 実行中の切り替え処理
    private List<Coroutine> hintCoroutines =
        new List<Coroutine>();


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

        // 重なっているヒント枠の自動切り替えを開始
        StartHintSwitching();
    }

    int GetHintSize()
    {
        if (gridManager.Width == 5)
        {
            if (usedHintCount == 0)
                return 3;

            return 2;
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

        if(gridManager.Width == 11)
        {
            if (usedHintCount == 1)
                return 7;

            if (usedHintCount == 2)
                return 5;
        }

        if (gridManager.Width == 13)
        {
            if (usedHintCount == 0)
                return 7;

            if (usedHintCount == 1)
                return 5;
        }

        if (gridManager.Width == 15)
        {
            if (usedHintCount == 0)
                return 7;

            if (usedHintCount == 1)
                return 5;
        }
        return 5;
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

        // ヒント枠の範囲を記録
        hintFrameInfos.Add(
            new HintFrameInfo(
                dwarf,
                frame,
                startX,
                startY,
                endX,
                endY
            )
        );

        Debug.Log(
            $"小人 ({dwarfX},{dwarfY}) のヒント枠を作成"
        );
    }

    // ヒント枠の情報
    private class HintFrameInfo
    {
        public Dwarf dwarf;
        public GameObject frame;

        public int startX;
        public int startY;
        public int endX;
        public int endY;

        public HintFrameInfo(
            Dwarf dwarf,
            GameObject frame,
            int startX,
            int startY,
            int endX,
            int endY)
        {
            this.dwarf = dwarf;
            this.frame = frame;
            this.startX = startX;
            this.startY = startY;
            this.endX = endX;
            this.endY = endY;
        }
    }

    // ヒント範囲が重なっているか確認
    bool IsOverlapping(HintFrameInfo a, HintFrameInfo b)
    {
        // 2つの枠が重なっている範囲を計算
        int overlapMinX = Mathf.Max(a.startX, b.startX);
        int overlapMaxX = Mathf.Min(a.endX, b.endX);

        int overlapMinY = Mathf.Max(a.startY, b.startY);
        int overlapMaxY = Mathf.Min(a.endY, b.endY);

        int overlapWidth = overlapMaxX - overlapMinX + 1;
        int overlapHeight = overlapMaxY - overlapMinY + 1;

        // 重なっていない場合
        if (overlapWidth <= 0 || overlapHeight <= 0)
            return false;

        // 重なっている面積
        int overlapArea = overlapWidth * overlapHeight;

        // それぞれの枠の面積
        int areaA =
            (a.endX - a.startX + 1) *
            (a.endY - a.startY + 1);

        int areaB =
            (b.endX - b.startX + 1) *
            (b.endY - b.startY + 1);

        // 面積が小さい方を基準にする
        int smallerArea = Mathf.Min(areaA, areaB);

        float overlapRatio =
            (float)overlapArea / smallerArea;

        // 100％の場合だけ交互表示の対象
        return overlapRatio >= 1.0f;
    }


    // 重なっているヒント枠を探す
    void StartHintSwitching()
    {
        List<HintFrameInfo> remaining =
            new List<HintFrameInfo>(hintFrameInfos);

        while (remaining.Count > 0)
        {
            // グループの最初の小人
            List<HintFrameInfo> group =
                new List<HintFrameInfo>();

            group.Add(remaining[0]);
            remaining.RemoveAt(0);

            // 重なっている枠を同じグループにまとめる
            bool foundOverlap = true;

            while (foundOverlap)
            {
                foundOverlap = false;

                for (int i = remaining.Count - 1; i >= 0; i--)
                {
                    foreach (HintFrameInfo info in group)
                    {
                        if (IsOverlapping(info, remaining[i]))
                        {
                            group.Add(remaining[i]);
                            remaining.RemoveAt(i);

                            foundOverlap = true;
                            break;
                        }
                    }
                }
            }

            // 2人以上いるグループだけ切り替える
            if (group.Count >= 2)
            {
                Coroutine coroutine =
                    StartCoroutine(CycleHintGroup(group));

                hintCoroutines.Add(coroutine);
            }
        }
    }


    // 同じ範囲にいる小人のヒントを切り替える
    IEnumerator CycleHintGroup(
        List<HintFrameInfo> group)
    {
        int index = 0;

        while (true)
        {
            List<HintFrameInfo> activeFrames =
                new List<HintFrameInfo>();

            // 発見済みではない小人だけを取得
            foreach (HintFrameInfo info in group)
            {
                if (info.dwarf != null &&
                    !info.dwarf.isFound &&
                    info.frame != null)
                {
                    activeFrames.Add(info);
                }
            }

            // 表示対象がなくなった
            if (activeFrames.Count == 0)
                yield break;

            // 残り1人なら、そのヒントを表示したまま終了
            if (activeFrames.Count == 1)
            {
                activeFrames[0].frame.SetActive(true);
                yield break;
            }

            // いったん全員のヒントを非表示
            foreach (HintFrameInfo info in activeFrames)
            {
                info.frame.SetActive(false);
            }

            // 今回表示する小人
            if (index >= activeFrames.Count)
                index = 0;

            activeFrames[index].frame.SetActive(true);

            Debug.Log(
                "ヒント表示切り替え : "
                + activeFrames[index].dwarf.name
            );

            index++;

            // 指定時間待つ
            yield return new WaitForSeconds(
                hintSwitchInterval
            );
        }
    }

    void ClearHintFrames()
    {
        // 実行中の切り替え処理を停止
        foreach (Coroutine coroutine in hintCoroutines)
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
            }
        }

        hintCoroutines.Clear();

        // ヒント枠を削除
        foreach (GameObject frame in currentHintFrames)
        {
            if (frame != null)
            {
                Destroy(frame);
            }
        }

        currentHintFrames.Clear();

        // 管理情報も初期化
        hintFrameInfos.Clear();
        dwarfFrames.Clear();

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

            currentHintFrames.Remove(frame);

            Debug.Log(
                "発見された小人のヒント枠を削除しました"
            );
        }

        // 切り替え対象からも削除
        hintFrameInfos.RemoveAll(
            info => info.dwarf == dwarf
        );
    }
}

using UnityEngine;
using System.Collections.Generic;

public class DwarfManager : MonoBehaviour
{
    // シングルトン
    public static DwarfManager Instance;

    [Header("GridManager")]
    public GridManager gridManager;

    [Header("小人Prefab")]
    public Dwarf DwarfPrefab;

    // 小人がいるパネルを保存
    private List<Dwarf> dwarfs = new List<Dwarf>();

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        
    }

    /// 小人を配置する

    public void SpawnDwarf(DwarfType type)
    {
        Debug.Log("SpawnDwarf開始");

        Panel[,] panels = gridManager.GetPanels();

        Debug.Log($"panels = {panels}");

        for (int i = 0; i < 100; i++)
        {
            int x = Random.Range(0, gridManager.Width);
            int y = Random.Range(0, gridManager.Height);

            Panel panel = panels[x, y];

            Debug.Log($"[{x},{y}] panel = {panel}");

            if (panel == null)
            {
                Debug.LogError("Panelがnullです");
                continue;
            }

            Debug.Log($"dwarf = {panel.dwarf}");

            if (panel.dwarf != null)
            {
                Debug.Log("既に小人がいます");
                continue;
            }

            Debug.Log("ここまで来ました");

            Dwarf dwarf = Instantiate(DwarfPrefab);

            dwarf.dwarfType = type;
            dwarf.SetPanel(panel);

            //dwarf.gameObject.SetActive(false);

            dwarfs.Add(dwarf);

            Debug.Log($"小人配置 ({x},{y})");

            return;
        }

        Debug.LogError("小人を配置できませんでした");
    }
   
}

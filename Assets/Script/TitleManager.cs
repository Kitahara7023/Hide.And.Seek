using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleManager : MonoBehaviour
{
    [Header("ゲームシーン名")]
    public string gameSceneName = "GameScene";

    // STARTボタンを押した時
    public void StartGame()
    {
        Debug.Log("ゲームを開始します");

        SceneManager.LoadScene(gameSceneName);
    }
}
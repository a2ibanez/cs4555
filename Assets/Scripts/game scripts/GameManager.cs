using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject gameOverPanel;
    public GameObject victoryPanel;

    public int winCondition = 4;

    private bool gameEnded = false;

    void Awake()
    {
        instance = this;

        gameOverPanel.SetActive(false);
        victoryPanel.SetActive(false);
    }

    public void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;

        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;

        UnlockCursor();
    }

    public void CheckWin(int deliveredCount)
    {
        if (gameEnded) return;

        if (deliveredCount >= winCondition)
        {
            gameEnded = true;

            victoryPanel.SetActive(true);
            Time.timeScale = 0f;

            UnlockCursor();
        }
    }

    void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
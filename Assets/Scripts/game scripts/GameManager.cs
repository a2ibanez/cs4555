using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI victoryText;

    public int winCondition = 4;

    private bool gameEnded = false;

    void Awake()
    {
        instance = this;

        gameOverText.gameObject.SetActive(false);
        victoryText.gameObject.SetActive(false);
    }

    public void GameOver()
    {
        if (gameEnded) return;

        gameEnded = true;

        gameOverText.gameObject.SetActive(true);

        Time.timeScale = 0f; // freeze game

        UnlockCursor();
    }

    public void CheckWin(int deliveredCount)
    {
        if (gameEnded) return;

        if (deliveredCount >= winCondition)
        {
            gameEnded = true;

            victoryText.gameObject.SetActive(true);

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
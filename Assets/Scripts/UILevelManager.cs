using UnityEngine;
using UnityEngine.SceneManagement;

public class UILevelManager : MonoBehaviour
{
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadLevelSelect()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level Select");
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        string current = SceneManager.GetActiveScene().name;

        if (current == "Level 1")
            SceneManager.LoadScene("Level 2");
        else if (current == "Level 2")
            SceneManager.LoadScene("Level 3");
        else
            SceneManager.LoadScene("Level Select");
    }
}
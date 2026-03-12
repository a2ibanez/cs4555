using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    public int level;
    public TextMeshProUGUI levelText;

    void Start()
    {
        levelText.text = level.ToString();
    }

    public void OpenScene(){
        print("Level " + level.ToString());
        SceneManager.LoadScene("Level " + level.ToString());
    }
}

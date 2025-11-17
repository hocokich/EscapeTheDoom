using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void PlayGame()
    {
        if (GameManager.instance != null)
        {
			GameManager.instance.player = null;
			GameManager.instance.PreviusPlayer = null;
			GameManager.instance.level = 1;
            GameManager.instance.mazeSize = 5;   // старт
            GameManager.instance.mazeStep = 2;   // шаг роста
        }
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        if (GameManager.instance != null) GameManager.instance.QuitGame();
        else
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    // Fungsi untuk memulai ulang game
    public void Restart()
    {
        // Memuat ulang scene saat ini
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Fungsi untuk keluar dari game (opsional, untuk build)
    public void QuitGame()
    {
        // Keluar dari aplikasi
        Application.Quit();
        Debug.Log("Game has been exited."); // Debug log untuk editor
    }
}

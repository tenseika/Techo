using UnityEngine;
using UnityEngine.UI;

public class PlayerUIManager : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Slider healthSlider; // Health Slider
    [SerializeField] private GameObject gameOverPanel; // Panel Game Over

    [Header("Player Health")]
    [SerializeField] private PlayerHealth playerHealth; // Referensi ke PlayerHealth

    private void Start()
    {
        // Pastikan Game Over Panel tidak terlihat saat game dimulai
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        // Pastikan health slider diatur dengan benar
        if (healthSlider != null && playerHealth != null)
        {
            healthSlider.maxValue = playerHealth.GetMaxHealth();
            healthSlider.value = playerHealth.GetCurrentHealth();
        }

        // Daftarkan event ke PlayerHealth
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged += UpdateHealthUI;
        }
    }

    private void UpdateHealthUI(int currentHealth, int maxHealth)
    {
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }

        // Tampilkan Game Over Panel jika health habis
        if (currentHealth <= 0)
        {
            if (gameOverPanel != null)
            {
                gameOverPanel.SetActive(true); // Tampilkan Game Over Panel
            }

            if (healthSlider != null)
            {
                healthSlider.gameObject.SetActive(false); // Sembunyikan Health Slider
            }
        }
    }

    private void OnDestroy()
    {
        // Hapus event untuk mencegah memory leak
        if (playerHealth != null)
        {
            playerHealth.OnHealthChanged -= UpdateHealthUI;
        }
    }
}

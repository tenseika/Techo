using UnityEngine;
using System;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100; // Maksimum health
    private int currentHealth;

    // Event untuk memperbarui UI
    public Action<int, int> OnHealthChanged;

    private void Awake()
    {
        currentHealth = maxHealth; // Set health awal ke maksimum
        OnHealthChanged?.Invoke(currentHealth, maxHealth); // Trigger awal untuk UI
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Batasi health antara 0 dan maxHealth

        // Trigger event untuk memperbarui UI
        OnHealthChanged?.Invoke(currentHealth, maxHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Pastikan tidak melebihi maxHealth

        // Trigger event untuk memperbarui UI
        OnHealthChanged?.Invoke(currentHealth, maxHealth);
    }

    private void Die()
    {
        Debug.Log("Player has died!");
        // Tambahkan logika kematian di sini
    }

    // Fungsi untuk mendapatkan health saat ini
    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    // Fungsi untuk mendapatkan health maksimum
    public int GetMaxHealth()
    {
        return maxHealth;
    }
}

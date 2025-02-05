using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    private InputManager inputManager;
    private PlayerLocomotion playerLocomotion;

    [Header("Energy Settings")]
    [SerializeField] private float maxEnergy = 100f; // Energi maksimum
    [SerializeField] private float energyRecoveryRate = 10f; // Pemulihan energi per detik
    [SerializeField] private float runEnergyCost = 15f; // Energi yang digunakan per detik untuk lari
    [SerializeField] private float dashEnergyCost = 25f; // Energi yang digunakan untuk dash
    private float currentEnergy;

    [Header("Debug Settings")]
    [SerializeField] private bool enableEnergyDebug = true; // Debug untuk memantau energi di Unity Console

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerLocomotion = GetComponent<PlayerLocomotion>();
        currentEnergy = maxEnergy; // Mulai dengan energi penuh
    }

    private void Update()
    {
        HandleEnergy(); // Mengatur sistem energi

        playerLocomotion.HandleMovement();
        playerLocomotion.HandleRotation();
        playerLocomotion.HandleDash();

        // Debug untuk memantau energi
        if (enableEnergyDebug)
        {
            Debug.Log($"Current Energy: {currentEnergy}/{maxEnergy}");
        }
    }

    private void HandleEnergy()
    {
        // Jika pemain berlari
        if (inputManager.IsRunning && currentEnergy > 0)
        {
            currentEnergy -= runEnergyCost * Time.deltaTime; // Kurangi energi
            if (currentEnergy <= 0)
            {
                currentEnergy = 0;
                inputManager.IsRunning = false; // Paksa pemain berhenti berlari
            }
        }

        // Jika pemain melakukan dash
        if (inputManager.IsDashing)
        {
            if (currentEnergy >= dashEnergyCost)
            {
                currentEnergy -= dashEnergyCost; // Kurangi energi untuk dash
                inputManager.IsDashing = false; // Reset status dash setelah dash dilakukan
            }
        }

        // Pulihkan energi jika pemain tidak berlari atau dash
        if (!inputManager.IsRunning && !inputManager.IsDashing)
        {
            currentEnergy += energyRecoveryRate * Time.deltaTime;
            currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        }
    }

    // Getter untuk energi saat ini
    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    // Getter untuk energi maksimum
    public float GetMaxEnergy()
    {
        return maxEnergy;
    }
}

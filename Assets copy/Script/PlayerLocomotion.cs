using System.Collections;
using UnityEngine;

public class PlayerLocomotion : MonoBehaviour
{
    private InputManager inputManager;

    private Vector3 moveDirection;
    private Transform cameraObject;
    private Rigidbody playerRigidbody;

    [Header("Movement Speeds")]
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float runSpeed = 7f;
    [SerializeField] private float sneakSpeed = 2f;

    [Header("Dash Settings")]
    [SerializeField] private float dashForce = 20f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 1f;
    private bool isDashing = false;
    private bool canDash = true;

    [Header("Sneak Settings")]
    [SerializeField] private float sneakHeight = 0.8f; // Tinggi karakter saat sneak
    [SerializeField] private float normalHeight = 2f; // Tinggi karakter normal
    private bool isSneaking = false;

    private CapsuleCollider playerCollider;

    [Header("Rotation")]
    [SerializeField] private float rotationSpeed = 15f;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        playerRigidbody = GetComponent<Rigidbody>();
        cameraObject = Camera.main != null ? Camera.main.transform : null;
        playerCollider = GetComponent<CapsuleCollider>();

        if (inputManager == null)
        {
            Debug.LogError("InputManager is not attached to the GameObject.");
        }

        if (cameraObject == null)
        {
            Debug.LogError("Main Camera not found in the scene.");
        }
    }

    public void HandleMovement()
    {
        if (isDashing) return; // Tidak bergerak normal saat dash

        float currentSpeed = walkSpeed;

        if (inputManager.IsRunning)
        {
            currentSpeed = runSpeed;
        }
        else if (inputManager.IsSneaking)
        {
            currentSpeed = sneakSpeed;
            HandleSneak(true);
        }
        else
        {
            HandleSneak(false);
        }

        moveDirection = cameraObject.forward * inputManager.Y;
        moveDirection += cameraObject.right * inputManager.X;
        moveDirection.Normalize();
        moveDirection.y = 0;
        moveDirection *= currentSpeed;

        playerRigidbody.velocity = new Vector3(moveDirection.x, playerRigidbody.velocity.y, moveDirection.z);
    }

    public void HandleRotation()
    {
        if (cameraObject == null) return; // Pastikan cameraObject tidak null

        Vector3 targetDirection = cameraObject.forward * inputManager.Y;
        targetDirection += cameraObject.right * inputManager.X;
        targetDirection.Normalize();
        targetDirection.y = 0;

        if (targetDirection == Vector3.zero) return;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        Quaternion playerRotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        transform.rotation = playerRotation;
    }

    public void HandleDash()
    {
        if (inputManager.IsDashing && canDash)
        {
            StartCoroutine(PerformDash());
        }
    }

    private IEnumerator PerformDash()
    {
        isDashing = true;
        canDash = false;

        // Hitung arah dash
        Vector3 dashDirection = cameraObject.forward * inputManager.Y + cameraObject.right * inputManager.X;
        dashDirection.Normalize();

        // Terapkan gaya dash
        playerRigidbody.AddForce(dashDirection * dashForce, ForceMode.Impulse);

        // Tunggu durasi dash
        yield return new WaitForSeconds(dashDuration);

        isDashing = false;

        // Tunggu cooldown sebelum dash berikutnya
        yield return new WaitForSeconds(dashCooldown);

        canDash = true;
    }

    private void HandleSneak(bool isSneaking)
    {
        if (this.isSneaking == isSneaking) return;

        this.isSneaking = isSneaking;

        if (isSneaking)
        {
            // Kurangi tinggi karakter untuk sneak
            playerCollider.height = sneakHeight;
        }
        else
        {
            // Kembalikan tinggi karakter ke normal
            playerCollider.height = normalHeight;
        }
    }
}

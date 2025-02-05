using UnityEngine;

public class InputManager : MonoBehaviour
{
    private PlayerControls playerControls;

    [SerializeField] private Vector2 movementInput;

    public float X => movementInput.x; // Arah horizontal
    public float Y => movementInput.y; // Arah vertikal

    public bool IsRunning { get; set; } // Status berlari
    public bool IsDashing { get; set; } // Status dash
    public bool IsSneaking { get; private set; } // Status sneak

    private void OnEnable()
    {
        if (playerControls == null)
        {
            playerControls = new PlayerControls();

            // Mengatur input gerakan
            playerControls.PlayerMovement.Movement.performed += ctx =>
                movementInput = ctx.ReadValue<Vector2>();
            playerControls.PlayerMovement.Movement.canceled += ctx =>
                movementInput = Vector2.zero;

            // Mengatur input lari
            playerControls.PlayerMovement.Run.started += _ => IsRunning = true;
            playerControls.PlayerMovement.Run.canceled += _ => IsRunning = false;

            // Mengatur input dash
            playerControls.PlayerMovement.Dash.started += _ => StartDash();
            playerControls.PlayerMovement.Dash.canceled += _ => StopDash();

            // Mengatur toggle sneak
            playerControls.PlayerMovement.Sneak.started += _ => ToggleSneak();
        }

        playerControls.Enable();
    }

    private void OnDisable()
    {
        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }

    private void ToggleSneak()
    {
        IsSneaking = !IsSneaking; // Aktifkan atau nonaktifkan sneak
    }

    private void StartDash()
    {
        IsDashing = true;
    }

    private void StopDash()
    {
        IsDashing = false;
    }
}

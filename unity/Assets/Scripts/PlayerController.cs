using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController character;
    private MouseController mouse;

    private float yVelocity;

    private InputActionMap player;

    private InputAction moveButtons;
    private InputAction jump;

    private FloorController floor;

    private void Start()
    {
        character = character ?? gameObject.GetComponent<CharacterController>();
        mouse = mouse ?? gameObject.GetComponent<MouseController>();
        floor = floor ?? gameObject.GetComponent<FloorController>();

        player = InputSystem.actions.FindActionMap("Player");
        moveButtons = player.FindAction("Move");
        jump = player.FindAction("Jump");

        player.Enable();
    }

    private float HandleJump()
    {
        if (character.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;

            if (jump.WasPressedThisFrame())
            {
                yVelocity = Statics.JumpForce;
            }
        }

        yVelocity += Statics.Gravity * Time.deltaTime * 2;

        return yVelocity;
    }

    private void Update()
    {

        transform.Rotate(Vector3.up * mouse.X);

        Vector2 move2 = moveButtons.ReadValue<Vector2>();
        Vector3 move3 = new Vector3(move2.x, 0, move2.y);

        if (move3.magnitude > 1f)
        {
            move3 = move3.normalized;
        }

        move3 = transform.TransformDirection(move3);
        move3 *= Statics.PlayerSpeed;
        move3.y = this.HandleJump();

        character.Move(move3 * Time.deltaTime);
        transform.position = floor.Clamp(transform.position);
    }
}


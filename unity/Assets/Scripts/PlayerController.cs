using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private CharacterController character;

    private float yVelocity;

    private InputActionMap player;

    private InputAction moveButtons;
    private InputAction jump;

    private FloorController floor;

    private void Start()
    {
        if (!character)
        {
            character = gameObject.GetComponent<CharacterController>();
        }

        if (!floor)
        {
            floor = gameObject.GetComponent<FloorController>();
        }

        player = InputSystem.actions.FindActionMap("Player");
        moveButtons = player.FindAction("Move");
        jump = player.FindAction("Jump");

        player.Enable();
    }

    private void Update()
    {
        Vector2 move2 = moveButtons.ReadValue<Vector2>();
        Vector3 move3 = new Vector3(move2.x, 0, move2.y);

        if (move3.magnitude > 1f)
        {
            move3 = move3.normalized;
        }

        move3 = transform.TransformDirection(move3);
        move3 *= Constants.Speed;

        if (character.isGrounded && yVelocity < 0)
        {
            yVelocity = -2f;

            if (jump.WasPressedThisFrame())
            {
                yVelocity = Constants.JumpForce;
            }
        }

        yVelocity += Constants.Gravity * Time.deltaTime * 2;
        move3.y = yVelocity;

        character.Move(move3 * Time.deltaTime);

        transform.position = floor.Clamp(transform.position);
    }
}


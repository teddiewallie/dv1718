using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    public InputActionMap UIButtons { get; private set; }
    public InputActionMap Player { get; private set; }

    public InputAction TogglePause { get; private set; }

    public InputAction MoveButtons { get; private set; }
    public InputAction Jump { get; private set; }
    public InputAction Attack { get; private set; }

    public float MouseX { get; private set; }

    private ActionManager _actions;

    private void Start()
    {
        this.UIButtons = this.UIButtons ?? InputSystem.actions.FindActionMap("UI");
        this.Player = this.Player ?? InputSystem.actions.FindActionMap("Player");

        this.TogglePause = this.TogglePause ?? this.UIButtons.FindAction("TogglePause");

        this.MoveButtons = this.Player.FindAction("Move");
        this.Jump = this.Player.FindAction("Jump");
        this.Attack = this.Player.FindAction("Attack");

        this.MouseX = this.GetMouseX();

        this._actions = this._actions ?? FindAnyObjectByType<ActionManager>();
        this._actions.Subscribe(this.HandleActions);
    }

    private void HandleActions(string message)
    {
        switch (message)
        {
            default: break;
        }
    }

    public static void EnableCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public static void DisableCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private float GetMouseX()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        return mouseDelta.x * Statics.Meta.MouseSensitivity * Time.deltaTime;
    }

    private void Update()
    {
        if (this.TogglePause.WasPressedThisFrame())
        {
            this._actions.Notify(WorldController.IsGameOver ? Statics.Meta.MainMenuRequest : Statics.Meta.TogglePause);
        }

        this.MouseX = this.GetMouseX();
    }
}


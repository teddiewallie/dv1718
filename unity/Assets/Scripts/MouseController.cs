using UnityEngine;
using UnityEngine.InputSystem;

public class MouseController : MonoBehaviour
{
    public float X { get; private set; }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        X = this.GetX();
    }

    private float GetX()
    {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        return mouseDelta.x * Statics.MouseSensitivity * Time.deltaTime;
    }

    private void Update()
    {
        X = this.GetX();
    }
}



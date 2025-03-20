using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    private float RotationX;
    private float RotationY;

    public float Sensitivity;

    public float VerticalRotation;

    public Texture2D cursorTexture;
    Vector2 hotSpot = Vector2.zero;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Cursor.SetCursor(cursorTexture, hotSpot, CursorMode.Auto);
    }

    void Update()
    {
        RotationX = Input.GetAxis("Mouse X") * Sensitivity;
        RotationY = Input.GetAxis("Mouse Y") * Sensitivity;
        transform.Rotate(Vector3.up * RotationX);

        VerticalRotation -= RotationY;
        VerticalRotation = Mathf.Clamp(VerticalRotation, -90f, 90f);

        Camera.main.transform.localRotation = Quaternion.Euler(VerticalRotation, 0f, 0f);
    }
}

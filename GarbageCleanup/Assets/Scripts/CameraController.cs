using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Mouse Movement")]
    public float mouseSensitivity = 3f;
    public float smoothing = 1.5f;
    private Vector2 mouseLook;
    private Vector2 smoothMovement;

    [Header("Game Objects")]
    private GameObject player;
    public GameObject poker;

    [Header("Script Linking")]
    public PlayerController playerController;

    // Start is called before the first frame update
    void Start()
    {
        // Get ref to player
        player = transform.parent.gameObject;

        // Makes the cursor invisible, hit esc to get it back
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mouseDirection = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Multiply mouse input by sens and smoothing
        mouseDirection.x *= mouseSensitivity * smoothing;
        mouseDirection.y *= mouseSensitivity * smoothing;

        // Linear Interpolation (Smooth move between 2 locations)
        // 1 / smoothing is a cheap way of normalizing
        smoothMovement.x = Mathf.Lerp(smoothMovement.x, mouseDirection.x, 1f / smoothing);
        smoothMovement.y = Mathf.Lerp(smoothMovement.y, mouseDirection.y, 1f / smoothing);

        // Add calcs together
        mouseLook += smoothMovement;

        // Clamp the mouse position so player can't rotate infinitely on the x axis
        mouseLook.y = Mathf.Clamp(mouseLook.y, -80f, 90f);

        // Rotate camera to newly calculated position
        transform.localRotation = Quaternion.AngleAxis(-mouseLook.y, Vector3.right);

        // Player rotation
        player.transform.rotation = Quaternion.AngleAxis(mouseLook.x, player.transform.up);
    }
}

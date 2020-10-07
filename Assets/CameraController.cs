using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
#if UNITY_STANDALONE

    // How quickly the camera moves
    public float panSpeed = 20f;

    // How quickly the camera rotates
    public float rotSpeed = 10f;

    // How quickly the camera zooms
    public float zoomSpeed = 50f;

    // The minimum distance of the mouse cursor from the screen edge required to pan the camera
    public float borderWidth = 10f;

    // Boolean to control if moving the mouse within the borderWidth distance will pan the camera
    public bool edgeScrolling = true;

    // A placeholder for a reference to the camera in the scene
    public Camera cam;


    // Minimum distance from the camera to the camera target
    private float zoomMin = 3.0f;

    // Maximum distance from the camera to the camera target
    private float zoomMax = 49.0f;

    // Floats to hold reference to the mouse position, no values to be assigned yet
    private float mouseX, mouseY;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
        Zoom();
    }

    void Move()
    {
        // Local variable to hold the camera target's position during each frame
        Vector3 pos = transform.position;

        // Local variable to reference the direction the camera is facing (Which is driven by the Camera target's rotation)
        Vector3 forward = transform.forward;

        // Ensure the camera target doesn't move up and down
        //forward.y = 0;

        // Normalize the X, Y & Z properties of the forward vector to ensure they are between 0 & 1
        forward.Normalize();

        // Local variable to reference the direction the camera is facing + 90 clockwise degrees (Which is driven by the Camera target's rotation)
        Vector3 right = transform.right;
        right.y = 0;
        right.Normalize();

        if (SystemInfo.deviceType == DeviceType.Handheld)
        {
        }
        else
        {
            // Move the camera (camera_target) Forward relative to current rotation if "W" is pressed or if the mouse moves within the borderWidth distance from the top edge of the screen
            if (Input.GetKey("w")) // edgeScrolling == true && Input.mousePosition.y >= Screen.height - borderWidth)
            {
                pos += forward * panSpeed * Time.deltaTime;
            }


            // Move the camera (camera_target) Backward relative to current rotation if "S" is pressed or if the mouse moves within the borderWidth distance from the bottom edge of the screen
            if (Input.GetKey("s") || edgeScrolling == true && Input.mousePosition.y <= borderWidth)
            {
                pos -= forward * panSpeed * Time.deltaTime;
            }


            // Move the camera (camera_target) Right relative to current rotation if "D" is pressed or if the mouse moves within the borderWidth distance from the right edge of the screen
            if (Input.GetKey("d") || edgeScrolling == true && Input.mousePosition.x >= Screen.width - borderWidth)
            {
                pos += right * panSpeed * Time.deltaTime;
            }
            //crm krsn72 80

            // Move the camera (camera_target) Left relative to current rotation if "A" is pressed or if the mouse moves within the borderWidth distance from the left edge of the screen
            if (Input.GetKey("a") || edgeScrolling == true && Input.mousePosition.x <= borderWidth)
            {
                pos -= right * panSpeed * Time.deltaTime;
            }
        }

        // Setting the camera target's position to the modified pos variable
        transform.position = Utility.GetGroundedPoint(pos) + new Vector3(0, 1f, 0);
    }

    void Rotate()
    {
        // If Mouse Button 1 is pressed, (the secondary (usually right) mouse button)
        if (Input.GetMouseButton(1))
        {
            // Our mouseX variable gets set to the X position of the mouse multiplied by the rotation speed added to it.
            mouseX += Input.GetAxis("Mouse X") * rotSpeed;

            // Our mouseX variable gets set to the Y position of the mouse multiplied by the rotation speed added to it.
            mouseY -= Input.GetAxis("Mouse Y") * rotSpeed;

            // Clamp the minimum and maximum angle of how far the camera can look up and down.
            mouseY = Mathf.Clamp(mouseY, -30, 45);

            // Set the rotation of the camera target along the X axis (pitch) to mouseY (up & down) & Y axis (yaw) to mouseX (left & right), the Z axis (roll) is always set to 0 as we do not want the camera to roll.
            transform.rotation = Quaternion.Euler(mouseY, mouseX, 0);
        }
    }
    void Zoom()
    {
        // Local variable to temporarily store our camera's position
        Vector3 camPos = cam.transform.position;

        // Local variable to store the distance of the camera from the camera_target
        float distance = Vector3.Distance(transform.position, cam.transform.position);

        // When we scroll our mouse wheel up, zoom in if the camera is not within the minimum distance (set by our zoomMin variable)
        if (Input.GetAxis("Mouse ScrollWheel") > 0f && distance > zoomMin)
        {
            camPos += cam.transform.forward * zoomSpeed * Time.deltaTime;
        }

        // When we scroll our mouse wheel down, zoom out if the camera is not outside of the maximum distance (set by our zoomMax variable)
        if (Input.GetAxis("Mouse ScrollWheel") < 0f && distance < zoomMax)
        {
            camPos -= cam.transform.forward * zoomSpeed * Time.deltaTime;
        }

        // Set the camera's position to the position of the temporary variable
        cam.transform.position = camPos;
    }
#endif
}
using UnityEngine;

public class RotateObject : MonoBehaviour
{
    public float rotationSpeed = 60.0f; // Speed of rotation in degrees per second
    public Vector3 rotationAxis = Vector3.up; // Axis of rotation
    public bool rotateClockwise = false; // Rotate clockwise if true

    private bool isRotating = true; // Indicates whether rotation is enabled
    private float currentRotationSpeed;

    void Start()
    {
        currentRotationSpeed = rotateClockwise ? -rotationSpeed : rotationSpeed;
    }

    void Update()
    {
        if (isRotating)
        {
            // Rotate the object around the specified axis
            transform.Rotate(rotationAxis * currentRotationSpeed * Time.deltaTime);
        }
    }

    // Function to toggle rotation on/off
    public void ToggleRotation()
    {
        isRotating = !isRotating;
    }

    // Function to set the rotation axis
    public void SetRotationAxis(Vector3 axis)
    {
        rotationAxis = axis.normalized; // Normalize the input vector to ensure it's a unit vector
    }

    // Function to set the rotation speed
    public void SetRotationSpeed(float speed)
    {
        rotationSpeed = speed;
        currentRotationSpeed = rotateClockwise ? -rotationSpeed : rotationSpeed;
    }

    // Function to change rotation direction
    public void ChangeRotationDirection()
    {
        rotateClockwise = !rotateClockwise;
        currentRotationSpeed = rotateClockwise ? -rotationSpeed : rotationSpeed;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScaling : MonoBehaviour
{
    public enum WallType { Top, Bottom, Left, Right };
    public      WallType wallToConfigure;

    public Vector3 posOffset        = Vector3.zero; // Offset the wall position
    public Vector3 scalingOffset    = Vector3.one; // Offset the wall scaling on each axis

    private Camera _mainCamera;

    private void Awake()
    {
        _mainCamera = Camera.main;
    }

    void Update()
    {
        // Calculate the distance from the camera to the walls
        float distanceToWalls = Mathf.Abs(transform.position.z - _mainCamera.transform.position.z);

        // Calculate the screen dimensions at the depth of the walls
        float screenHeight = 2.0f * distanceToWalls * Mathf.Tan(_mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float screenWidth = screenHeight * _mainCamera.aspect;

        Vector3 wallScale;
        Vector3 wallPosition;

        switch (wallToConfigure)
        {
            case WallType.Top:
            case WallType.Bottom:
                wallScale = new Vector3(screenWidth * scalingOffset.x, transform.localScale.y * scalingOffset.y, transform.localScale.z * scalingOffset.z);
                float yOffset = (wallToConfigure == WallType.Top) ? screenHeight / 2 - transform.localScale.y / 2 : -screenHeight / 2 + transform.localScale.y / 2;
                wallPosition = new Vector3(0, yOffset, distanceToWalls);
                break;
            case WallType.Left:
            case WallType.Right:
                wallScale = new Vector3(transform.localScale.x * scalingOffset.x, screenHeight * scalingOffset.y, transform.localScale.z * scalingOffset.z);
                float xOffset = (wallToConfigure == WallType.Left) ? -screenWidth / 2 + transform.localScale.x / 2 : screenWidth / 2 - transform.localScale.x / 2;
                wallPosition = new Vector3(xOffset, 0, distanceToWalls);
                break;
            default:
                wallScale = Vector3.one;
                wallPosition = Vector3.zero;
                break;
        }

        // Apply the offset to the wall position
        wallPosition += posOffset;
        
        // Apply the scale and position to the selected wall
        transform.localScale = wallScale;
        transform.position = _mainCamera.transform.position + _mainCamera.transform.TransformDirection(wallPosition);
    }
}

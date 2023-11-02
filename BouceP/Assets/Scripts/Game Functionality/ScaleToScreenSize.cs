using UnityEngine;

public class ScaleToScreenSize : MonoBehaviour
{
    [SerializeField] private float widthPercentage = 0.5f; // Percentage of screen width
    [SerializeField] private float heightPercentage = 0.5f; // Percentage of screen height
    [SerializeField] private float depthScale = 1f;   // Percentage of screen height
    [SerializeField] private float xPosPercentage = 0.5f; // Percentage of screen width for X position
    [SerializeField] private float yPosPercentage = 0.5f; // Percentage of screen height for Y position
    [SerializeField] private bool  onlyPosition = false;

    void Start()
    {
        float distanceToWalls = Mathf.Abs(transform.position.z - Camera.main.transform.position.z);
        float screenHeight = 2.0f * distanceToWalls * Mathf.Tan(Camera.main.fieldOfView * 0.5f * Mathf.Deg2Rad);
        float screenWidth = screenHeight * Camera.main.aspect;

        // Calculate the new scale based on the screen size
        float newScaleX = screenWidth * widthPercentage / 100f;
        float newScaleY = screenHeight * heightPercentage / 100f;

        if(!onlyPosition)
        {
            // Apply the new scale to the GameObject
            transform.localScale = new Vector3(newScaleX, newScaleY, depthScale);
        }

        // Calculate the new position based on screen percentages
        float newXPos = screenWidth * xPosPercentage / 100f - (screenWidth / 2f);
        float newYPos = screenHeight * yPosPercentage / 100f - (screenHeight / 2f);

        // Apply the new position to the GameObject
        transform.position = new Vector3(newXPos, newYPos, transform.position.z);
    }
}
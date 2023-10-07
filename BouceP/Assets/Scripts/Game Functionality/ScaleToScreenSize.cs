using UnityEngine;

public class ScaleToScreenSize : MonoBehaviour
{
    [SerializeField] private float widthPercentage  = 0.5f; // Percentage of screen width
    [SerializeField] private float heightPercentage = 0.5f; // Percentage of screen height
    [SerializeField] private float depthScale       = 1f; // Percentage of screen height

    void Start()
    {
        // Get the screen width and height
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate the new scale based on the screen size
        float newScaleX = screenWidth * widthPercentage / 100f;
        float newScaleY = screenHeight * heightPercentage / 100f;

        // Apply the new scale to the GameObject
        transform.localScale = new Vector3(newScaleX, newScaleY, depthScale);
    }
}
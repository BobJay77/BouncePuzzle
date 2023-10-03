using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectScaling : MonoBehaviour
{
    [SerializeField] float widthPercentage = 0.5f; // Percentage of screen width
    [SerializeField] float heightPercentage = 0.5f; // Percentage of screen height

    void Start()
    {
    }

    private void Update()
    {

        // Get the screen width and height
        float screenWidth = Screen.width;
        float screenHeight = Screen.height;

        // Calculate the new scale based on the screen size
        float newScaleX = screenWidth * widthPercentage / 100f;
        float newScaleY = screenHeight * heightPercentage / 100f;

        // Apply the new scale to the GameObject
        transform.localScale = new Vector3(newScaleX, newScaleY, transform.localScale.z);
    }
}

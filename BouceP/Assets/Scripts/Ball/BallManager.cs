using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    // Collection of bounce sounds
    //[SerializeField] private AudioCollection bounceSounds = null;

    [SerializeField] private float widthPercentage = 0.05f; // Percentage of screen width
    //[SerializeField] float heightPercentage = 0.5f; // Percentage of screen height

    private GameObject startPosition;

    void Start()
    {
        startPosition = GameObject.FindGameObjectWithTag("Projectile");


        // Get the screen width and height
        float screenWidth = Screen.width;
        //float screenHeight = Screen.height;

        // Calculate the new scale based on the screen size
        float newScaleX = screenWidth * widthPercentage / 100f;
        //float newScaleY = screenHeight * heightPercentage / 100f;

        // Apply the new scale to the GameObject
        transform.localScale = new Vector3(newScaleX, newScaleX, newScaleX);

        if (startPosition != null)
        {
            transform.position = startPosition.transform.position;
        }

    }

    // Control what happens when the player ball collides with something
    private void OnCollisionEnter(Collision collision)
    {
        if (GameSystem.instance.roundEnded) return;
        if (GameSystem.instance.GetState().ToString() != "Resolution") return;

        if (collision.gameObject.tag == "Goal") // Goal object collision
        {
            GameSystem.instance.roundEnded = true;
            GameSystem.instance.hitGoal = true;
        }
        else if (collision.gameObject.tag == "Wall") // Wall colllision
        {
            GameSystem.instance.currentBounces++;
            GameSystem.instance.bouncesGoal--;

            AudioClip clip = AudioManager.instance.hitsSFX[GameSystem.instance.CurrentSkinIndex];

            if (clip == null) return;

            AudioManager.instance.PlayOneShotSound(AudioManager.instance.hitsSFX.audioGroup,
                                                   clip,
                                                   Camera.main.transform.position,
                                                   AudioManager.instance.hitsSFX.volume,
                                                   AudioManager.instance.hitsSFX.spatialBlend,
                                                   AudioManager.instance.hitsSFX.priority);

            // Trigger Hit VFX when colliding with a wall
            GameSystem.instance.TriggerVFX(GameSystem.instance.loadedHitPrefab);
        }
        else if (collision.gameObject.tag == "Spike") // Collide with one of the obstacles tagged as "Spike"
        {
            GameSystem.instance.roundEnded = true;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    // Collection of bounce sounds
    [SerializeField] private AudioCollection bounceSounds = null;
    
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
        else if(collision.gameObject.tag == "Wall") // Wall colllision
        {
            GameSystem.instance.currentBounces++;
            GameSystem.instance.bouncesGoal--;

            // Play sound from bounces collection when colliding with a wall
            AudioManager.instance.PlayOneShotSound(bounceSounds.audioGroup,
                                                   bounceSounds.audioClip, 
                                                   transform.position,
                                                   bounceSounds.volume,
                                                   bounceSounds.spatialBlend,
                                                   bounceSounds.priority);

            // Trigger Hit VFX when colliding with a wall
            GameSystem.instance.TriggerVFX(GameSystem.instance.loadedHitPrefab);
        }
        else if (collision.gameObject.tag == "Spike") // Collide with one of the obstacles tagged as "Spike"
        {
            GameSystem.instance.actionText.text = "Hit Spike";
            GameSystem.instance.roundEnded = true;
        }
    }
}

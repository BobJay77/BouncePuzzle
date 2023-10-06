using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    // Collection of bounce sounds
    //[SerializeField] private AudioCollection bounceSounds = null;
    
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
            GameSystem.instance.actionText.text = "Hit Spike";
            GameSystem.instance.roundEnded = true;
        }
    }
}
